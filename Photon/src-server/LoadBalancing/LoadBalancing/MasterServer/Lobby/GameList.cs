// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GameList.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the GameList type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using ExitGames.Logging;

using Photon.Common;
using Photon.LoadBalancing.Diagnostic;
using Photon.LoadBalancing.Operations;
using Photon.SocketServer.Diagnostics;

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Photon.LoadBalancing.MasterServer.Lobby
{

    public class GameList : GameListBase
    {
        #region Constants and Fields

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        private static readonly LogCountGuard noLobbyFilterGuard = new LogCountGuard(new TimeSpan(0, 0, 0, 10));

        private readonly Random rnd = new Random();

        private readonly LinkedList<GameState> joinableGames = new LinkedList<GameState>();

        protected LinkedListNode<GameState> nextJoinRandomStartNode;


        #endregion

        #region Constructors and Destructors

        public GameList(AppLobby lobby)
            : base(lobby)
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("Creating new GameList");
            }
        }

        #endregion

        #region Public Methods

        public override ErrorCode TryGetRandomGame(JoinRandomGameRequest joinRequest, ILobbyPeer peer, out GameState gameState, out string message)
        {
            message = null;

            if (this.joinableGames.Count == 0)
            {
                gameState = null;
                return ErrorCode.NoMatchFound;
            }

            LinkedListNode<GameState> startNode;
            switch ((JoinRandomType)joinRequest.JoinRandomType)
            {
                default:
                case JoinRandomType.FillRoom:
                    startNode = this.joinableGames.First;
                    break;

                case JoinRandomType.SerialMatching:
                    startNode = this.nextJoinRandomStartNode ?? this.joinableGames.First;
                    break;

                case JoinRandomType.RandomMatching:
                    var startNodeIndex = this.rnd.Next(this.joinableGames.Count);
                    startNode = GetAtIndex(this.joinableGames, startNodeIndex);
                    break;
            }

            if (!this.TryGetRandomGame(startNode, joinRequest, peer, out gameState))
            {
                return ErrorCode.NoMatchFound;
            }

            return ErrorCode.Ok;
        }

        #endregion

        #region Methods

        private static LinkedListNode<GameState> GetAtIndex(LinkedList<GameState> list, int index)
        {
            if (index >= list.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "The specified index is out of range");
            }

            if (index < list.Count / 2)
            {
                var node = list.First;
                for (int i = 0; i < index; i++)
                {
                    Debug.Assert(node != null, "node != null");
                    node = node.Next;
                }

                return node;
            }
            else
            {
                var node = list.Last;
                for (int i = list.Count - 1; i > index; i--)
                {
                    Debug.Assert(node != null, "node != null");
                    node = node.Previous;
                }

                return node;
            }
        }

        private bool TryGetRandomGame(LinkedListNode<GameState> startNode, JoinRandomGameRequest joinRequest, ILobbyPeer peer, out GameState gameState)
        {
            var node = startNode;
            do
            {
                var game = node.Value;
                node = node.Next ?? this.joinableGames.First;

                if (!IsGameJoinable(joinRequest, peer, game))
                {
                    continue;
                }

                if (joinRequest.GameProperties != null && game.MatchGameProperties(joinRequest.GameProperties) == false)
                {
                    continue;
                }

                if (log.IsDebugEnabled)
                {
                    log.DebugFormat("Found match. Next start node gameId={0}", node.Value.Id);
                }

                if (!game.HasLobbyPropertiesFilter)
                {
                    if (log.IsInfoEnabled)
                    {
                        log.InfoFormat(noLobbyFilterGuard, "Game '{0}' has not lobby filter but uses properties", node.Value.Id);
                    }
                }

                this.nextJoinRandomStartNode = node;
                gameState = game;
                return true;
            }
            while (node != startNode);

            gameState = null;
            return false;
        }


        public override void OnGameJoinableChanged(GameState gameState)
        {
            base.OnGameJoinableChanged(gameState);

            if (gameState.IsJoinable)
            {
                var node = this.joinableGames.AddLast(gameState);
                gameState.JoinableListNode = node;
                LBPerformanceCounters.MSJoinableGamesCount.Increment();
            }
            else
            {
                if (this.nextJoinRandomStartNode != null && this.nextJoinRandomStartNode.Value == gameState)
                {
                    this.AdvanceNextJoinRandomStartNode();
                }

                this.joinableGames.Remove(gameState.JoinableListNode);
                gameState.JoinableListNode = null;
                LBPerformanceCounters.MSJoinableGamesCount.Decrement();
            }
        }

        protected override bool RemoveGameState(GameState gameState)
        {
            if (this.nextJoinRandomStartNode != null && this.nextJoinRandomStartNode.Value == gameState)
            {
                this.AdvanceNextJoinRandomStartNode();
            }

            if (gameState.JoinableListNode != null)
            {
                this.joinableGames.Remove(gameState.JoinableListNode);
                gameState.JoinableListNode = null;
                LBPerformanceCounters.MSJoinableGamesCount.Decrement();
            }
            return base.RemoveGameState(gameState);
        }

        private void AdvanceNextJoinRandomStartNode()
        {
            if (this.nextJoinRandomStartNode == null)
            {
                return;
            }

            if (log.IsDebugEnabled)
            {
                log.DebugFormat(
                    "Changed last join random match: oldGameId={0}, newGameId={1}",
                    this.nextJoinRandomStartNode.Value.Id,
                    this.nextJoinRandomStartNode.Next == null ? "{null}" : this.nextJoinRandomStartNode.Value.Id);
            }

            this.nextJoinRandomStartNode = this.nextJoinRandomStartNode.Next;
        }


        #endregion
    }
}