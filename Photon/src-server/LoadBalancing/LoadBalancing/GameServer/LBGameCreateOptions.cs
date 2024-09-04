using System.Collections.Generic;
using ExitGames.Concurrency.Fibers;
using Photon.Hive;
using Photon.Hive.Common;
using Photon.Hive.Plugin;
using Photon.Plugins.Common;

namespace Photon.LoadBalancing.GameServer
{
    public struct LBGameCreateOptions
    {
        private static readonly GameHttpQueueSettings DefaultHttpRequestQueueOptions = GameServerSettings.Default.HttpQueueSettings;
        private GSLogContext logContext;
        private GameCreateOptions gameCreateOptions;

        public GameCreateOptions GameCreateOptions
        {
            get => this.gameCreateOptions;
            set => this.gameCreateOptions = value;
        }

        public GameApplication Application { get; set; }

        public GSLogContext LogContext
        {
            get => this.logContext;
            set
            {
                this.gameCreateOptions.HiveLogContext = value;
                this.logContext = value;
            }
        }

        public LBGameCreateOptions(GameApplication application,
            string gameId,
            Hive.Caching.RoomCacheBase roomCache = null,
            IPluginManager pluginManager = null,
            Dictionary<string, object> environment = null,
            ExtendedPoolFiber executionFiber = null,
            IPluginLogMessagesCounter logMessagesCounter = null,
            PropertiesLimitsSettings propertiesLimits = null
        )
            : this()
        {
            this.Application = application;
            this.GameCreateOptions = new GameCreateOptions(gameId, roomCache, pluginManager, GameServerSettings.Default.MaxEmptyRoomTTL)
            {
                HttpRequestQueueOptions = DefaultHttpRequestQueueOptions,
                Environment = environment,
                ExecutionFiber = executionFiber,
                LogMessagesCounter = logMessagesCounter,
                PropertiesLimits = propertiesLimits ?? GameServerSettings.Default.Limits.Inbound.Properties,
                HiveLogContext = GSLogContext.Default,
            };
        }
    }
}