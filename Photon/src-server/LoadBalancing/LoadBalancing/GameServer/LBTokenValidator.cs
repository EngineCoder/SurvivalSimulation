using ExitGames.Logging;

using Photon.Common;
using Photon.Common.Authentication;
using Photon.Hive;
using Photon.Hive.Operations;
using Photon.LoadBalancing.Operations;
using Photon.SocketServer;
using Photon.SocketServer.Diagnostics;

using System;

namespace Photon.LoadBalancing.GameServer
{
    public class LBTokenValidator
    {
        protected static readonly ILogger log = LogManager.GetCurrentClassLogger();
        protected static readonly LogCountGuard tokenValidationLogGuard = new LogCountGuard(new TimeSpan(0, 0, 10));

        public virtual bool ValidateAuthToken(AuthenticateRequest authenticateRequest, SendParameters sendParameters,
            AuthenticationToken authToken, HivePeer peer, out ErrorCode errorCode, out string errorMsg)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug(
                    $"Token Validation: checking token with GS: {authToken.ExpectedGS} and GameId:{authToken.ExpectedGameId}");
            }

            if (GameServerSettings.Default.TokenCheckExpectedHostAndGame && authToken.ExpectedGS != GameServerSettings.Default.Master.PublicHostName)
            {
                log.Warn(tokenValidationLogGuard, "Token Validation: Expected GS is different from this one. " +
                                                  $"egs:'{authToken.ExpectedGS}', gs:{GameServerSettings.Default.Master.PublicHostName}, uid:{authToken.UserId}, " +
                                                  $"AppId:{authToken.ApplicationId}/{authToken.ApplicationVersion}, peer:{peer}");

                errorCode = ErrorCode.ExpectedGSCheckFailure;
                errorMsg = ErrorMessages.ExpectedGSCheckFailure;
                return false;
            }

            errorMsg = string.Empty;
            errorCode = ErrorCode.Ok;
            return true;
        }

        public virtual bool ValidateExpectedGame(JoinGameRequest joinRequest, HivePeer peer, out ErrorCode errorCode, out string errorMsg)
        {
            errorCode = ErrorCode.Ok;
            errorMsg = string.Empty;

            if (!GameServerSettings.Default.TokenCheckExpectedHostAndGame || peer.AuthToken == null || joinRequest.GameId == peer.AuthToken.ExpectedGameId)
            {
                return true;
            }

            log.Warn(tokenValidationLogGuard, "Token Validation: Expected game is different from requested game. " +
                          $"eg:'{peer.AuthToken.ExpectedGameId}', rg:{joinRequest.GameId}, uid:{peer.UserId}, peer:{peer}");

            errorCode = ErrorCode.ExpectedGameCheckFailure;
            errorMsg = HiveErrorMessages.ExpectedGameCheckFailure;
            return false;
        }
    }
}
