using ExitGames.Logging;

using Photon.Common.LoadBalancer.Common;
using Photon.Common.LoadBalancer.LoadShedding;
using Photon.LoadBalancing.Diagnostic;
using Photon.LoadBalancing.GameServer;
using Photon.LoadBalancing.ServerToServer.Events;
using Photon.LoadBalancing.ServerToServer.Operations;
using Photon.SocketServer;
using Photon.SocketServer.Diagnostics;

using System;
using System.Collections.Generic;
using System.Threading;

namespace Photon.LoadBalancing.MasterServer.GameServer
{
    public class GameServerContext : IComparable<GameServerContext>
    {
        #region Fields and Constants

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        private readonly GameServerContextManager contextManager;
        private readonly MasterApplication application;

        /// <summary>
        /// amount of peers on GS according to master server calculation
        /// </summary>
        private int msPeerCount;
        /// <summary>
        /// amount of peers on GS according to reports from GS
        /// </summary>
        private int gsPeerCount;


        protected ReplicationAssistantBase replicationAssistant;
        private IDisposable replicationFinishCheck;

        /// <summary>
        /// to keep amount of load levels that server uses
        /// </summary>
        private readonly byte loadLevelCount;

        private ContextState contextState;

        [Flags]
        public enum ContextState
        {
            Normal,
            ServerLeaving,
            ContextAbandoned,
        }

        #endregion

        #region .ctr

        public GameServerContext(GameServerContextManager gameServerContextManager, MasterApplication application, IRegisterGameServer request)
        {
            this.contextManager = gameServerContextManager;
            this.application = application;

            this.AddressInfo = GameServerAddressInfo.CreateAddressInfo(request, log);

            this.Key = GetKey(request);
            this.ServerId = request.ServerId;
            this.LoadBalancerPriority = request.LoadBalancerPriority;
            this.State = ServerState.Offline;
            this.loadLevelCount = request.LoadLevelCount;

            if (request.SupportedProtocols != null && request.SupportedProtocols.Length != 0)
            {
                this.SupportedProtocols = new List<byte>(request.SupportedProtocols);
            }

            LBPerformanceCounters.MSGSContexts.Increment();
        }

        #endregion

        #region Properties
        public string Key { get; }

        public string ServerId { get; }

        public GameServerAddressInfo AddressInfo { get; }

        public ServerState State { get; private set; }

        public IGameServerPeer Peer { get; private set; }

        public FeedbackLevel LoadLevel { get; private set; }
        /// <summary>
        /// Load Level with prediction
        /// </summary>
        public FeedbackLevel SmartLoadLevel { get; private set; }

        public byte LoadBalancerPriority { get; }


        public List<byte> SupportedProtocols { get; }

        public LogCountGuard OnServerDisconnectLogGuard { get; } = new LogCountGuard(new TimeSpan(0, 1, 0));
        #endregion

        #region Publics

        public static string GetKey(IRegisterGameServer request)
        {
            return $"{request.GameServerAddress}-{request.UdpPort}-{request.TcpPort}";
        }


        public void AttachPeerAndHandleRegisterRequest(IGameServerPeer peer, IRegisterGameServer request, bool reconnect)
        {
            this.AttachPeer(peer);
            this.SetStateAndLoadPrediction(request);

            if (reconnect)
            {
                this.OnGameServerReconnected();
            }
        }

        public void DetachPeerAndClose()
        {
            this.DetachPeer(this.Peer);
            this.CloseContext();
        }

        public bool DetachPeer(IGameServerPeer peer)
        {
            if (this.Peer == null || this.Peer != peer)
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug($"Context is bound to another peer. No detach done for peer:{peer}. bound peer:{this.Peer}");
                }
                return false;
            }

            this.Peer.DetachFromContext();
            this.Peer = null;

            this.OnPeerDetached();
            return true;
        }

        public void CloseContext()
        {
            this.application.LoadBalancer.TryRemoveServer(this, this.LoadBalancerPriority);

            this.application.OnServerWentOffline(this);

            this.OnContextClose();
        }

        public void OnGameServerDisconnect(IGameServerPeer peer, int reason)
        {
            this.contextManager.OnGameServerDisconnect(this, peer);
        }

        public void OnPlayerCountChanged(int playerCount, int oldPlayerCount)
        {
            var diff = playerCount - oldPlayerCount;
            Interlocked.Add(ref this.msPeerCount, diff);
        }

        public void HandleUpdateGameServerEvent(UpdateServerEvent updateGameServer)
        {
            var previousLoadLevel = this.SmartLoadLevel;
            var loadIndex = updateGameServer.LoadIndex;

            this.UpdateLoadLevel(loadIndex);

            this.gsPeerCount = updateGameServer.PeerCount;

            if ((ServerState)updateGameServer.State != this.State)
            {
                this.SetServerState((ServerState)updateGameServer.State);
            }
            else if (previousLoadLevel != this.SmartLoadLevel && this.State == ServerState.Normal)
            {
                if (log.IsDebugEnabled)
                {
                    log.DebugFormat("UpdateGameServer - from LoadLevel {0} to {1}, Reported LL:{3}, PeerCount {2}", previousLoadLevel, this.SmartLoadLevel, this.gsPeerCount, this.LoadLevel);
                }

                if (!this.application.LoadBalancer.TryUpdateServer(this, this.SmartLoadLevel, this.LoadBalancerPriority))
                {
                    log.WarnFormat("Failed to update game server state for {0}", this.AddressInfo.TcpAddress);
                }
            }
        }

        public void HandleReplicationHelperEvent(ReplicationHelperEvent finishEvent)
        {
            this.replicationAssistant.HandleReplicationHelperEvent(finishEvent);
        }

        public virtual void HandleUpdateGameEvent(UpdateGameEvent updateGameEvent)
        {
            this.application.DefaultApplication.OnGameUpdateOnGameServer(updateGameEvent, this);
            this.replicationAssistant.OnGameUpdateEventHandled(updateGameEvent);
        }

        public virtual void HandleGameUpdatesBatchEvent(GameUpdatesBatchEvent updateBatchEvent)
        {
            var gameApp = this.application.DefaultApplication;

            this.ParseBatchedUpdates(updateBatchEvent, gameApp);
        }

        public virtual void HandleRemoveGameState(RemoveGameEvent removeEvent)
        {
            this.application.DefaultApplication.OnGameRemovedOnGameServer(this, removeEvent.GameId, removeEvent.RemoveReason);
        }

        public virtual void HandleUpdateApplicationStats(IEventData eventData)
        {
            if (MasterApplication.AppStats != null)
            {
                var updateAppStatsEvent = new UpdateAppStatsEvent(this.Peer.Protocol, eventData);
                MasterApplication.AppStats.UpdateGameServerStats(this, updateAppStatsEvent.PlayerCount, updateAppStatsEvent.GameCount);
            }
        }

        public virtual void HandleGameServerLeave(IEventData eventData)
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("Got GameServerLeave event: {0}", this);
            }

            this.contextState = ContextState.ServerLeaving;

            var peer = this.Peer;
            this.contextManager.OnGameServerLeft(this, peer);

            peer.Disconnect(ErrorCodes.Ok);
        }

        public int CompareTo(GameServerContext other)
        {
            if (other == null)
            {
                return 1;
            }

            var result = string.Compare(this.AddressInfo.Hostname, other.AddressInfo.Hostname, StringComparison.Ordinal);
            if (result != 0)
            {
                return result;
            }

            return string.Compare(this.Key, other.Key, StringComparison.Ordinal);
        }

        public override string ToString()
        {
            return $"[key:{this.Key}, serverId:{this.ServerId}]";
        }


        public virtual void IncrementGameCreationTimeouts()
        {
            
        }

        public virtual void IncrementGameCreations()
        {
        }

        #endregion

        #region Methods

        private void AttachPeer(IGameServerPeer peer)
        {
            if (log.IsInfoEnabled)
            {
                log.Info($"Attaching new peer to context. old peer:{this.Peer}, new peer:{peer}, ctx:{this}");
            }

            if (this.Peer != null)
            {
                var p = this.Peer;
                this.DetachPeer(p);
                p.Disconnect(ErrorCodes.Ok);
            }

            this.Peer = peer;

            this.Peer.AttachToContext(this);

            this.OnPeerAttached();
        }

        protected virtual ReplicationAssistantBase GetReplicationAssistant()
        {
            return new ReplicationAssistant(this, this.application.DefaultApplication);
        }

        protected virtual void OnPeerAttached()
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("Peer attached to context:{0}", this);
            }

            this.replicationAssistant = this.GetReplicationAssistant();

            LBPerformanceCounters.MSGSTotal.Increment();
            AdjustCountersOnStateEnter(this.State);

            LBPerformanceCounters.MSGSConnects.Increment();
            LBPerformanceCounters.MSGSConnectsPerSec.Increment();

            this.ScheduleReplicationFinishCheck();

            if (this.contextState == ContextState.ContextAbandoned)
            {
                LBPerformanceCounters.MSGSContextsAbandoned.Decrement();
                this.contextState = ContextState.Normal;
            }
        }

        protected virtual void OnPeerDetached()
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("Peer detached from context:{0}", this);
            }
            this.StopReplicationCheck();
            this.StopReplication();

            LBPerformanceCounters.MSGSTotal.Decrement();
            AdjustCountersOnStateLeave(this.State);
            LBPerformanceCounters.MSGSDisconnects.Increment();
            LBPerformanceCounters.MSGSDisconnectsPerSec.Increment();
            if (this.contextState == ContextState.Normal)
            {
                LBPerformanceCounters.MSGSContextsAbandoned.Increment();
                this.contextState = ContextState.ContextAbandoned;
            }

        }

        private void StopReplication()
        {
            if (this.replicationAssistant != null)
            {
                this.replicationAssistant.Stop();
                this.replicationAssistant = null;
            }
        }

        protected void ScheduleReplicationFinishCheck()
        {
            this.replicationFinishCheck = this.Peer.RequestFiber.Schedule(this.ReplicationFinishCheck, MasterServerSettings.Default.GS.ReplicationTimeout);
        }

        private void ReplicationFinishCheck()
        {
            this.StopReplicationCheck();
            if (this.replicationAssistant != null && !this.replicationAssistant.IsReplicationComplete)
            {
                log.WarnFormat("Replication stopped by timeout in {0} seconds after start. context:{1}", 
                    MasterServerSettings.Default.GS.ReplicationTimeout / 1000, this);

                this.replicationAssistant.ForceFinishReplication();
            }
        }

        private void StopReplicationCheck()
        {
            var disp = this.replicationFinishCheck;
            if (disp != null)
            {
                disp.Dispose();
                this.replicationFinishCheck = null;
            }
        }

        protected virtual void OnGameServerStateChanged(ServerState oldState)
        {
            AdjustCountersOnStateLeave(oldState);

            AdjustCountersOnStateEnter(this.State);
        }

        protected virtual void OnContextClose()
        {
            LBPerformanceCounters.MSGSContexts.Decrement();
            if (this.contextState == ContextState.ContextAbandoned)
            {
                LBPerformanceCounters.MSGSContextsAbandoned.Decrement();
            }
        }

        protected virtual void OnGameServerReconnected()
        {
            LBPerformanceCounters.MSGSReconnects.Increment();
            LBPerformanceCounters.MSGSReconnectsPerSec.Increment();
        }

        private bool SetServerState(ServerState serverState)
        {
            if (this.State == serverState)
            {
                return false;
            }

            if (serverState < ServerState.Normal || serverState > ServerState.Offline)
            {
                log.WarnFormat("Invalid server state for {0}: old={1}, new={2}", this.AddressInfo.TcpAddress, this.State, serverState);
                return false;
            }

            if (log.IsDebugEnabled)
            {
                log.DebugFormat("GameServer state changed for {0}: old={1}, new={2} ", this.AddressInfo.TcpAddress, this.State, serverState);
            }
            var oldState = this.State;
            this.State = serverState;

            switch (serverState)
            {
                case ServerState.Normal:
                    if (this.application.LoadBalancer.TryAddServer(this, this.LoadLevel, this.LoadBalancerPriority) == false)
                    {
                        log.WarnFormat("Failed to add game server to load balancer: serverId={0}", this.ServerId);
                    }
                    break;

                case ServerState.OutOfRotation:
                    this.application.LoadBalancer.TryRemoveServer(this, this.LoadBalancerPriority);
                    break;

                case ServerState.Offline:
                    this.application.LoadBalancer.TryRemoveServer(this, this.LoadBalancerPriority);
                    this.application.OnServerWentOffline(this);
                    break;
            }
            this.OnGameServerStateChanged(oldState);

            return true;
        }

        private void UpdateLoadLevel(byte loadIndex)
        {
            if (this.loadLevelCount != (int)FeedbackLevel.LEVELS_COUNT)
            {
                this.LoadLevel = (FeedbackLevel)((loadIndex * (int)FeedbackLevel.LEVELS_COUNT) / (float)this.loadLevelCount);
            }
            else
            {
                this.LoadLevel = (FeedbackLevel)loadIndex;
            }

            this.SmartLoadLevel = this.LoadLevel;
        }

        private void SetStateAndLoadPrediction(IRegisterGameServer request)
        {
            this.UpdateLoadLevel(request.LoadIndex);

            if (!this.SetServerState((ServerState) request.ServerState))
            {
                this.application.LoadBalancer.TryUpdateServer(this, this.LoadLevel, this.LoadBalancerPriority, this.State);
            }

        }

        protected void ParseBatchedUpdates(GameUpdatesBatchEvent updateBatchEvent, GameApplication gameApp)
        {
            foreach (var batchedEvent in updateBatchEvent.BatchedEvents)
            {
                var updateEvent = new UpdateGameEvent(this.Peer.Protocol, batchedEvent);

                gameApp.OnGameUpdateOnGameServer(updateEvent, this);
                this.replicationAssistant.OnGameUpdateEventHandled(updateEvent);
            }
        }

        private static void AdjustCountersOnStateEnter(ServerState newState)
        {
            switch (newState)
            {
                case ServerState.Normal:
                    LBPerformanceCounters.MSGSOnline.Increment();
                    break;
                case ServerState.OutOfRotation:
                    LBPerformanceCounters.MSGSOoR.Increment();
                    break;
                case ServerState.Offline:
                    LBPerformanceCounters.MSGSOffline.Increment();
                    break;
            }
        }

        private static void AdjustCountersOnStateLeave(ServerState oldState)
        {
            switch (oldState)
            {
                case ServerState.Normal:
                    LBPerformanceCounters.MSGSOnline.Decrement();
                    break;
                case ServerState.OutOfRotation:
                    LBPerformanceCounters.MSGSOoR.Decrement();
                    break;
                case ServerState.Offline:
                    LBPerformanceCounters.MSGSOffline.Decrement();
                    break;
            }
        }


        #endregion
    }
}
