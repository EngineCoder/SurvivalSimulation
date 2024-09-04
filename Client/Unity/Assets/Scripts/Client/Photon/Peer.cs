
namespace UFO.Realtime
{
    using ExitGames.Client.Photon;
    using System;
    using System.Collections.Generic;

    public class Peer : PhotonPeer
    {
        #region Enum

        /// <summary>
        /// Options for authentication modes. From "classic" auth on each server to AuthOnce (on NameServer).
        /// </summary>
        public enum AuthModeOption
        {
            Auth,
            AuthOnce,
            AuthOnceWss
        }

        /// <summary>
        /// Options for optional "Custom Authentication" services used with Photon. Used by OpAuthenticate after connecting to Photon.
        /// </summary>
        public enum CustomAuthenticationType : byte
        {
            /// <summary>Use a custom authentification service. Currently the only implemented option.</summary>
            Custom = 0,

            /// <summary>Authenticates users by their Steam Account. Set auth values accordingly!</summary>
            Steam = 1,

            /// <summary>Authenticates users by their Facebook Account. Set auth values accordingly!</summary>
            Facebook = 2,

            /// <summary>Authenticates users by their Oculus Account and token.</summary>
            Oculus = 3,

            /// <summary>Authenticates users by their PSN Account and token.</summary>
            PlayStation = 4,

            /// <summary>Authenticates users by their Xbox Account and XSTS token.</summary>
            Xbox = 5,

            /// <summary>Authenticates users by their HTC Viveport Account and user token. Set AuthGetParameters to "userToken=[userToken]"</summary>
            Viveport = 10,

            /// <summary>Authenticates users by their NSA ID.</summary>
            NintendoSwitch = 11,

            /// <summary>Disables custom authentification. Same as not providing any AuthenticationValues for connect (more precisely for: OpAuthenticate).</summary>
            None = byte.MaxValue
        }

        /// <summary>Types of lobbies define their behaviour and capabilities. Check each value for details.</summary>
        /// <remarks>Values of this enum must be matched by the server.</remarks>
        public enum LobbyType : byte
        {
            /// <summary>Standard type and behaviour: While joined to this lobby clients get room-lists and JoinRandomRoom can use a simple filter to match properties (perfectly).</summary>
            Default = 0,
            /// <summary>This lobby type lists rooms like Default but JoinRandom has a parameter for SQL-like "where" clauses for filtering. This allows bigger, less, or and and combinations.</summary>
            SqlLobby = 2,
            /// <summary>This lobby does not send lists of games. It is only used for OpJoinRandomRoom. It keeps rooms available for a while when there are only inactive users left.</summary>
            AsyncRandomLobby = 3
        }
        #endregion


        #region Construction Method

        public Peer(ConnectionProtocol protocolType) : base(protocolType)
        {

        }

        public Peer(IPhotonPeerListener listener, ConnectionProtocol protocolType) : base(listener, protocolType)
        {
            this.Listener = Listener;
        }

        #endregion


        #region Protected Method

        /// <summary>
        /// Sets properties of a room.
        /// Internally this uses OpSetProperties, which can be used to either set room or player properties.
        /// </summary>
        /// <param name="gameProperties">The properties to add or update.</param>
        /// <param name="expectedProperties">The properties expected when update occurs. (CAS : "Check And Swap")</param>
        /// <param name="webflags">WebFlag to indicate if request should be forwarded as "PathProperties" webhook or not.</param>
        /// <returns>If the operation could be sent (has to be connected).</returns>
        protected internal bool OpSetPropertiesOfRoom(Hashtable gameProperties, Hashtable expectedProperties = null, WebFlags webflags = null)
        {
            if (this.DebugOut >= DebugLevel.INFO)
            {
                this.Listener.DebugReturn(DebugLevel.INFO, "OpSetPropertiesOfRoom()");
            }
            if (gameProperties == null || gameProperties.Count == 0)
            {
                if (this.DebugOut >= DebugLevel.INFO)
                {
                    this.Listener.DebugReturn(DebugLevel.INFO, "OpSetPropertiesOfRoom not sent. gameProperties must be not null nor empty.");
                }
                return false;
            }

            Dictionary<byte, object> opParameters = new Dictionary<byte, object>();
            opParameters.Add(ParameterCode.Properties, gameProperties);
            opParameters.Add(ParameterCode.Broadcast, true);
            if (expectedProperties != null && expectedProperties.Count != 0)
            {
                opParameters.Add(ParameterCode.ExpectedValues, expectedProperties);
            }

            if (webflags != null && webflags.HttpForward)
            {
                opParameters[ParameterCode.EventForward] = webflags.WebhookFlags;
            }

            return this.SendOperation(OperationCode.SetProperties, opParameters, SendOptions.SendReliable);
        }
        /// <summary>
        /// Sets properties of a player / actor.
        /// Internally this uses OpSetProperties, which can be used to either set room or player properties.
        /// </summary>
        /// <param name="actorNr">The payer ID (a.k.a. actorNumber) of the player to attach these properties to.</param>
        /// <param name="actorProperties">The properties to add or update.</param>
        /// <param name="expectedProperties">If set, these must be in the current properties-set (on the server) to set actorProperties: CAS.</param>
        /// <param name="webflags">Set these to forward the properties to a WebHook as defined for this app (in Dashboard).</param>
        /// <returns>If the operation could be sent (requires connection).</returns>
        protected internal bool OpSetPropertiesOfActor(int actorNr, Hashtable actorProperties, Hashtable expectedProperties = null, WebFlags webflags = null)
        {
            if (this.DebugOut >= DebugLevel.INFO)
            {
                this.Listener.DebugReturn(DebugLevel.INFO, "OpSetPropertiesOfActor()");
            }

            if (actorNr <= 0 || actorProperties == null || actorProperties.Count == 0)
            {
                if (this.DebugOut >= DebugLevel.INFO)
                {
                    this.Listener.DebugReturn(DebugLevel.INFO, "OpSetPropertiesOfActor not sent. ActorNr must be > 0 and actorProperties must be not null nor empty.");
                }
                return false;
            }

            Dictionary<byte, object> opParameters = new Dictionary<byte, object>();
            opParameters.Add(ParameterCode.Properties, actorProperties);
            opParameters.Add(ParameterCode.ActorNr, actorNr);
            opParameters.Add(ParameterCode.Broadcast, true);
            if (expectedProperties != null && expectedProperties.Count != 0)
            {
                opParameters.Add(ParameterCode.ExpectedValues, expectedProperties);
            }

            if (webflags != null && webflags.HttpForward)
            {
                opParameters[ParameterCode.EventForward] = webflags.WebhookFlags;
            }

            return this.SendOperation(OperationCode.SetProperties, opParameters, SendOptions.SendReliable);
        }

        #endregion


        #region  Public Method

        #endregion


        /// <summary>
        /// Container for user authentication in Photon. Set AuthValues before you connect - all else is handled.
        /// </summary>
        /// <remarks>
        /// On Photon, user authentication is optional but can be useful in many cases.
        /// If you want to FindFriends, a unique ID per user is very practical.
        ///
        /// There are basically three options for user authentication: None at all, the client sets some UserId
        /// or you can use some account web-service to authenticate a user (and set the UserId server-side).
        ///
        /// Custom Authentication lets you verify end-users by some kind of login or token. It sends those
        /// values to Photon which will verify them before granting access or disconnecting the client.
        ///
        /// The AuthValues are sent in OpAuthenticate when you connect, so they must be set before you connect.
        /// If the AuthValues.UserId is null or empty when it's sent to the server, then the Photon Server assigns a UserId!
        ///
        /// The Photon Cloud Dashboard will let you enable this feature and set important server values for it.
        /// https://dashboard.photonengine.com
        /// </remarks>
        public class AuthenticationValues
        {
            /// <summary>See AuthType.</summary>
            private CustomAuthenticationType authType = CustomAuthenticationType.None;

            /// <summary>The type of authentication provider that should be used. Defaults to None (no auth whatsoever).</summary>
            /// <remarks>Several auth providers are available and CustomAuthenticationType.Custom can be used if you build your own service.</remarks>
            public CustomAuthenticationType AuthType
            {
                get { return authType; }
                set { authType = value; }
            }

            /// <summary>This string must contain any (http get) parameters expected by the used authentication service. By default, username and token.</summary>
            /// <remarks>
            /// Maps to operation parameter 216.
            /// Standard http get parameters are used here and passed on to the service that's defined in the server (Photon Cloud Dashboard).
            /// </remarks>
            public string AuthGetParameters { get; set; }

            /// <summary>Data to be passed-on to the auth service via POST. Default: null (not sent). Either string or byte[] (see setters).</summary>
            /// <remarks>Maps to operation parameter 214.</remarks>
            public object AuthPostData { get; private set; }

            /// <summary>Internal <b>Photon token</b>. After initial authentication, Photon provides a token for this client, subsequently used as (cached) validation.</summary>
            /// <remarks>Any token for custom authentication should be set via SetAuthPostData or AddAuthParameter.</remarks>
            public string Token { get; protected internal set; }

            /// <summary>The UserId should be a unique identifier per user. This is for finding friends, etc..</summary>
            /// <remarks>See remarks of AuthValues for info about how this is set and used.</remarks>
            public string UserId { get; set; }


            /// <summary>Creates empty auth values without any info.</summary>
            public AuthenticationValues()
            {
            }

            /// <summary>Creates minimal info about the user. If this is authenticated or not, depends on the set AuthType.</summary>
            /// <param name="userId">Some UserId to set in Photon.</param>
            public AuthenticationValues(string userId)
            {
                this.UserId = userId;
            }

            /// <summary>Sets the data to be passed-on to the auth service via POST.</summary>
            /// <remarks>AuthPostData is just one value. Each SetAuthPostData replaces any previous value. It can be either a string, a byte[] or a dictionary.</remarks>
            /// <param name="stringData">String data to be used in the body of the POST request. Null or empty string will set AuthPostData to null.</param>
            public virtual void SetAuthPostData(string stringData)
            {
                this.AuthPostData = (string.IsNullOrEmpty(stringData)) ? null : stringData;
            }

            /// <summary>Sets the data to be passed-on to the auth service via POST.</summary>
            /// <remarks>AuthPostData is just one value. Each SetAuthPostData replaces any previous value. It can be either a string, a byte[] or a dictionary.</remarks>
            /// <param name="byteData">Binary token / auth-data to pass on.</param>
            public virtual void SetAuthPostData(byte[] byteData)
            {
                this.AuthPostData = byteData;
            }

            /// <summary>Sets data to be passed-on to the auth service as Json (Content-Type: "application/json") via Post.</summary>
            /// <remarks>AuthPostData is just one value. Each SetAuthPostData replaces any previous value. It can be either a string, a byte[] or a dictionary.</remarks>
            /// <param name="dictData">A authentication-data dictionary will be converted to Json and passed to the Auth webservice via HTTP Post.</param>
            public virtual void SetAuthPostData(Dictionary<string, object> dictData)
            {
                this.AuthPostData = dictData;
            }

            /// <summary>Adds a key-value pair to the get-parameters used for Custom Auth (AuthGetParameters).</summary>
            /// <remarks>This method does uri-encoding for you.</remarks>
            /// <param name="key">Key for the value to set.</param>
            /// <param name="value">Some value relevant for Custom Authentication.</param>
            public virtual void AddAuthParameter(string key, string value)
            {
                string ampersand = string.IsNullOrEmpty(this.AuthGetParameters) ? "" : "&";
                this.AuthGetParameters = string.Format("{0}{1}{2}={3}", this.AuthGetParameters, ampersand, System.Uri.EscapeDataString(key), System.Uri.EscapeDataString(value));
            }

            /// <summary>
            /// Transform this object into string.
            /// </summary>
            /// <returns>string representation of this object.</returns>
            public override string ToString()
            {
                return string.Format("AuthenticationValues Type: {3} UserId: {0}, GetParameters: {1} Token available: {2}", this.UserId, this.AuthGetParameters, !string.IsNullOrEmpty(this.Token), this.AuthType);
            }
        }


        /// <summary>Refers to a specific lobby on the server.</summary>
        /// <remarks>
        /// Name and Type combined are the unique identifier for a lobby.<br/>
        /// The server will create lobbies "on demand", so no registration or setup is required.<br/>
        /// An empty or null Name always points to the "default lobby" as special case.
        /// </remarks>
        public class TypedLobby
        {
            /// <summary>
            /// Name of the lobby. Default: null, pointing to the "default lobby".
            /// </summary>
            /// <remarks>
            /// If Name is null or empty, a TypedLobby will point to the "default lobby". This ignores the Type value and always acts as  <see cref="LobbyType.Default"/>.
            /// </remarks>
            public string Name;

            /// <summary>
            /// Type (and behaviour) of the lobby.
            /// </summary>
            /// <remarks>
            /// An empty or null Name always points to the "default lobby" as special case.
            /// </remarks>
            public LobbyType Type;

            /// <summary>
            /// A reference to the default lobby which is the unique lobby that uses null as name and is of type <see cref="LobbyType.Default"/>.
            /// </summary>
            /// <remarks>
            /// There is only a single lobby with an empty name on the server. It is always of type  <see cref="LobbyType.Default"/>.<br/>
            /// On the other hand, this is a shortcut and reusable reference to the default lobby.<br/>
            /// Do not change Name or Type.<br/>
            /// </remarks>
            public static readonly TypedLobby Default = new TypedLobby();

            /// <summary>
            /// Returns whether or not this instance points to the "default lobby" (<see cref="TypedLobby.Default"/>).
            /// </summary>
            /// <remarks>
            /// This comes up to checking if the Name is null or empty.
            /// <see cref="LobbyType.Default"/> is not the same thing as the "default lobby" (<see cref="TypedLobby.Default"/>).
            /// </remarks>
            public bool IsDefault { get { return string.IsNullOrEmpty(this.Name); } }


            /// <summary>
            /// Creates a TypedLobby instance. Unless Name is changed, this points to the "default lobby" (<see cref="TypedLobby.Default"/>).
            /// </summary>
            internal TypedLobby()
            {
            }

            /// <summary>
            /// Sets Name and Type of the new instance. Make sure name is not empty or null, as that always points to the "default lobby" (<see cref="TypedLobby.Default"/>).
            /// </summary>
            /// <param name="name">Some string to identify a lobby.</param>
            /// <param name="type">The type of a lobby defines it's capabilities and behaviour.</param>
            public TypedLobby(string name, LobbyType type)
            {
                this.Name = name;
                this.Type = type;
            }

            public override string ToString()
            {
                return string.Format("lobby '{0}'[{1}]", this.Name, this.Type);
            }
        }

        /// <summary>
        /// Info for a lobby on the server. Used when <see cref="LoadBalancingClient.EnableLobbyStatistics"/> is true.
        /// </summary>
        public class TypedLobbyInfo : TypedLobby
        {
            /// <summary>Count of players that currently joined this lobby.</summary>
            public int PlayerCount;

            /// <summary>Count of rooms currently associated with this lobby.</summary>
            public int RoomCount;

            public override string ToString()
            {
                return string.Format("TypedLobbyInfo '{0}'[{1}] rooms: {2} players: {3}", this.Name, this.Type, this.RoomCount, this.PlayerCount);
            }
        }

        /// <summary>
        /// Class for constants. These (byte) values are for "well known" room/game properties used in Photon LoadBalancing.
        /// </summary>
        /// <remarks>
        /// These constants are used internally.
        /// "Custom properties" have to use a string-type as key. They can be assigned at will.
        /// </remarks>
        public class GamePropertyKey
        {
            /// <summary>(255) Max number of players that "fit" into this room. 0 is for "unlimited".</summary>
            public const byte MaxPlayers = 255;

            /// <summary>(254) Makes this room listed or not in the lobby on master.</summary>
            public const byte IsVisible = 254;

            /// <summary>(253) Allows more players to join a room (or not).</summary>
            public const byte IsOpen = 253;

            /// <summary>(252) Current count of players in the room. Used only in the lobby on master.</summary>
            public const byte PlayerCount = 252;

            /// <summary>(251) True if the room is to be removed from room listing (used in update to room list in lobby on master)</summary>
            public const byte Removed = 251;

            /// <summary>(250) A list of the room properties to pass to the RoomInfo list in a lobby. This is used in CreateRoom, which defines this list once per room.</summary>
            public const byte PropsListedInLobby = 250;

            /// <summary>(249) Equivalent of Operation Join parameter CleanupCacheOnLeave.</summary>
            public const byte CleanupCacheOnLeave = 249;

            /// <summary>(248) Code for MasterClientId, which is synced by server. When sent as op-parameter this is (byte)203. As room property this is (byte)248.</summary>
            /// <remarks>Tightly related to ParameterCode.MasterClientId.</remarks>
            public const byte MasterClientId = (byte)248;

            /// <summary>(247) Code for ExpectedUsers in a room. Matchmaking keeps a slot open for the players with these userIDs.</summary>
            public const byte ExpectedUsers = (byte)247;

            /// <summary>(246) Player Time To Live. How long any player can be inactive (due to disconnect or leave) before the user gets removed from the playerlist (freeing a slot).</summary>
            public const byte PlayerTtl = (byte)246;

            /// <summary>(245) Room Time To Live. How long a room stays available (and in server-memory), after the last player becomes inactive. After this time, the room gets persisted or destroyed.</summary>
            public const byte EmptyRoomTtl = (byte)245;
        }

        /// <summary>Wraps up common room properties needed when you create rooms. Read the individual entries for more details.</summary>
        /// <remarks>This directly maps to the fields in the Room class.</remarks>
        public class RoomOptions
        {
            /// <summary>Defines if this room is listed in the lobby. If not, it also is not joined randomly.</summary>
            /// <remarks>
            /// A room that is not visible will be excluded from the room lists that are sent to the clients in lobbies.
            /// An invisible room can be joined by name but is excluded from random matchmaking.
            ///
            /// Use this to "hide" a room and simulate "private rooms". Players can exchange a roomname and create it
            /// invisble to avoid anyone else joining it.
            /// </remarks>
            public bool IsVisible { get { return this.isVisible; } set { this.isVisible = value; } }
            private bool isVisible = true;

            /// <summary>Defines if this room can be joined at all.</summary>
            /// <remarks>
            /// If a room is closed, no player can join this. As example this makes sense when 3 of 4 possible players
            /// start their gameplay early and don't want anyone to join during the game.
            /// The room can still be listed in the lobby (set isVisible to control lobby-visibility).
            /// </remarks>
            public bool IsOpen { get { return this.isOpen; } set { this.isOpen = value; } }
            private bool isOpen = true;

            /// <summary>Max number of players that can be in the room at any time. 0 means "no limit".</summary>
            public byte MaxPlayers;

            /// <summary>Time To Live (TTL) for an 'actor' in a room. If a client disconnects, this actor is inactive first and removed after this timeout. In milliseconds.</summary>
            public int PlayerTtl;

            /// <summary>Time To Live (TTL) for a room when the last player leaves. Keeps room in memory for case a player re-joins soon. In milliseconds.</summary>
            public int EmptyRoomTtl;

            /// <summary>Removes a user's events and properties from the room when a user leaves.</summary>
            /// <remarks>
            /// This makes sense when in rooms where players can't place items in the room and just vanish entirely.
            /// When you disable this, the event history can become too long to load if the room stays in use indefinitely.
            /// Default: true. Cleans up the cache and props of leaving users.
            /// </remarks>
            public bool CleanupCacheOnLeave { get { return this.cleanupCacheOnLeave; } set { this.cleanupCacheOnLeave = value; } }
            private bool cleanupCacheOnLeave = true;

            /// <summary>The room's custom properties to set. Use string keys!</summary>
            /// <remarks>
            /// Custom room properties are any key-values you need to define the game's setup.
            /// The shorter your keys are, the better.
            /// Example: Map, Mode (could be "m" when used with "Map"), TileSet (could be "t").
            /// </remarks>
            public Hashtable CustomRoomProperties;

            /// <summary>Defines the custom room properties that get listed in the lobby.</summary>
            /// <remarks>
            /// Name the custom room properties that should be available to clients that are in a lobby.
            /// Use with care. Unless a custom property is essential for matchmaking or user info, it should
            /// not be sent to the lobby, which causes traffic and delays for clients in the lobby.
            ///
            /// Default: No custom properties are sent to the lobby.
            /// </remarks>
            public string[] CustomRoomPropertiesForLobby = new string[0];

            /// <summary>Informs the server of the expected plugin setup.</summary>
            /// <remarks>
            /// The operation will fail in case of a plugin missmatch returning error code PluginMismatch 32757(0x7FFF - 10).
            /// Setting string[]{} means the client expects no plugin to be setup.
            /// Note: for backwards compatibility null omits any check.
            /// </remarks>
            public string[] Plugins;

            /// <summary>
            /// Tells the server to skip room events for joining and leaving players.
            /// </summary>
            /// <remarks>
            /// Using this makes the client unaware of the other players in a room.
            /// That can save some traffic if you have some server logic that updates players
            /// but it can also limit the client's usability.
            /// </remarks>
            public bool SuppressRoomEvents { get; set; }

            /// <summary>
            /// Defines if the UserIds of players get "published" in the room. Useful for FindFriends, if players want to play another game together.
            /// </summary>
            /// <remarks>
            /// When you set this to true, Photon will publish the UserIds of the players in that room.
            /// In that case, you can use PhotonPlayer.userId, to access any player's userID.
            /// This is useful for FindFriends and to set "expected users" to reserve slots in a room.
            /// </remarks>
            public bool PublishUserId { get; set; }

            /// <summary>Optionally, properties get deleted, when null gets assigned as value. Defaults to off / false.</summary>
            /// <remarks>
            /// When Op SetProperties is setting a key's value to null, the server and clients should remove the key/value from the Custom Properties.
            /// By default, the server keeps the keys (and null values) and sends them to joining players.
            ///
            /// Important: Only when SetProperties does a "broadcast", the change (key, value = null) is sent to clients to update accordingly.
            /// This applies to Custom Properties for rooms and actors/players.
            /// </remarks>
            public bool DeleteNullProperties { get; set; }

            /// <summary>By default, property changes are sent back to the client that's setting them to avoid de-sync when properties are set concurrently.</summary>
            /// <remarks>
            /// This option is enables by default to fix this scenario:
            ///
            /// 1) On server, room property ABC is set to value FOO, which triggers notifications to all the clients telling them that the property changed.
            /// 2) While that notification is in flight, a client sets the ABC property to value BAR.
            /// 3) Client receives notification from the server and changes it�s local copy of ABC to FOO.
            /// 4) Server receives the set operation and changes the official value of ABC to BAR, but never notifies the client that sent the set operation that the value is now BAR.
            ///
            /// Without this option, the client that set the value to BAR never hears from the server that the official copy has been updated to BAR, and thus gets stuck with a value of FOO.
            /// </remarks>
            public bool BroadcastPropsChangeToAll { get { return this.broadcastPropsChangeToAll; } set { this.broadcastPropsChangeToAll = value; } }
            private bool broadcastPropsChangeToAll = true;

#if SERVERSDK
        public bool CheckUserOnJoin { get; set; }
#endif
        }

        /// <summary>Class for constants. Codes for parameters of Operations and Events.</summary>
        /// <remarks>These constants are used internally.</remarks>
        public class ParameterCode
        {
            /// <summary>(237) A bool parameter for creating games. If set to true, no room events are sent to the clients on join and leave. Default: false (and not sent).</summary>
            public const byte SuppressRoomEvents = 237;

            /// <summary>(236) Time To Live (TTL) for a room when the last player leaves. Keeps room in memory for case a player re-joins soon. In milliseconds.</summary>
            public const byte EmptyRoomTTL = 236;

            /// <summary>(235) Time To Live (TTL) for an 'actor' in a room. If a client disconnects, this actor is inactive first and removed after this timeout. In milliseconds.</summary>
            public const byte PlayerTTL = 235;

            /// <summary>(234) Optional parameter of OpRaiseEvent and OpSetCustomProperties to forward the event/operation to a web-service.</summary>
            public const byte EventForward = 234;

            /// <summary>(233) Optional parameter of OpLeave in async games. If false, the player does abandons the game (forever). By default players become inactive and can re-join.</summary>
            [Obsolete("Use: IsInactive")]
            public const byte IsComingBack = (byte)233;

            /// <summary>(233) Used in EvLeave to describe if a user is inactive (and might come back) or not. In rooms with PlayerTTL, becoming inactive is the default case.</summary>
            public const byte IsInactive = (byte)233;

            /// <summary>(232) Used when creating rooms to define if any userid can join the room only once.</summary>
            public const byte CheckUserOnJoin = (byte)232;

            /// <summary>(231) Code for "Check And Swap" (CAS) when changing properties.</summary>
            public const byte ExpectedValues = (byte)231;

            /// <summary>(230) Address of a (game) server to use.</summary>
            public const byte Address = 230;

            /// <summary>(229) Count of players in this application in a rooms (used in stats event)</summary>
            public const byte PeerCount = 229;

            /// <summary>(228) Count of games in this application (used in stats event)</summary>
            public const byte GameCount = 228;

            /// <summary>(227) Count of players on the master server (in this app, looking for rooms)</summary>
            public const byte MasterPeerCount = 227;

            /// <summary>(225) User's ID</summary>
            public const byte UserId = 225;

            /// <summary>(224) Your application's ID: a name on your own Photon or a GUID on the Photon Cloud</summary>
            public const byte ApplicationId = 224;

            /// <summary>(223) Not used currently (as "Position"). If you get queued before connect, this is your position</summary>
            public const byte Position = 223;

            /// <summary>(223) Modifies the matchmaking algorithm used for OpJoinRandom. Allowed parameter values are defined in enum MatchmakingMode.</summary>
            public const byte MatchMakingType = 223;

            /// <summary>(222) List of RoomInfos about open / listed rooms</summary>
            public const byte GameList = 222;

            /// <summary>(221) Internally used to establish encryption</summary>
            public const byte Secret = 221;

            /// <summary>(220) Version of your application</summary>
            public const byte AppVersion = 220;

            /// <summary>(210) Internally used in case of hosting by Azure</summary>
            [Obsolete("TCP routing was removed after becoming obsolete.")]
            public const byte AzureNodeInfo = 210;  // only used within events, so use: EventCode.AzureNodeInfo

            /// <summary>(209) Internally used in case of hosting by Azure</summary>
            [Obsolete("TCP routing was removed after becoming obsolete.")]
            public const byte AzureLocalNodeId = 209;

            /// <summary>(208) Internally used in case of hosting by Azure</summary>
            [Obsolete("TCP routing was removed after becoming obsolete.")]
            public const byte AzureMasterNodeId = 208;

            /// <summary>(255) Code for the gameId/roomName (a unique name per room). Used in OpJoin and similar.</summary>
            public const byte RoomName = (byte)255;

            /// <summary>(250) Code for broadcast parameter of OpSetProperties method.</summary>
            public const byte Broadcast = (byte)250;

            /// <summary>(252) Code for list of players in a room.</summary>
            public const byte ActorList = (byte)252;

            /// <summary>(254) Code of the Actor of an operation. Used for property get and set.</summary>
            public const byte ActorNr = (byte)254;

            /// <summary>(249) Code for property set (Hashtable).</summary>
            public const byte PlayerProperties = (byte)249;

            /// <summary>(245) Code of data/custom content of an event. Used in OpRaiseEvent.</summary>
            public const byte CustomEventContent = (byte)245;

            /// <summary>(245) Code of data of an event. Used in OpRaiseEvent.</summary>
            public const byte Data = (byte)245;

            /// <summary>(244) Code used when sending some code-related parameter, like OpRaiseEvent's event-code.</summary>
            /// <remarks>This is not the same as the Operation's code, which is no longer sent as part of the parameter Dictionary in Photon 3.</remarks>
            public const byte Code = (byte)244;

            /// <summary>(248) Code for property set (Hashtable).</summary>
            public const byte GameProperties = (byte)248;

            /// <summary>
            /// (251) Code for property-set (Hashtable). This key is used when sending only one set of properties.
            /// If either ActorProperties or GameProperties are used (or both), check those keys.
            /// </summary>
            public const byte Properties = (byte)251;

            /// <summary>(253) Code of the target Actor of an operation. Used for property set. Is 0 for game</summary>
            public const byte TargetActorNr = (byte)253;

            /// <summary>(246) Code to select the receivers of events (used in Lite, Operation RaiseEvent).</summary>
            public const byte ReceiverGroup = (byte)246;

            /// <summary>(247) Code for caching events while raising them.</summary>
            public const byte Cache = (byte)247;

            /// <summary>(241) Bool parameter of CreateGame Operation. If true, server cleans up roomcache of leaving players (their cached events get removed).</summary>
            public const byte CleanupCacheOnLeave = (byte)241;

            /// <summary>(240) Code for "group" operation-parameter (as used in Op RaiseEvent).</summary>
            public const byte Group = 240;

            /// <summary>(239) The "Remove" operation-parameter can be used to remove something from a list. E.g. remove groups from player's interest groups.</summary>
            public const byte Remove = 239;

            /// <summary>(239) Used in Op Join to define if UserIds of the players are broadcast in the room. Useful for FindFriends and reserving slots for expected users.</summary>
            public const byte PublishUserId = 239;

            /// <summary>(238) The "Add" operation-parameter can be used to add something to some list or set. E.g. add groups to player's interest groups.</summary>
            public const byte Add = 238;

            /// <summary>(218) Content for EventCode.ErrorInfo and internal debug operations.</summary>
            public const byte Info = 218;

            /// <summary>(217) This key's (byte) value defines the target custom authentication type/service the client connects with. Used in OpAuthenticate</summary>
            public const byte ClientAuthenticationType = 217;

            /// <summary>(216) This key's (string) value provides parameters sent to the custom authentication type/service the client connects with. Used in OpAuthenticate</summary>
            public const byte ClientAuthenticationParams = 216;

            /// <summary>(215) Makes the server create a room if it doesn't exist. OpJoin uses this to always enter a room, unless it exists and is full/closed.</summary>
            // public const byte CreateIfNotExists = 215;

            /// <summary>(215) The JoinMode enum defines which variant of joining a room will be executed: Join only if available, create if not exists or re-join.</summary>
            /// <remarks>Replaces CreateIfNotExists which was only a bool-value.</remarks>
            public const byte JoinMode = 215;

            /// <summary>(214) This key's (string or byte[]) value provides parameters sent to the custom authentication service setup in Photon Dashboard. Used in OpAuthenticate</summary>
            public const byte ClientAuthenticationData = 214;

            /// <summary>(203) Code for MasterClientId, which is synced by server. When sent as op-parameter this is code 203.</summary>
            /// <remarks>Tightly related to GamePropertyKey.MasterClientId.</remarks>
            public const byte MasterClientId = (byte)203;

            /// <summary>(1) Used in Op FindFriends request. Value must be string[] of friends to look up.</summary>
            public const byte FindFriendsRequestList = (byte)1;

            /// <summary>(2) Used in Op FindFriends request. An integer containing option-flags to filter the results.</summary>
            public const byte FindFriendsOptions = (byte)2;

            /// <summary>(1) Used in Op FindFriends response. Contains bool[] list of online states (false if not online).</summary>
            public const byte FindFriendsResponseOnlineList = (byte)1;

            /// <summary>(2) Used in Op FindFriends response. Contains string[] of room names ("" where not known or no room joined).</summary>
            public const byte FindFriendsResponseRoomIdList = (byte)2;

            /// <summary>(213) Used in matchmaking-related methods and when creating a room to name a lobby (to join or to attach a room to).</summary>
            public const byte LobbyName = (byte)213;

            /// <summary>(212) Used in matchmaking-related methods and when creating a room to define the type of a lobby. Combined with the lobby name this identifies the lobby.</summary>
            public const byte LobbyType = (byte)212;

            /// <summary>(211) This (optional) parameter can be sent in Op Authenticate to turn on Lobby Stats (info about lobby names and their user- and game-counts).</summary>
            public const byte LobbyStats = (byte)211;

            /// <summary>(210) Used for region values in OpAuth and OpGetRegions.</summary>
            public const byte Region = (byte)210;

            /// <summary>(209) Path of the WebRPC that got called. Also known as "WebRpc Name". Type: string.</summary>
            public const byte UriPath = 209;

            /// <summary>(208) Parameters for a WebRPC as: Dictionary&lt;string, object&gt;. This will get serialized to JSon.</summary>
            public const byte WebRpcParameters = 208;

            /// <summary>(207) ReturnCode for the WebRPC, as sent by the web service (not by Photon, which uses ErrorCode). Type: byte.</summary>
            public const byte WebRpcReturnCode = 207;

            /// <summary>(206) Message returned by WebRPC server. Analog to Photon's debug message. Type: string.</summary>
            public const byte WebRpcReturnMessage = 206;

            /// <summary>(205) Used to define a "slice" for cached events. Slices can easily be removed from cache. Type: int.</summary>
            public const byte CacheSliceIndex = 205;

            /// <summary>(204) Informs the server of the expected plugin setup.</summary>
            /// <remarks>
            /// The operation will fail in case of a plugin mismatch returning error code PluginMismatch 32751(0x7FFF - 16).
            /// Setting string[]{} means the client expects no plugin to be setup.
            /// Note: for backwards compatibility null omits any check.
            /// </remarks>
            public const byte Plugins = 204;

            /// <summary>(202) Used by the server in Operation Responses, when it sends the nickname of the client (the user's nickname).</summary>
            public const byte NickName = 202;

            /// <summary>(201) Informs user about name of plugin load to game</summary>
            public const byte PluginName = 201;

            /// <summary>(200) Informs user about version of plugin load to game</summary>
            public const byte PluginVersion = 200;

            /// <summary>(196) Cluster info provided in OpAuthenticate/OpAuthenticateOnce responses.</summary>
            public const byte Cluster = 196;

            /// <summary>(195) Protocol which will be used by client to connect master/game servers. Used for nameserver.</summary>
            public const byte ExpectedProtocol = 195;

            /// <summary>(194) Set of custom parameters which are sent in auth request.</summary>
            public const byte CustomInitData = 194;

            /// <summary>(193) How are we going to encrypt data.</summary>
            public const byte EncryptionMode = 193;

            /// <summary>(192) Parameter of Authentication, which contains encryption keys (depends on AuthMode and EncryptionMode).</summary>
            public const byte EncryptionData = 192;

            /// <summary>(191) An int parameter summarizing several boolean room-options with bit-flags.</summary>
            public const byte RoomOptionFlags = 191;
        }


        /// <summary>
        /// Class for constants. Contains operation codes.
        /// </summary>
        /// <remarks>These constants are used internally.</remarks>
        public class OperationCode
        {
            [Obsolete("Exchanging encrpytion keys is done internally in the lib now. Don't expect this operation-result.")]
            public const byte ExchangeKeysForEncryption = 250;

            /// <summary>(255) Code for OpJoin, to get into a room.</summary>
            [Obsolete]
            public const byte Join = 255;

            /// <summary>(231) Authenticates this peer and connects to a virtual application</summary>
            public const byte AuthenticateOnce = 231;

            /// <summary>(230) Authenticates this peer and connects to a virtual application</summary>
            public const byte Authenticate = 230;

            /// <summary>(229) Joins lobby (on master)</summary>
            public const byte JoinLobby = 229;

            /// <summary>(228) Leaves lobby (on master)</summary>
            public const byte LeaveLobby = 228;

            /// <summary>(227) Creates a game (or fails if name exists)</summary>
            public const byte CreateGame = 227;

            /// <summary>(226) Join game (by name)</summary>
            public const byte JoinGame = 226;

            /// <summary>(225) Joins random game (on master)</summary>
            public const byte JoinRandomGame = 225;

            // public const byte CancelJoinRandom = 224; // obsolete, cause JoinRandom no longer is a "process". now provides result immediately

            /// <summary>(254) Code for OpLeave, to get out of a room.</summary>
            public const byte Leave = (byte)254;

            /// <summary>(253) Raise event (in a room, for other actors/players)</summary>
            public const byte RaiseEvent = (byte)253;

            /// <summary>(252) Set Properties (of room or actor/player)</summary>
            public const byte SetProperties = (byte)252;

            /// <summary>(251) Get Properties</summary>
            public const byte GetProperties = (byte)251;

            /// <summary>(248) Operation code to change interest groups in Rooms (Lite application and extending ones).</summary>
            public const byte ChangeGroups = (byte)248;

            /// <summary>(222) Request the rooms and online status for a list of friends (by name, which should be unique).</summary>
            public const byte FindFriends = 222;

            /// <summary>(221) Request statistics about a specific list of lobbies (their user and game count).</summary>
            public const byte GetLobbyStats = 221;

            /// <summary>(220) Get list of regional servers from a NameServer.</summary>
            public const byte GetRegions = 220;

            /// <summary>(219) WebRpc Operation.</summary>
            public const byte WebRpc = 219;

            /// <summary>(218) Operation to set some server settings. Used with different parameters on various servers.</summary>
            public const byte ServerSettings = 218;

            /// <summary>(217) Get the game list matching a supplied sql filter (SqlListLobby only) </summary>
            public const byte GetGameList = 217;
        }

        /// <summary>
        /// Class for constants. These (byte) values define "well known" properties for an Actor / Player.
        /// </summary>
        /// <remarks>
        /// These constants are used internally.
        /// "Custom properties" have to use a string-type as key. They can be assigned at will.
        /// </remarks>
        public class ActorProperties
        {
            /// <summary>(255) Name of a player/actor.</summary>
            public const byte PlayerName = 255; // was: 1

            /// <summary>(254) Tells you if the player is currently in this game (getting events live).</summary>
            /// <remarks>A server-set value for async games, where players can leave the game and return later.</remarks>
            public const byte IsInactive = 254;

            /// <summary>(253) UserId of the player. Sent when room gets created with RoomOptions.PublishUserId = true.</summary>
            public const byte UserId = 253;
        }

        /// <summary>
        /// ErrorCode defines the default codes associated with Photon client/server communication.
        /// </summary>
        public class ErrorCode
        {
            /// <summary>(0) is always "OK", anything else an error or specific situation.</summary>
            public const int Ok = 0;

            // server - Photon low(er) level: <= 0

            /// <summary>
            /// (-3) Operation can't be executed yet (e.g. OpJoin can't be called before being authenticated, RaiseEvent cant be used before getting into a room).
            /// </summary>
            /// <remarks>
            /// Before you call any operations on the Cloud servers, the automated client workflow must complete its authorization.
            /// Wait until State is: JoinedLobby or ConnectedToMasterServer
            /// </remarks>
            public const int OperationNotAllowedInCurrentState = -3;

            /// <summary>(-2) The operation you called is not implemented on the server (application) you connect to. Make sure you run the fitting applications.</summary>
            [Obsolete("Use InvalidOperation.")]
            public const int InvalidOperationCode = -2;

            /// <summary>(-2) The operation you called could not be executed on the server.</summary>
            /// <remarks>
            /// Make sure you are connected to the server you expect.
            ///
            /// This code is used in several cases:
            /// The arguments/parameters of the operation might be out of range, missing entirely or conflicting.
            /// The operation you called is not implemented on the server (application). Server-side plugins affect the available operations.
            /// </remarks>
            public const int InvalidOperation = -2;

            /// <summary>(-1) Something went wrong in the server. Try to reproduce and contact Exit Games.</summary>
            public const int InternalServerError = -1;

            // server - client: 0x7FFF and down
            // logic-level error codes start with short.max

            /// <summary>(32767) Authentication failed. Possible cause: AppId is unknown to Photon (in cloud service).</summary>
            public const int InvalidAuthentication = 0x7FFF;

            /// <summary>(32766) GameId (name) already in use (can't create another). Change name.</summary>
            public const int GameIdAlreadyExists = 0x7FFF - 1;

            /// <summary>(32765) Game is full. This rarely happens when some player joined the room before your join completed.</summary>
            public const int GameFull = 0x7FFF - 2;

            /// <summary>(32764) Game is closed and can't be joined. Join another game.</summary>
            public const int GameClosed = 0x7FFF - 3;

            [Obsolete("No longer used, cause random matchmaking is no longer a process.")]
            public const int AlreadyMatched = 0x7FFF - 4;

            /// <summary>(32762) All servers are busy. This is a temporary issue and the game logic should try again after a brief wait time.</summary>
            /// <remarks>
            /// This error may happen for all operations that create rooms. The operation response will contain this error code.
            ///
            /// This error is very unlikely to happen as we monitor load on all servers and add them on demand.
            /// However, it's good to be prepared for a shortage of machines or surge in CCUs.
            /// </remarks>
            public const int ServerFull = 0x7FFF - 5;

            /// <summary>(32761) Not in use currently.</summary>
            public const int UserBlocked = 0x7FFF - 6;

            /// <summary>(32760) Random matchmaking only succeeds if a room exists thats neither closed nor full. Repeat in a few seconds or create a new room.</summary>
            public const int NoRandomMatchFound = 0x7FFF - 7;

            /// <summary>(32758) Join can fail if the room (name) is not existing (anymore). This can happen when players leave while you join.</summary>
            public const int GameDoesNotExist = 0x7FFF - 9;

            /// <summary>(32757) Authorization on the Photon Cloud failed becaus the concurrent users (CCU) limit of the app's subscription is reached.</summary>
            /// <remarks>
            /// Unless you have a plan with "CCU Burst", clients might fail the authentication step during connect.
            /// Affected client are unable to call operations. Please note that players who end a game and return
            /// to the master server will disconnect and re-connect, which means that they just played and are rejected
            /// in the next minute / re-connect.
            /// This is a temporary measure. Once the CCU is below the limit, players will be able to connect an play again.
            ///
            /// OpAuthorize is part of connection workflow but only on the Photon Cloud, this error can happen.
            /// Self-hosted Photon servers with a CCU limited license won't let a client connect at all.
            /// </remarks>
            public const int MaxCcuReached = 0x7FFF - 10;

            /// <summary>(32756) Authorization on the Photon Cloud failed because the app's subscription does not allow to use a particular region's server.</summary>
            /// <remarks>
            /// Some subscription plans for the Photon Cloud are region-bound. Servers of other regions can't be used then.
            /// Check your master server address and compare it with your Photon Cloud Dashboard's info.
            /// https://dashboard.photonengine.com
            ///
            /// OpAuthorize is part of connection workflow but only on the Photon Cloud, this error can happen.
            /// Self-hosted Photon servers with a CCU limited license won't let a client connect at all.
            /// </remarks>
            public const int InvalidRegion = 0x7FFF - 11;

            /// <summary>
            /// (32755) Custom Authentication of the user failed due to setup reasons (see Cloud Dashboard) or the provided user data (like username or token). Check error message for details.
            /// </summary>
            public const int CustomAuthenticationFailed = 0x7FFF - 12;

            /// <summary>(32753) The Authentication ticket expired. Usually, this is refreshed behind the scenes. Connect (and authorize) again.</summary>
            public const int AuthenticationTicketExpired = 0x7FF1;

            /// <summary>
            /// (32752) A server-side plugin (or webhook) failed to execute and reported an error. Check the OperationResponse.DebugMessage.
            /// </summary>
            public const int PluginReportedError = 0x7FFF - 15;

            /// <summary>
            /// (32751) CreateGame/JoinGame/Join operation fails if expected plugin does not correspond to loaded one.
            /// </summary>
            public const int PluginMismatch = 0x7FFF - 16;

            /// <summary>
            /// (32750) for join requests. Indicates the current peer already called join and is joined to the room.
            /// </summary>
            public const int JoinFailedPeerAlreadyJoined = 32750; // 0x7FFF - 17,

            /// <summary>
            /// (32749)  for join requests. Indicates the list of InactiveActors already contains an actor with the requested ActorNr or UserId.
            /// </summary>
            public const int JoinFailedFoundInactiveJoiner = 32749; // 0x7FFF - 18,

            /// <summary>
            /// (32748) for join requests. Indicates the list of Actors (active and inactive) did not contain an actor with the requested ActorNr or UserId.
            /// </summary>
            public const int JoinFailedWithRejoinerNotFound = 32748; // 0x7FFF - 19,

            /// <summary>
            /// (32747) for join requests. Note: for future use - Indicates the requested UserId was found in the ExcludedList.
            /// </summary>
            public const int JoinFailedFoundExcludedUserId = 32747; // 0x7FFF - 20,

            /// <summary>
            /// (32746) for join requests. Indicates the list of ActiveActors already contains an actor with the requested ActorNr or UserId.
            /// </summary>
            public const int JoinFailedFoundActiveJoiner = 32746; // 0x7FFF - 21,

            /// <summary>
            /// (32745)  for SetProerties and Raisevent (if flag HttpForward is true) requests. Indicates the maximum allowd http requests per minute was reached.
            /// </summary>
            public const int HttpLimitReached = 32745; // 0x7FFF - 22,

            /// <summary>
            /// (32744) for WebRpc requests. Indicates the the call to the external service failed.
            /// </summary>
            public const int ExternalHttpCallFailed = 32744; // 0x7FFF - 23,

            /// <summary>
            /// (32743) for operations with defined limits (as in calls per second, content count or size).
            /// </summary>
            public const int OperationLimitReached = 32743; // 0x7FFF - 24,

            /// <summary>
            /// (32742) Server error during matchmaking with slot reservation. E.g. the reserved slots can not exceed MaxPlayers.
            /// </summary>
            public const int SlotError = 32742; // 0x7FFF - 25,

            /// <summary>
            /// (32741) Server will react with this error if invalid encryption parameters provided by token
            /// </summary>
            public const int InvalidEncryptionParameters = 32741; // 0x7FFF - 24,

        }

    }
}

