using System;
using Photon.Hive.Common;
using Photon.SocketServer.Diagnostics;

namespace Photon.Hive.Diagnostics
{
    public class HiveLogContext : IHiveLogContext, IHivePeerLogContext
    {
        public static readonly HiveLogContext Default = new HiveLogContext();

        //ILogContext

        public LogCountGuard MsgRateLogGuard { get; } = new LogCountGuard(new TimeSpan(0, 0, 10));
        public LogCountGuard DataRateLogGuard { get; } = new LogCountGuard(new TimeSpan(0, 0, 10));
        public LogCountGuard MsgSizeLogGuard { get; } = new LogCountGuard(new TimeSpan(0, 0, 10));
        public LogCountGuard DisconnectLogGuard { get; } = new LogCountGuard(new TimeSpan(0, 0, 10));

        // from HiveGame
        public LogCountGuard LobbyPropertiesValidatorLogGuard { get; } = new LogCountGuard(new TimeSpan(0, 0, 10));
        public LogCountGuard ForceGameCloseLogGuard { get; } = new LogCountGuard(new TimeSpan(0, 0, 10));
        public LogCountGuard ExceptionLogGuard { get; } = new LogCountGuard(new TimeSpan(0, 1, 0));
        public LogCountGuard NoExpectedUsersLogGuard { get; } = new LogCountGuard(new TimeSpan(0, 0, 10));
        public LogCountGuard GameDisconnectLogGuard { get; } = new LogCountGuard(new TimeSpan(0, 0, 0, 10));
        public LogCountGuard GameInvalidOpLogGuard { get; } = new LogCountGuard(new TimeSpan(0, 0, 0, 10));

        // from HiveHostGame
        public LogCountGuard RaiseEventExceptionLogGuard { get; } = new LogCountGuard(new TimeSpan(0, 0, 10));
        public LogCountGuard CreateGameCountGuard { get; } = new LogCountGuard(new TimeSpan(0, 0, 10));
        public LogCountGuard NotAllowedPropertiesUpdateLogGuard { get; } = new LogCountGuard(new TimeSpan(0, 0, 10));

        public LogCountGuard CustomHeaderExceptionLogGuard { get; } = new LogCountGuard(new TimeSpan(0, 1, 0));

        // from HivePeer
        public LogCountGuard UniqKeysValidationLogGuard { get; } = new LogCountGuard(new TimeSpan(0, 0, 10));
        public LogCountGuard ValidateOpCountGuard { get; } = new LogCountGuard(new TimeSpan(0, 0, 10));
        public LogCountGuard WebRpcLimitCountGuard { get; } = new LogCountGuard(new TimeSpan(0, 0, 10));
        public LogCountGuard OperationParametersLogGuard { get; } = new LogCountGuard(new TimeSpan(0, 0, 10));

        // from PeerBase
        public LogCountGuard UnexpectedDataLogGuard { get; } = new LogCountGuard(new TimeSpan(0, 0, 1));
        public LogCountGuard MetaDataViolationLogGuard { get; } = new LogCountGuard(new TimeSpan(0, 0, 1));
        public LogCountGuard ResponseSendErrorLogGuard { get; } = new LogCountGuard(new TimeSpan(0, 0, 10));
        public LogCountGuard EventSendErrorLogGuard { get; } = new LogCountGuard(new TimeSpan(0, 0, 10));
        public LogCountGuard MsgSendErrorLogGuard { get; } = new LogCountGuard(new TimeSpan(0, 0, 10));
        public LogCountGuard RawMsgSendErrorLogGuard { get; } = new LogCountGuard(new TimeSpan(0, 0, 10));
        public LogCountGuard DisconnectMsgSendErrorLogGuard { get; } = new LogCountGuard(new TimeSpan(0, 0, 10));
    }
}
