namespace Photon.Hive.Common
{
    public class PropertiesLimitsSettings
    {
        /// <summary>
        /// Limits amount of uniq properties that can be set by one peer. well known are included
        /// </summary>
        public int MaxUniqPropertyKeysPerPeer { get; set; } = 1000;

        /// <summary>
        /// Limits total size of properties in game
        /// </summary>
        public int MaxPropertiesSizePerGame { get; set; } = 51000;

        /// <summary>
        /// Limit number of properties in one request
        /// </summary>
        public int MaxPropertiesPerRequest { get; set; } = 100;

        /// <summary>
        /// limit for size of all properties in one request
        /// </summary>
        public int MaxPropertiesSizePerRequest { get; set; } = 10000;

        /// <summary>
        /// maximum amount of lobby properties per game
        /// </summary>
        public int MaxLobbyPropertiesCount { get; set; } = 15;

        /// <summary>
        /// maximum size of lobby properties per game. both key and value are counted
        /// </summary>
        public int MaxLobbyPropertiesSize { get; set; } = 400;

    }
}
