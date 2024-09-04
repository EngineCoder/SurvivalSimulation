namespace Photon.Hive.Plugin
{
    public static class GameParameters
    {
        /// <summary>
        /// Maximum number of actors allowed in the room.
        /// </summary>
        public const byte MaxPlayers = 255;
        /// <summary>
        /// Whether the room should be visible in the lobby (public, 'available' for 'random' matchmaking) or hidden (private, can be joined only via room name).
        /// </summary>
        public const byte IsVisible = 254;
        /// <summary>
        /// Whether the room should accept anymore actors.
        /// </summary>
        public const byte IsOpen = 253;
        //252 and 251 are reserved
        /// <summary>
        /// The string array of custom properties keys that should be exposed in the lobby for matchmaking.
        /// </summary>
        public const byte LobbyProperties = 250;
        /// <summary>
        /// The actor number of the master client.
        /// </summary>
        public const byte MasterClientId = 248;
        /// <summary>
        /// The string array of UserIDs for slots-reservation.
        /// </summary>
        public const byte ExpectedUsers = 247;
        /// <summary>
        /// Duration in milliseconds of how long an actor who left the room can remain inactive in the room's actors list (can rejoin later).
        /// int.MaxValue or negative values mean infinite time.
        /// </summary>
        public const byte PlayerTTL = 246;
        /// <summary>
        /// Duration in milliseconds of how long a room should remain in the server once empty (all actors left).
        /// The value is capped.
        /// </summary>
        public const byte EmptyRoomTTL = 245;
    }
}
