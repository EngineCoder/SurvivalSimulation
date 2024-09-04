using Photon.SocketServer.Diagnostics;

namespace Photon.Hive.Common
{
    public interface IHiveLogContext
    {
        LogCountGuard LobbyPropertiesValidatorLogGuard { get; }
        LogCountGuard ForceGameCloseLogGuard { get; }
        LogCountGuard ExceptionLogGuard { get; }
        LogCountGuard NoExpectedUsersLogGuard { get; }
        LogCountGuard GameDisconnectLogGuard { get; }
        LogCountGuard GameInvalidOpLogGuard { get; }

        LogCountGuard RaiseEventExceptionLogGuard { get; }
        LogCountGuard CreateGameCountGuard { get; }
        LogCountGuard NotAllowedPropertiesUpdateLogGuard { get; }

        LogCountGuard CustomHeaderExceptionLogGuard { get; }

    }
}
