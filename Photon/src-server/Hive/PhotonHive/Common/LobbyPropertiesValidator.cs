using ExitGames.Logging;

using Photon.Hive.Operations;
using Photon.Hive.Plugin;
using Photon.SocketServer.Rpc;
using Photon.SocketServer.Rpc.Protocols;

using System.Collections;

namespace Photon.Hive.Common
{
    public class LobbyPropertiesValidator
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();


        #region .publics

        public static bool ValidateLimits(SetPropertiesRequest request, 
            PropertyBag<object> gameProperties, 
            PropertiesLimitsSettings propertiesLimits,
            out string errorMsg)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug("checking lobby properties size for set properties request");
            }


            var lobbyFilterSet = gameProperties.TryGetValue((byte)GameParameter.LobbyProperties, out var currentLobbyFilter);

            if (!ValidateCount(propertiesLimits.MaxLobbyPropertiesCount, 
                    request,
                    gameProperties,
                    lobbyFilterSet,
                    out errorMsg))
            {
                return false;
            }

            if (request.OperationRequest?.RequestMetaData?.ParametersMetaData == null)
            {
                return true;
            }

            if (request.newLobbyProperties != null)
            {
                currentLobbyFilter = request.newLobbyProperties;
            }

            return ValidateSize(request, gameProperties, (object[])currentLobbyFilter, propertiesLimits, out errorMsg);
        }

        public static bool ValidateLimits(JoinGameRequest createGameRequest, PropertiesLimitsSettings propertiesLimits, out string errorMsg)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug("checking lobby properties size for create game request");
            }


            if (!ValidateCountOnCreate(propertiesLimits.MaxLobbyPropertiesCount,
                    createGameRequest,
                    out errorMsg))
            {
                return false;
            }

            if (createGameRequest.OperationRequest?.RequestMetaData?.ParametersMetaData == null)
            {
                return true;
            }

            return ValidateSizeOnCreate(createGameRequest, propertiesLimits, out errorMsg);
        }
        #endregion

        #region .methods

        private static bool ValidateCount(int countLimit, 
            SetPropertiesRequest request, PropertyBag<object> gameProperties, bool lobbyFilterSet, out string errorMsg)
        {
            var lobbyProperties = request.newLobbyProperties;

            if (lobbyProperties != null)
            {
                if (lobbyProperties.Length <= countLimit)
                {
                    errorMsg = string.Empty;
                    return true;
                }

                errorMsg = "too many lobby properties. " +
                           $"Limit is {countLimit}, request has {lobbyProperties.Length}";

                return false;
            }

            //that means that request did not update existing correct lobby properties filter
            if (lobbyFilterSet)
            {
                errorMsg = string.Empty;
                return true;
            }

            // we handle case when lobby properties filter is not set at all.
            // in this case we should treat all properties as lobby properties

            var propDic = gameProperties.AsDictionary();

            var totalPropsCount = propDic.Count;

            foreach (DictionaryEntry property in request.Properties)
            {
                if (!propDic.ContainsKey(property.Key))
                {
                    ++totalPropsCount;
                }
            }

            if (totalPropsCount > countLimit)
            {
                errorMsg = "too many lobby properties. " +
                           $"Limit is {countLimit}, request increases totalNumber of lobby properties to {totalPropsCount}";

                return false;
            }

            if (log.IsDebugEnabled)
            {
                log.Debug("lobby properties count check for set properties request passed");
            }

            errorMsg = string.Empty;
            return true;
        }

        private static bool ValidateSize(SetPropertiesRequest request,
            PropertyBag<object> gameProperties,
            object[] currentLobbyFilter,
            PropertiesLimitsSettings propertiesLimits,
            out string errorMsg)
        {
            if (request.newLobbyProperties != null)
            {
                currentLobbyFilter = request.newLobbyProperties;
            }

            var totalSize = gameProperties.TotalSize;
            var propertiesMetaData = GetParameterMetaData(request, (byte)ParameterKey.Properties);

            if (propertiesMetaData != null)
            {
                if (currentLobbyFilter == null)
                {
                    // we handle simple case when all properties are sent to lobby.
                    totalSize += propertiesMetaData.DataSize;

                    foreach (DictionaryEntry property in request.Properties)
                    {
                        if (gameProperties.AsDictionary().TryGetValue(property.Key, out var existingProperty))
                        {
                            totalSize -= existingProperty.TotalSize;
                        }
                    }
                }
                else
                {
                    totalSize = 0;
                    foreach (var entry in currentLobbyFilter)
                    {
                        if (propertiesMetaData.SubtypeMetaData.TryGetValue(entry, out var sizePair))
                        {
                            totalSize += sizePair.Key + sizePair.Value;
                        }
                        else if (gameProperties.AsDictionary().TryGetValue(entry, out var property))
                        {
                            totalSize += property.TotalSize;
                        }
                    }
                }

                if (totalSize > propertiesLimits.MaxLobbyPropertiesSize)
                {
                    errorMsg =
                        $"lobby properties size exceeds limit. limit:{propertiesLimits.MaxLobbyPropertiesSize}. actual size:{totalSize}";

                    return false;
                }
            }

            if (log.IsDebugEnabled)
            {
                log.Debug("lobby properties size check for set properties request passed");
            }

            errorMsg = string.Empty;
            return true;
        }

        private static bool ValidateCountOnCreate(int countLimit, JoinGameRequest request, out string errorMsg)
        {
            var lobbyProperties = request.wellKnownPropertiesCache.LobbyProperties;

            if (lobbyProperties != null)
            {
                if (lobbyProperties.Length <= countLimit)
                {
                    errorMsg = string.Empty;
                    return true;
                }

                errorMsg = "too many lobby properties. " +
                           $"Limit is {countLimit}, request has {lobbyProperties.Length}";

                return false;
            }

            var totalPropsCount = request.GameProperties?.Count ?? 0;

            if (totalPropsCount > countLimit)
            {
                errorMsg = "too many lobby properties." +
                           $"Limit is {countLimit}, request creates game with {totalPropsCount} lobby properties";

                return false;
            }

            if (log.IsDebugEnabled)
            {
                log.Debug("lobby properties count check for create game request passed");
            }
            errorMsg = string.Empty;
            return true;
        }

        private static bool ValidateSizeOnCreate(JoinGameRequest request,
            PropertiesLimitsSettings propertiesLimits,
            out string errorMsg)
        {
            var currentLobbyFilter = request.wellKnownPropertiesCache.LobbyProperties;

            var propertiesMetaData = GetParameterMetaData(request, (byte)ParameterKey.GameProperties);
            int totalSize = 0;

            if (propertiesMetaData != null)
            {
                if (currentLobbyFilter == null)
                {
                    // we handle simple case when all properties are sent to lobby.
                    totalSize += propertiesMetaData.DataSize;
                }
                else
                {
                    totalSize = 0;
                    foreach (var entry in currentLobbyFilter)
                    {
                        if (propertiesMetaData.SubtypeMetaData.TryGetValue(entry, out var sizePair))
                        {
                            totalSize += sizePair.Key + sizePair.Value;
                        }
                    }
                }

                if (totalSize > propertiesLimits.MaxLobbyPropertiesSize)
                {
                    errorMsg =
                        $"lobby properties size exceeds limit on game creation. limit:{propertiesLimits.MaxLobbyPropertiesSize}. actual size:{totalSize}";

                    //log.WarnFormat(logContext.LobbyPropertiesValidatorLogGuard,
                    //    "Limit exceeded: lobby properties size exceeds limit on game creation. limit:{0}. actual size:{1}",
                    //    propertiesLimits.MaxLobbyPropertiesSize, totalSize);
                    return false;
                }
            }

            if (log.IsDebugEnabled)
            {
                log.Debug("lobby properties size check for create game request passed");
            }
            errorMsg = string.Empty;
            return true;
        }

        private static ParameterMetaData GetParameterMetaData(Operation request, byte paramId)
        {
            var mdc = request.OperationRequest.RequestMetaData.ParametersMetaData.Count;
            for (var i = 0; i < mdc; ++i)
            {
                var md = request.OperationRequest.RequestMetaData.ParametersMetaData[i];
                if (md.Key == paramId)
                {
                    return md.Value;
                }
            }
            return null;
        }

        #endregion
    }
}