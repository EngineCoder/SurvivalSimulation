//#define AutoRejoin
//#define USE_ACTOR_DEBUG_STRING
using ExitGames.Logging;

using Photon.Common;
using Photon.Hive.Operations;
using Photon.Hive.Plugin;
using Photon.SocketServer;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace Photon.Hive.Collections
{
    public class ActorsManager : IEnumerable<Actor>
    {
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        protected readonly List<Actor> allActors = new List<Actor>();
        /// <summary>
        ///   The actor number counter is increase whenever a new <see cref = "Actor" /> joins the game.
        /// </summary>
        private int actorNumberCounter;


        private List<ExcludedActorInfo> excludedActors = new List<ExcludedActorInfo>();
        private List<string> expectedActors = new List<string>();

        public ActorsManager()
        {
        }

        #region Properties

        public int Count { get { return this.allActors.Count; } }

        public int ActiveActorsCount { get { return this.allActors.Count(a => a.IsActive); } }

        public int InactiveActorsCount { get { return this.allActors.Count(a => a.IsInactive); } }

        public int ActorNumberCounter
        {
            get { return this.actorNumberCounter; }
            set { this.actorNumberCounter = value; }
        }

        public List<ExcludedActorInfo> ExcludedActors
        {
            get { return this.excludedActors; }
            set { this.excludedActors = value ?? new List<ExcludedActorInfo>(); }
        }


        public IEnumerable<Actor> AllActors
        {
            get
            {
                return this;
            }
        }

        public IEnumerable<Actor> ActiveActors
        {
            get
            {
                return this.allActors.Where(t => t.IsActive);
            }
        }

        public IEnumerable<Actor> InactiveActors
        {
            get
            {
                return this.allActors.Where(t => t.IsInactive);
            }
        }

        public int YetExpectedUsersCount
        {
            get
            {
                return this.expectedActors.Count(userId => this.GetActorByUserId(userId) == null);
            }
        }

        public List<string> ExpectedUsers
        {
            get { return this.expectedActors; }
            set { this.expectedActors = value; }
        }

        #endregion 

        #region Common Publics

        /// <summary>
        /// Tries to add a <see cref="HivePeer"/> to this game instance.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="peer">
        ///     The peer to add.
        /// </param>
        /// <param name="actorNr">
        ///     The actor Nr.
        /// </param>
        /// <param name="actor">
        ///     When this method returns this out param contains the <see cref="Actor"/> associated with the <paramref name="peer"/>.
        /// </param>
        /// <param name="errorCode">error code if error happened</param>
        /// <param name="reason">
        ///     reason why player can not be added
        /// </param>
        /// <param name="joinRequest"></param>
        /// <returns>
        /// Returns true if no actor exists for the specified peer and a new actor for the peer has been successfully added. 
        ///   The actor parameter is set to the newly created <see cref="Actor"/> instance.
        ///   Returns false if an actor for the specified peer already exists. 
        ///   The actor parameter is set to the existing <see cref="Actor"/> for the specified peer.
        /// </returns>
        public bool TryAddPeerToGame(HiveGame game, HivePeer peer, int actorNr, out Actor actor, 
            out ErrorCode errorCode, out string reason, JoinGameRequest joinRequest)
        {
            var isNewActor = false;

            if (!this.VerifyCanJoin(peer, actorNr, game.PlayerTTL, out actor, out errorCode, out reason, ref isNewActor, game.CheckUserOnJoin, joinRequest))
            {
                return false;
            }

            if (isNewActor)
            {
                actor = new Actor(peer);
                this.actorNumberCounter++;
                actor.ActorNr = this.actorNumberCounter;

                this.SanityChecks(peer, actorNr, actor, ref reason, game.CheckUserOnJoin);

                this.allActors.Add(actor);
#if USE_ACTOR_DEBUG_STRING
                actor.DbgString += $";Added:{DateTime.UtcNow}"; 
#endif
            }
            else
            {
#if USE_ACTOR_DEBUG_STRING
                actor.DbgString += $";Reactivating:{DateTime.UtcNow}"; 
#endif
                actor.Reactivate(peer);
            }

            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("Actor {0}: {1} {2} to game: {3} -- peer:{4}", isNewActor ? "added" : "reactivated", actor.ActorNr, actor.UserId, game.Name, peer.ToString());
            }

            reason = "";
            return true;
        }

        public int RemovePeerFromGame(HiveGame game, HivePeer peer, int playerTTL, bool isComingBack)
        {
            //if (Log.IsDebugEnabled)
            //{
            //    Log.DebugFormat("RemovePeerFromGame: conId={0}", peer.ConnectionId);
            //}


            var actorIndex = this.allActors.FindIndex(a => a.Peer == peer);
            if (actorIndex == -1)
            {
                if (Log.IsDebugEnabled)
                {
                    Log.DebugFormat("RemovePeerFromGame - Cant remove actor. It was not found for peer: {0}", peer);
                }

                return -1;
            }

            var actor = this.allActors[actorIndex];
            Debug.Assert(actor.IsActive);

            if (playerTTL != 0 && isComingBack)
            {
                // Note: deactivate actor first, so it doesn't receive its own leave event.
                // TBD - fix groups
#if USE_ACTOR_DEBUG_STRING
                actor.DbgString += $"DeactivatingPeer:{DateTime.UtcNow};"; 
#endif
                actor.Deactivate();

                game.OnActorDeactivated(actor);
                //Note: the player TTL can be set to never timeout (expected behavior specially for SavedGames)
                if (playerTTL > 0 && playerTTL != int.MaxValue)
                {
#if USE_ACTOR_DEBUG_STRING
                    actor.DbgString += $"PlayerTTL:{playerTTL}, isComingBack:{true}"; 
#endif
                    // setup cleanup timer
                    actor.CleanUpTimer = game.ExecutionFiber.Schedule(() => game.RemoveInactiveActor(actor), playerTTL);
                }

                if (Log.IsDebugEnabled)
                {
                    Log.DebugFormat("Actor {0}: {1} {2} to game: {3} -- peer:{4}", "deactivated", actor.ActorNr, actor.UserId, game.Name, peer.ToString());
                }

                var idx = this.allActors.IndexOf(actor);
                if (idx != -1)
                {
#if USE_ACTOR_DEBUG_STRING
                    actor.DbgString += "FoundInCollection"; 
#endif
                }
                else
                {
#if USE_ACTOR_DEBUG_STRING
                    actor.DbgString += "NOOOOTFoundInCollection"; 
#endif
                }

            }
            else
            {
#if USE_ACTOR_DEBUG_STRING
                actor.DbgString += $"RemovingActor:{DateTime.UtcNow};"; 
#endif
                this.allActors.RemoveAt(actorIndex);
                // cleanup does raise leave event
                game.OnActorRemoved(actor);

                if (Log.IsDebugEnabled)
                {
                    Log.DebugFormat("Actor {0}: {1} {2} to game: {3} -- peer:{4}", "removed", actor.ActorNr, actor.UserId, game.Name, peer.ToString());
                }
            }

            return actor.ActorNr;
        }

        public bool RemoveInactiveActor(HiveGame game, Actor actor)
        {
#if USE_ACTOR_DEBUG_STRING
            actor.DbgString += $"RemovingInActiveActor:{DateTime.UtcNow};"; 
#endif
            var idx = this.allActors.IndexOf(actor);
            if (idx != -1)
            {
                this.allActors.RemoveAt(idx);

                actor.RemovedFromInactive = DateTime.Now;

                game.OnActorRemoved(actor);

                if (Log.IsDebugEnabled)
                {
                    Log.DebugFormat("Actor {0}: {1} {2} to game: {3} -- TTL expired", "removed", actor.ActorNr, actor.UserId, game.Name);
                }
            }
            else
            {
                Log.Error($"Can not find actor:'{actor}' " +
                          $"in inactiveActors collection. timer:{(actor.CleanUpTimer != null ? "NOT null" : " < null > ")}. game: '{game}', actorsDump:{this.DumpActors()}");
            }
            return idx != -1;
        }

        /// <summary>
        /// we use this method to remove expired players after game restoring
        /// </summary>
        /// <param name="game"></param>
        public void DeactivateActors(HiveGame game)
        {
            if (!canPlayerTtlExpire(game.PlayerTTL))
            {
                // game should not contain any inactive actor
                return;
            }

            var actorsToCleanUp = new List<Actor>();
            var now = DateTime.Now;
            foreach (var actor in this.InactiveActors)
            {
                var closureActor = actor;
                if (actor.DeactivationTime.HasValue)
                {
                    if (Log.IsDebugEnabled)
                    {
                        Log.DebugFormat("Actor {0} clean up time is '{1}', now is '{2}'", actor.ActorNr, actor.DeactivationTime, now);
                    }
                    var scheduleTime = (int)((DateTime)actor.DeactivationTime - now).TotalMilliseconds + game.PlayerTTL;
                    if (scheduleTime > 0)
                    {
                        actor.CleanUpTimer = game.ExecutionFiber.Schedule(() => game.RemoveInactiveActor(closureActor), scheduleTime);
                        if (Log.IsDebugEnabled)
                        {
                            Log.DebugFormat("Actor {0} has clean up time. Clean up will happen in {1} sec", actor.ActorNr, scheduleTime / 1000);
                        }

#if USE_ACTOR_DEBUG_STRING
                        actor.DbgString += "DeactivateActors+Timer;"; 
#endif
                    }
                    else
                    {
                        if (Log.IsDebugEnabled)
                        {
                            Log.DebugFormat("Actor's {0} cleanup time expired. Cleanup as fast as possible", actor.ActorNr);
                        }
                        actorsToCleanUp.Add(closureActor);
#if USE_ACTOR_DEBUG_STRING
                        actor.DbgString += "DeactivateActors+NoTimer;"; 
#endif
                    }
                }
                else
                {
                    if (Log.IsDebugEnabled)
                    {
                        Log.DebugFormat("Actor {0} does not have clean up time. use PlayerTtl:{1}", actor.ActorNr, game.PlayerTTL);
                    }
#if USE_ACTOR_DEBUG_STRING
                    actor.DbgString += "DeactivateActors+NoDeactivationTime;"; 
#endif
                    actor.DeactivationTime = now;
                    actor.CleanUpTimer = game.ExecutionFiber.Schedule(() => game.RemoveInactiveActor(closureActor), game.PlayerTTL);
                }
            }

            foreach (var actor in actorsToCleanUp)
            {
                game.RemoveInactiveActor(actor);
            }
        }

        public void AddToExcludeList(string userId, byte reason)
        {
            this.excludedActors.Add(new ExcludedActorInfo { UserId = userId, Reason = reason });
        }

        public string DumpActors()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("Dump of Actors: ");
            foreach (var actor in this.AllActors)
            {
                if (actor == null)
                {
                    stringBuilder.Append("(null),");
                }
                else
                {
                    stringBuilder
                        .Append("(")
                        .AppendFormat("#:{0},usrId:'{1}',nick:'{2}',IsActv:{3}, p:{4}" +
#if USE_ACTOR_DEBUG_STRING
                                ", DbgStr:{5}", 
#endif
                                    actor.ActorNr, actor.UserId, actor.Nickname, actor.IsActive,
                                    actor.Peer != null ? actor.Peer.ToString() : "LastPeerInfo: " + actor.LastPeerInfo
#if USE_ACTOR_DEBUG_STRING
        ,
                                    actor.DbgString 
#endif
                                    )
                        .Append(")");
                }
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Method should be called either when game is closing - so no more usage for it
        /// or when game is going to be restored from state. in both cases there is no more need 
        /// in any type of actors. So we just clean up
        /// </summary>
        public void RemoveAllActors()
        {
            // kill clean up timers because otherwise they will complain that actor was not found
            foreach (var actor in this.InactiveActors)
            {
                actor.KillInActiveActorCleanUpTimer();
            }

            this.allActors.Clear();
        }

        public bool CheckMayAddSlots(string[] slots, int maxPlayers)
        {
            if (maxPlayers == 0)
            {
                return true;
            }

            var newSlotsCount = slots.Count(userId => this.GetActorByUserId(userId) == null && !this.IsExpectedUser(userId));

            var roomSize = this.Count + this.YetExpectedUsersCount;

            var playersCount = roomSize + newSlotsCount;

            return playersCount <= maxPlayers;
        }
        
        public bool CheckMayOverrideSlots(string[] newExpectedUsersArray, int maxPlayers)
        {
            if (maxPlayers == 0)
            {
                return true;
            }
            int expectedUsersNotJoinedYet = newExpectedUsersArray == null ? 0 : newExpectedUsersArray.Count(userId => this.GetActorByUserId(userId) == null);
            int roomSize = this.Count + expectedUsersNotJoinedYet;
            return roomSize <= maxPlayers;
        }

        public bool IsExpectedUser(string userId)
        {
            return this.expectedActors.IndexOf(userId) != -1;
        }
        
        public bool TryAddExpectedUsers(HiveGame hiveGame, JoinGameRequest joinRequest)
        {
            if (joinRequest.AddUsers == null)
            {
                return true;
            }

            if (!this.CheckMayAddSlots(joinRequest.AddUsers, hiveGame.MaxPlayers))
            {
                return false;
            }

            var added = joinRequest.AddUsers.Where(userId => !this.IsExpectedUser(userId)).ToArray();
            foreach (var userId in added)
            {
                this.expectedActors.Add(userId);
            }
            joinRequest.AddUsers = added.Length > 0 ? added : null;
            return true;
        }

        public Actor GetActorByUserId(string userId)
        {
            return GetActorByUserId(this, userId);
        }

        public Actor GetActorByNumber(int actorNr)
        {
            return GetActorByNumber(this, actorNr);
        }

        #region Serialization
        public void DeserializeActors(IList<SerializableActor> list)
        {
            if (this.allActors.Count > 0)
            {
                Log.WarnFormat("we deserialize actors but ActorManager is not empty. " +
                    "RemoveAllActors methos was not called at proper time. Actors:{0}", this.DumpActors());
            }
            this.allActors.Clear();
            foreach (var item in list)
            {
                this.allActors.Add(Actor.Deserialize(item));
            }
        }

        public List<SerializableActor> SerializeActors(bool withDebugInfo)
        {
            var actorList = new List<SerializableActor>();

            // enumerate all actors
            foreach (var actor in this)
            {
                actorList.Add(actor.Serialize(withDebugInfo));
            }
            return actorList;
        }

        #endregion

        #endregion

        #region Active Actors

        /// <summary>
        /// Gets an actor by the actor number.
        /// </summary>
        /// <param name="actorNumber">
        /// The actor number.
        /// </param>
        /// <returns>
        /// Return the actor with the specified actor number if found.
        /// If no actor with the specified actor number exits null will be returned.
        /// </returns>
        public Actor ActorsGetActorByNumber(int actorNumber)
        {
            return GetActorByNumber(this.ActiveActors, actorNumber);
        }

        /// <summary>
        /// Gets an actor by a specified peer.
        /// </summary>
        /// <param name="peer">
        /// The peer.
        /// </param>
        /// <returns>
        /// Returns the actor for the specified peer or null 
        /// if no actor for the specified peer was found.
        /// </returns>
        public Actor ActorsGetActorByPeer(PeerBase peer)
        {
            return this.ActiveActors.FirstOrDefault(actor => actor.Peer == peer);
        }

        /// <summary>
        /// Gets an actor by userId.
        /// </summary>
        /// <param name="userId">
        /// The userId to query for.
        /// </param>
        /// <returns>
        /// Returns the actor for the specified userId or null 
        /// if no actor was found.
        /// </returns>
        public Actor ActorsGetActorByUserId(string userId)
        {
            return GetActorByUserId(this.ActiveActors, userId);
        }

        /// <summary>
        /// Gets the actor numbers of all actors in this instance as an array.
        /// </summary>
        /// <returns>
        /// Array of the actor numbers.
        /// </returns>
        public IEnumerable<int> ActorsGetActorNumbers()
        {
            return GetActorNumbers(this.ActiveActors);
        }

        /// <summary>
        /// Returns all actors with the given actor numbers.
        /// </summary>
        /// <param name="actors">
        /// The actor numbers.
        /// </param>
        /// <returns>
        /// The actors with the given actor numbers.
        /// </returns>
        public IEnumerable<Actor> ActorsGetActorsByNumbers(int[] actors)
        {
            return GetActiveActorsByNumbers(this.allActors, actors);
        }

        /// <summary>
        /// Gets a list of actors in the room exluding a specified actor.
        /// This method can be used to get the actor list for an event, 
        /// where the actor causing the event should not be notified.
        /// </summary>
        /// <param name="actorToExclude">
        /// The actor to exclude.
        /// </param>
        /// <returns>
        /// the actors without <paramref name="actorToExclude"/>
        /// </returns>
        public IEnumerable<Actor> ActorsGetExcludedList(Actor actorToExclude)
        {
            return this.ActiveActors.Where(actor => actor != actorToExclude);
        }

        public IEnumerable<Actor> ActorsGetExcludedList(int actorToExclude)
        {
            return this.ActiveActors.Where(actor => actor.ActorNr != actorToExclude);
        }

        #endregion

        #region Inactive Actors
        public IEnumerable<int> InactiveActorsGetActorNumbers()
        {
            return GetActorNumbers(this.InactiveActors);
        }

        public Actor InactiveActorsGetActorByNumber(int actorNr)
        {
            return GetActorByNumber(this.InactiveActors, actorNr);
        }

        public Actor InactiveActorsGetActorByUserId(string userId)
        {
            return GetActorByUserId(this.InactiveActors, userId);
        }

        #endregion

        #region AllActors
        public Actor AllActorsGetActorByNumber(int actorNr)
        {
            return GetActorByNumber(this.AllActors, actorNr);
        }
        #endregion

        #region Enumeration

        public IEnumerator<Actor> GetEnumerator()
        {
            return this.allActors.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        #region Privates Methods

        // SanityChecks before we add the actor
        [Conditional("DEBUG")]
        private void SanityChecks(HivePeer peer, int actorNr, Actor actor, ref string reason, bool checkUserOnJoin)
        {
            Actor actorInCollection;
            if (!string.IsNullOrEmpty(peer.UserId) && checkUserOnJoin)
            {
                actorInCollection = this.allActors.Find(a => actorNr == a.ActorNr || a.UserId == peer.UserId);
            }
            else
            {
                actorInCollection = this.allActors.Find(a => actorNr == a.ActorNr);
            }
            if (actorInCollection != null)
            {
                reason =
                    $"Trying to add ActorNr already in list: in list: ActorNr: {actorInCollection.ActorNr}, userId:'{actorInCollection.UserId}', Active:{actorInCollection.IsActive}," +
                    $" trying to add: ActorNr:{actor.ActorNr}, userId:'{actor.UserId}'";
                Log.Warn(reason);
                //return false;
            }
        }


        private bool VerifyCanJoin(HivePeer peer, int actorNr, int playerTtl,
            out Actor actor, out Photon.Common.ErrorCode errorCode, out string reason, ref bool isNewActor, bool checkUserIdOnJoin, JoinGameRequest joinRequest)
        {
            // check if the peer already is linked to this game
            actor = this.ActorsGetActorByPeer(peer);
            if (actor != null)
            {
                reason = "Join failed: Peer already joined the specified game.";
                errorCode = Photon.Common.ErrorCode.JoinFailedPeerAlreadyJoined;
                return false;
            }

            reason = string.Empty;

            var joinMode = joinRequest.JoinMode;

            if (checkUserIdOnJoin)
            {
                if (string.IsNullOrEmpty(peer.UserId))
                {
                    // ERROR: should never happen with auto-generated userid's
                    //        without autogen-userid's its not supported 
                    reason = "Join failed: UserId is not set, checkUserIdOnJoin=true expects a UserId.";
                    errorCode = Photon.Common.ErrorCode.OperationInvalid;
                    return false;
                }
                if (!this.CheckExcludeList(peer.UserId, out reason))
                {
                    errorCode = Photon.Common.ErrorCode.JoinFailedFoundExcludedUserId;
                    return false;
                }

                // check if the userId already joined this game
                actor = GetActorByUserId(this, peer.UserId);
                if (actor != null)
                {
                    if (actor.IsActive)
                    {
                        if (!TrySameSessionRejoin(peer, playerTtl, actor, joinMode, joinRequest.ForceRejoin))
                        {
                            reason = string.Format(HiveErrorMessages.UserAlreadyJoined, peer.UserId, joinMode);
                            errorCode = Photon.Common.ErrorCode.JoinFailedFoundActiveJoiner;
                            return false;
                        }

                        errorCode = 0;
                        return true;
                    }

                    // actor.IsInactive
                    // verify that inactive actor is not expired, since actorlist could contain expired actors after reload
                    if (canPlayerTtlExpire(playerTtl) && actor.DeactivationTime.HasValue)
                    {
                        var scheduleTime = (int)((DateTime)actor.DeactivationTime - DateTime.Now).TotalMilliseconds + playerTtl;
                        if (scheduleTime <= 0)
                        {
                            reason = $"Join failed: UserId '{peer.UserId}' is expired (JoinMode={joinMode}).";
                            errorCode = Photon.Common.ErrorCode.JoinFailedWithRejoinerNotFound;
                            return false;
                        }
                    }
#if !AutoRejoin
                    if (joinMode != JoinModes.RejoinOnly && joinMode != JoinModes.RejoinOrJoin)
                    {
                        reason = $"Found inactive UserId '{peer.UserId}', but not rejoining (JoinMode={joinMode}).";
                        errorCode = Photon.Common.ErrorCode.JoinFailedFoundInactiveJoiner;
                        return false;
                    }
#endif
                    errorCode = 0;
                    return true;
                }
            }
            else
            {
                // actorNr > 0 => re-joining with actornr & without userid
                if (actorNr > 0)
                {
                    actor = GetActorByNumber(this, actorNr);
                    if (actor == null)
                    {
                        // not finding the actor was always an error - so NO JoinOrRejoin (NOR RejoinOrJoin)!
                        reason = $"Rejoin failed: actor nr={actorNr} not found.";
                        errorCode = Photon.Common.ErrorCode.JoinFailedWithRejoinerNotFound;
                        return false;
                    }

                    if (actor.IsActive)
                    {
                        if (!TrySameSessionRejoin(peer, playerTtl, actor, joinMode, false))
                        {
                            reason = string.Format(HiveErrorMessages.UserWithActorIdAlreadyJoined, actorNr, joinMode);
                            errorCode = Photon.Common.ErrorCode.JoinFailedFoundActiveJoiner;
                            return false;
                        }

                        errorCode = 0;
                        return true;
                    }

                    // TODO: comment above stays that we re-join without userid. 
                    // why we check it here? it also may be null or empty
                    // this may happen if checkUserOnJoin is false
                    if (actor.IsInactive && actor.UserId != peer.UserId)
                    {
                        if (!(Guid.TryParse(actor.UserId, out _) && Guid.TryParse(peer.UserId, out _)))
                        {
                            reason = "Rejoin failed: userId of peer doesn't match userid of actor.";

                            errorCode = Photon.Common.ErrorCode.InternalServerError;
                            return false;
                        }

                        Log.Debug("Rejoin: userId of peer doesn't match userid of actor. But are GUId's - assuming its AutoUserIds");
                    }

                    // actor is inActive
                    if (canPlayerTtlExpire(playerTtl) && actor.DeactivationTime.HasValue)
                    {
                        var scheduleTime = (int)((DateTime)actor.DeactivationTime - DateTime.Now).TotalMilliseconds + playerTtl;
                        if (scheduleTime <= 0)
                        {
                            reason = $"Join failed: ActorNr '{actorNr}' is expired.";
                            errorCode = Photon.Common.ErrorCode.JoinFailedWithRejoinerNotFound;
                            return false;
                        }
                    }
#if !AutoRejoin
                    if (joinMode != JoinModes.RejoinOnly && joinMode != JoinModes.RejoinOrJoin)
                    {
                        // should never happen, since actorn>0 means rejoinmode is set / see joingamerequest.actornr
                        reason = $"Found inactive ActorNr '{actorNr}', but not rejoining (JoinMode={joinMode}).";
                        errorCode = Photon.Common.ErrorCode.InternalServerError;
                        return false;
                    }
#endif
                    errorCode = 0;
                    return true;
                }
            }

            if (joinMode == JoinModes.RejoinOnly)
            {
                reason = $"Rejoin failed: userid={peer.UserId} not found, actor nr={actorNr}.";
                errorCode = Photon.Common.ErrorCode.JoinFailedWithRejoinerNotFound;
                return false;
            }

            // create new actor instance 
            isNewActor = true;
            errorCode = 0;
            return true;
        }

        private static bool TrySameSessionRejoin(HivePeer peer, int playerTtl, Actor actor, byte joinMode, bool forceRejoin)
        {
            if (playerTtl == 0
                || (joinMode != JoinModes.RejoinOnly && joinMode != JoinModes.RejoinOrJoin)
                || (!forceRejoin && !actor.Peer.IsThisSameSession(peer)) // only if this is a Rejoin, we follow through
                )
            {
                return false;
            }

#if USE_ACTOR_DEBUG_STRING
            actor.DbgString += $"SameSessionCheck:{DateTime.UtcNow}"; 
#endif
            var actorPeer = actor.Peer;
            actor.Deactivate();

            var dbgString = actorPeer.GetDebugString();
            actorPeer.SetDebugString(dbgString + $", SameSessionRejoinKick at {DateTime.UtcNow} by p:{peer}");
            // in order to prevent RemovePeerFromCurrentGame
            actorPeer.ReleaseRoomReference();
            actorPeer.Disconnect((int)ErrorCode.ConnectionSwitch);
            return true;
        }

        private bool CheckExcludeList(string userId, out string reason)
        {
            reason = string.Empty;
            var info = this.excludedActors.Find(a => a.UserId == userId);
            if (info == null)
            {
                return true;
            }

            switch (info.Reason)
            {
                case RemoveActorReason.Banned:
                    reason = $"User {userId} is banned in this game";
                    break;
                case RemoveActorReason.GlobalBanned:
                    reason = $"User {userId} is global banned";
                    break;
                default:
                    Debug.Assert(false, "CheckExcludeList failure", "CheckExcludeList: case {0} is not covered", info.Reason);
                    Log.ErrorFormat("CheckExcludeList: case {0} is not covered for user {1}", info.Reason, userId);
                    reason = $"Unknown reason {info.Reason} for excluding ";
                    break;
            }
            return false;
        }

        private static bool canPlayerTtlExpire(int playerTtl)
        {
            return playerTtl > 0 && playerTtl != int.MaxValue;
        }

        /// <summary>
        /// Checks whether an array is sorted.
        /// </summary>
        /// <param name="array">
        /// The array.
        /// </param>
        /// <returns>
        /// True if the array is sorted, otherwise false.
        /// </returns>
        private static bool IsSorted(int[] array)
        {
            for (var i = 1; i < array.Length; i++)
            {
                if (array[i - 1] > array[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Returns all actors with the given actor numbers.
        /// </summary>
        /// <param name="actorsList">list of actors to search</param>
        /// <param name="actors">
        /// The actor numbers.
        /// </param>
        /// <returns>
        /// The actors with the given actor numbers.
        /// </returns>
        private static IEnumerable<Actor> GetActiveActorsByNumbers(IList<Actor> actorsList, int[] actors)
        {
            if (!IsSorted(actors))
            {
                Array.Sort(actors);
            }

            for (int i = 0, j = 0; i < actors.Length; i++)
            {
                // since both lists are sorted we don't have to start at the beginning at each iteration
                for (; j < actorsList.Count; j++)
                {
                    if (actorsList[j].IsActive && actorsList[j].ActorNr == actors[i])
                    {
                        yield return actorsList[j];
                        break;
                    }
                }

                if (actorsList.Count == j)
                {
                    j = 0;
                }
            }
        }

        /// <summary>
        /// Gets an actor by the actor number.
        /// </summary>
        /// <param name="actorsList">list of actors to search</param>
        /// <param name="actorNumber">
        /// The actor number.
        /// </param>
        /// <returns>
        /// Return the actor with the specified actor number if found.
        /// If no actor with the specified actor number exits null will be returned.
        /// </returns>
        private static Actor GetActorByNumber(IEnumerable<Actor> actorsList, int actorNumber)
        {
            return actorsList.FirstOrDefault(actor => actor.ActorNr == actorNumber);
        }

        /// <summary>
        /// Gets an actor by userId.
        /// </summary>
        /// <param name="actorsList">list of actors to search</param>
        /// <param name="userId">
        /// The userId to query for.
        /// </param>
        /// <returns>
        /// Returns the actor for the specified userId or null 
        /// if no actor was found.
        /// </returns>
        private static Actor GetActorByUserId(IEnumerable<Actor> actorsList, string userId)
        {
            return actorsList.FirstOrDefault(actor => actor.UserId == userId);
        }

        /// <summary>
        /// Gets the actor numbers of all actors in this instance as an array.
        /// </summary>
        /// <returns>
        /// Array of the actor numbers.
        /// </returns>
        private static IEnumerable<int> GetActorNumbers(IEnumerable<Actor> actorsList)
        {
            return actorsList.Select(actor => actor.ActorNr);
        }

        #endregion
    }
}
