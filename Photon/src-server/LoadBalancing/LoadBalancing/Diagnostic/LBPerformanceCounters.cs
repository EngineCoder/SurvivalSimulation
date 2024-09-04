using System.Diagnostics;
using Photon.SocketServer.Diagnostics.Counters;
using Photon.SocketServer.Diagnostics.Counters.Wrappers;

namespace Photon.LoadBalancing.Diagnostic
{
    [PerfCounterCategory(GSLBCounters, PerformanceCounterInstanceLifetime.Process)]
    [PerfCounterCategory(MSLBCounters, PerformanceCounterInstanceLifetime.Process)]
    [PerfCounterCategory(MSLBOpsCounters, PerformanceCounterInstanceLifetime.Process)]
    public sealed class LBPerformanceCounters : PerfCounterManagerBase<LBPerformanceCounters>
    {
        private const string GSLBCounters = "Photon: GS LB Counters";
        private const string MSLBCounters = "Photon: MS LB Counters";
        private const string MSLBOpsCounters = "Photon: MS LB Operations Counters";

        static LBPerformanceCounters()
        {
            InitializeWithDefaults();
        }

        #region GS Countes

        [PerfCounter("GS: Peers", PerformanceCounterType.NumberOfItems32, "Number of connected peers", GSLBCounters)]
        public static PerformanceCounterWrapper GSPeers;

        [PerfCounter("GS: Connected Masters", PerformanceCounterType.NumberOfItems32, "Number of connected masters", GSLBCounters)]
        public static PerformanceCounterWrapper GSConnectedMasters;

        [PerfCounter("GS: Disconnects", PerformanceCounterType.NumberOfItems32, "Number of disconnects", GSLBCounters)]
        public static PerformanceCounterWrapper GSDisconnects;

        [PerfCounter("GS: ConnFails", PerformanceCounterType.NumberOfItems32, "Number of connection failed events", GSLBCounters)]
        public static PerformanceCounterWrapper GSConnFails;

        [PerfCounter("GS: Reconnects", PerformanceCounterType.NumberOfItems32, "Number of reconnects", GSLBCounters)]
        public static PerformanceCounterWrapper GSReconnects;

        #endregion

        #region MS Counters

        [PerfCounter("MS: Peers", PerformanceCounterType.NumberOfItems32, "Number of connected peers", MSLBCounters)] 
        public static PerformanceCounterWrapper MSPeers;

        [PerfCounter("MS: GS Online", PerformanceCounterType.NumberOfItems32, "Number of GS online", MSLBCounters)]
        public static PerformanceCounterWrapper MSGSOnline;

        [PerfCounter("MS: GS OoR", PerformanceCounterType.NumberOfItems32, "Number of GS out of rotation", MSLBCounters)]
        public static PerformanceCounterWrapper MSGSOoR;

        [PerfCounter("MS: GS Offline", PerformanceCounterType.NumberOfItems32, "Number of GS offline", MSLBCounters)]
        public static PerformanceCounterWrapper MSGSOffline;

        [PerfCounter("MS: GS Total", PerformanceCounterType.NumberOfItems32, "Number of GS connected to master", MSLBCounters)]
        public static PerformanceCounterWrapper MSGSTotal;


        [PerfCounter("MS: GS Contexts", PerformanceCounterType.NumberOfItems32, "Number of GS contexts existing on Master", MSLBCounters)]
        public static PerformanceCounterWrapper MSGSContexts;

        [PerfCounter("MS: NO GS Contexts", PerformanceCounterType.NumberOfItems32, "Number of GS contexts existing on Master without connected GS", MSLBCounters)]
        public static PerformanceCounterWrapper MSGSContextsAbandoned;

        [PerfCounter("MS: GS+", PerformanceCounterType.NumberOfItems32, "Number of GS connections", MSLBCounters)]
        public static PerformanceCounterWrapper MSGSConnects;

        [PerfCounter("MS: GS+/sec", PerformanceCounterType.RateOfCountsPerSecond32, "Number of GS connections per second", MSLBCounters)]
        public static PerformanceCounterWrapper MSGSConnectsPerSec;

        [PerfCounter("MS: GS-", PerformanceCounterType.NumberOfItems32, "Number of GS disconnections", MSLBCounters)]
        public static PerformanceCounterWrapper MSGSDisconnects;

        [PerfCounter("MS: GS-/sec", PerformanceCounterType.RateOfCountsPerSecond32, "Number of GS disconnections per second", MSLBCounters)]
        public static PerformanceCounterWrapper MSGSDisconnectsPerSec;

        [PerfCounter("MS: GS Reconnects", PerformanceCounterType.NumberOfItems32, "Number of GS reconnections", MSLBCounters)]
        public static PerformanceCounterWrapper MSGSReconnects;

        [PerfCounter("MS: GS Reconnects/sec", PerformanceCounterType.RateOfCountsPerSecond32, "Number of GS reconnections per second", MSLBCounters)]
        public static PerformanceCounterWrapper MSGSReconnectsPerSec;


        [PerfCounter("MS: ClientOp ET", PerformanceCounterType.AverageTimer32, "ExecTime for all client ops in lobby", MSLBOpsCounters)]
        public static AverageCounterWrapper MSLobbyOpsET;

        [PerfCounter("MS: NonClientOp ET", PerformanceCounterType.AverageTimer32, "ExecTime for all non client tasks in lobby", MSLBOpsCounters)]
        public static AverageCounterWrapper MSLobbyTasksET;

        [PerfCounter("MS: JoinTimeoutCheck ET", PerformanceCounterType.AverageTimer32, "ExecTime for join timeout check in lobby", MSLBOpsCounters)]
        public static AverageCounterWrapper MSLobbyJoinTimeoutCheckET;

        [PerfCounter("MS: PublishGames ET", PerformanceCounterType.AverageTimer32, "ExecTime for games changes publishing in lobby", MSLBOpsCounters)]
        public static AverageCounterWrapper MSLobbyPublishGamesET;



        [PerfCounter("MS: CreateGame ET", PerformanceCounterType.AverageTimer32, "ExecTime for create game", MSLBOpsCounters)]
        public static AverageCounterWrapper MSCreateGameET;

        [PerfCounter("MS: JoinGame ET", PerformanceCounterType.AverageTimer32, "ExecTime for join game", MSLBOpsCounters)]
        public static AverageCounterWrapper MSJoinGameET;

        [PerfCounter("MS: JoinRandomGame ET", PerformanceCounterType.AverageTimer32, "ExecTime for join random game", MSLBOpsCounters)]
        public static AverageCounterWrapper MSJoinRandomGameET;

        [PerfCounter("MS: FindFriends ET", PerformanceCounterType.AverageTimer32, "ExecTime for find friends", MSLBOpsCounters)]
        public static AverageCounterWrapper MSFindFriendsET;

        //[PerfCounter("MS: CreateGame TT", PerformanceCounterType.AverageTimer32, "Travel Time for create game", MSLBOpsCounters)]
        //public static AverageCounterWrapper MSCreateGameTT;

        //[PerfCounter("MS: JoinGame TT", PerformanceCounterType.AverageTimer32, "Travel Time for join game", MSLBOpsCounters)]
        //public static AverageCounterWrapper MSJoinGameTT;

        //[PerfCounter("MS: JoinRandomGame TT", PerformanceCounterType.AverageTimer32, "Travel Time for join random game", MSLBOpsCounters)]
        //public static AverageCounterWrapper MSJoinRandomGameTT;

        //[PerfCounter("MS: FindFriends TT", PerformanceCounterType.AverageTimer32, "Travel Time for find friends", MSLBOpsCounters)]
        //public static AverageCounterWrapper MSFindFriendsTT;


        [PerfCounter("MS: CreateGame Enqueued", PerformanceCounterType.NumberOfItems32, "create game enqueued to lobby", MSLBOpsCounters)]
        public static PerformanceCounterWrapper MSCreateGameEnq;

        [PerfCounter("MS: JoinGame Enqueued", PerformanceCounterType.NumberOfItems32, "join game enqueued to lobby", MSLBOpsCounters)]
        public static PerformanceCounterWrapper MSJoinGameEnq;

        [PerfCounter("MS: JoinRandomGame Enqueued", PerformanceCounterType.NumberOfItems32, "join random game enqueued to lobby", MSLBOpsCounters)]
        public static PerformanceCounterWrapper MSJoinRandomGameEnq;

        [PerfCounter("MS: FindFriends Enqueued", PerformanceCounterType.NumberOfItems32, "find friends enqueued to lobby", MSLBOpsCounters)]
        public static PerformanceCounterWrapper MSFindFriendsEnq;



        [PerfCounter("MS: Client Ops Enqd", PerformanceCounterType.NumberOfItems32, "all client ops enqueued to lobby", MSLBOpsCounters)]
        public static PerformanceCounterWrapper MSLobbyClientOpsEnq;

        [PerfCounter("MS: Lobby Tasks Enqd", PerformanceCounterType.NumberOfItems32, "all possible tasks enqueued to lobby", MSLBOpsCounters)]
        public static PerformanceCounterWrapper MSLobbyTaskEnq;

        [PerfCounter("MS: Client Ops Rate", PerformanceCounterType.RateOfCountsPerSecond32, "rate of client ops enqueued to lobby", MSLBOpsCounters)]
        public static PerformanceCounterWrapper MSLobbyClientOpsRate;

        [PerfCounter("MS: Lobby Tasks Rate", PerformanceCounterType.RateOfCountsPerSecond32, "rate tasks enqueued to lobby", MSLBOpsCounters)]
        public static PerformanceCounterWrapper MSLobbyTaskRate;

        [PerfCounter("MS: Joinable Games", PerformanceCounterType.NumberOfItems32, "amount of joinable games to lobby", MSLBOpsCounters)]
        public static PerformanceCounterWrapper MSJoinableGamesCount;

        [PerfCounter("MS: Lobby Games", PerformanceCounterType.NumberOfItems32, "amount of all games in lobby", MSLBOpsCounters)]
        public static PerformanceCounterWrapper MSLobbyGamesCount;

        #endregion

    }
}
