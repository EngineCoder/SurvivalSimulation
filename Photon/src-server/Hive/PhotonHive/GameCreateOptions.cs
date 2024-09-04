using ExitGames.Concurrency.Fibers;

using Photon.Hive.Caching;
using Photon.Hive.Common;
using Photon.Hive.Plugin;
using Photon.Plugins.Common;

using System.Collections.Generic;

namespace Photon.Hive
{
    public struct GameCreateOptions
    {
        private IPluginLogMessagesCounter logMessagesCounter;

        public string GameName { get; set; }
        public RoomCacheBase RoomCache { get; set; }
        public IGameStateFactory GameStateFactory { get; set; }
        public int MaxEmptyRoomTTL { get; set; }
        public IPluginManager PluginManager { get; set; }
        public Dictionary<string, object> Environment { get; set; }
        public GameHttpQueueSettings HttpRequestQueueOptions { get; set; }
        public ExtendedPoolFiber ExecutionFiber { get; set; }
        public IPluginLogMessagesCounter LogMessagesCounter 
        {
            get => this.logMessagesCounter;

            set => this.logMessagesCounter = value ?? NullPluginLogMessageCounter.Instance;
        }

        public IHiveLogContext HiveLogContext { get; set; }

        public PropertiesLimitsSettings PropertiesLimits { get; set; }

        public GameCreateOptions(string gameName, 
            RoomCacheBase roomCache, 
            IPluginManager pluginManager, 
            int maxEmptyRoomTtl = 0, IGameStateFactory gameStateFactory = null)
        : this()
        {
            this.logMessagesCounter = NullPluginLogMessageCounter.Instance;
            this.HiveLogContext = Diagnostics.HiveLogContext.Default;

            this.GameName = gameName;
            this.RoomCache = roomCache;
            this.PluginManager = pluginManager;
            this.MaxEmptyRoomTTL = maxEmptyRoomTtl;
            this.GameStateFactory = gameStateFactory;
            this.Environment = null;
            this.HttpRequestQueueOptions = new GameHttpQueueSettings();
            this.ExecutionFiber = null;
        }
    }
}