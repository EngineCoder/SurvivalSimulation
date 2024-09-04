namespace UFO.Realtime
{
    using ExitGames.Client.Photon;
    using System;
    using System.Collections.Generic;
    using static UFO.Realtime.Peer;

    #region Enum

    /// <summary>
    /// State values for a client, which handles switching Photon server types, some operations, etc.
    /// </summary>
    /// \ingroup publicApi
    public enum ClientState
    {
        /// <summary>Peer is created but not used yet.</summary>
        PeerCreated,

        /// <summary>Transition state while connecting to a server. On the Photon Cloud this sends the AppId and AuthenticationValues (UserID).</summary>
        Authenticating,

        /// <summary>Not Used.</summary>
        Authenticated,

        /// <summary>The client sent an OpJoinLobby and if this was done on the Master Server, it will result in. Depending on the lobby, it gets room listings.</summary>
        JoiningLobby,

        /// <summary>The client is in a lobby, connected to the MasterServer. Depending on the lobby, it gets room listings.</summary>
        JoinedLobby,

        /// <summary>Transition from MasterServer to GameServer.</summary>
        DisconnectingFromMasterServer,
        [Obsolete("Renamed to DisconnectingFromMasterServer")]
        DisconnectingFromMasterserver = DisconnectingFromMasterServer,

        /// <summary>Transition to GameServer (client authenticates and joins/creates a room).</summary>
        ConnectingToGameServer,
        [Obsolete("Renamed to ConnectingToGameServer")]
        ConnectingToGameserver = ConnectingToGameServer,

        /// <summary>Connected to GameServer (going to auth and join game).</summary>
        ConnectedToGameServer,
        [Obsolete("Renamed to ConnectedToGameServer")]
        ConnectedToGameserver = ConnectedToGameServer,

        /// <summary>Transition state while joining or creating a room on GameServer.</summary>
        Joining,

        /// <summary>The client entered a room. The CurrentRoom and Players are known and you can now raise events.</summary>
        Joined,

        /// <summary>Transition state when leaving a room.</summary>
        Leaving,

        /// <summary>Transition from GameServer to MasterServer (after leaving a room/game).</summary>
        DisconnectingFromGameServer,
        [Obsolete("Renamed to DisconnectingFromGameServer")]
        DisconnectingFromGameserver = DisconnectingFromGameServer,

        /// <summary>Connecting to MasterServer (includes sending authentication values).</summary>
        ConnectingToMasterServer,
        [Obsolete("Renamed to ConnectingToMasterServer.")]
        ConnectingToMasterserver = ConnectingToMasterServer,

        /// <summary>The client disconnects (from any server). This leads to state Disconnected.</summary>
        Disconnecting,

        /// <summary>The client is no longer connected (to any server). Connect to MasterServer to go on.</summary>
        Disconnected,

        /// <summary>Connected to MasterServer. You might use matchmaking or join a lobby now.</summary>
        ConnectedToMasterServer,
        [Obsolete("Renamed to ConnectedToMasterServer.")]
        ConnectedToMasterserver = ConnectedToMasterServer,
        [Obsolete("Renamed to ConnectedToMasterServer.")]
        ConnectedToMaster = ConnectedToMasterServer,

        /// <summary>Client connects to the NameServer. This process includes low level connecting and setting up encryption. When done, state becomes ConnectedToNameServer.</summary>
        ConnectingToNameServer,

        /// <summary>Client is connected to the NameServer and established encryption already. You should call OpGetRegions or ConnectToRegionMaster.</summary>
        ConnectedToNameServer,

        /// <summary>Clients disconnects (specifically) from the NameServer (usually to connect to the MasterServer).</summary>
        DisconnectingFromNameServer,

        /// <summary>Client was unable to connect to Name Server and will attempt to connect with an alternative network protocol (TCP).</summary>
        ConnectWithFallbackProtocol
    }

    /// <summary>
    /// Internal state, how this peer gets into a particular room (joining it or creating it).
    /// </summary>
    internal enum JoinType
    {
        /// <summary>This client creates a room, gets into it (no need to join) and can set room properties.</summary>
        CreateRoom,
        /// <summary>The room existed already and we join into it (not setting room properties).</summary>
        JoinRoom,
        /// <summary>Done on Master Server and (if successful) followed by a Join on Game Server.</summary>
        JoinRandomRoom,
        /// <summary>Done on Master Server and (if successful) followed by a Join or Create on Game Server.</summary>
        JoinRandomOrCreateRoom,
        /// <summary>Client is either joining or creating a room. On Master- and Game-Server.</summary>
        JoinOrCreateRoom
    }

    /// <summary>Enumeration of causes for Disconnects (used in Client.DisconnectedCause).</summary>
    /// <remarks>Read the individual descriptions to find out what to do about this type of disconnect.</remarks>
    public enum DisconnectCause
    {
        /// <summary>No error was tracked.</summary>
        None,
        /// <summary>OnStatusChanged: The server is not available or the address is wrong. Make sure the port is provided and the server is up.</summary>
        ExceptionOnConnect,
        /// <summary>OnStatusChanged: Some internal exception caused the socket code to fail. This may happen if you attempt to connect locally but the server is not available. In doubt: Contact Exit Games.</summary>
        Exception,

        /// <summary>OnStatusChanged: The server disconnected this client due to timing out (missing acknowledgement from the client).</summary>
        ServerTimeout,

        /// <summary>OnStatusChanged: This client detected that the server's responses are not received in due time.</summary>
        ClientTimeout,

        /// <summary>OnStatusChanged: The server disconnected this client from within the room's logic (the C# code).</summary>
        DisconnectByServerLogic,
        /// <summary>OnStatusChanged: The server disconnected this client for unknown reasons.</summary>
        DisconnectByServerReasonUnknown,

        /// <summary>OnOperationResponse: Authenticate in the Photon Cloud with invalid AppId. Update your subscription or contact Exit Games.</summary>
        InvalidAuthentication,
        /// <summary>OnOperationResponse: Authenticate in the Photon Cloud with invalid client values or custom authentication setup in Cloud Dashboard.</summary>
        CustomAuthenticationFailed,
        /// <summary>The authentication ticket should provide access to any Photon Cloud server without doing another authentication-service call. However, the ticket expired.</summary>
        AuthenticationTicketExpired,
        /// <summary>OnOperationResponse: Authenticate (temporarily) failed when using a Photon Cloud subscription without CCU Burst. Update your subscription.</summary>
        MaxCcuReached,

        /// <summary>OnOperationResponse: Authenticate when the app's Photon Cloud subscription is locked to some (other) region(s). Update your subscription or master server address.</summary>
        InvalidRegion,

        /// <summary>OnOperationResponse: Operation that's (currently) not available for this client (not authorized usually). Only tracked for op Authenticate.</summary>
        OperationNotAllowedInCurrentState,
        /// <summary>OnStatusChanged: The client disconnected from within the logic (the C# code).</summary>
        DisconnectByClientLogic
    }

    /// <summary>Available server (types) for internally used field: server.</summary>
    /// <remarks>Photon uses 3 different roles of servers: Name Server, Master Server and Game Server.</remarks>
    public enum ServerConnection
    {
        /// <summary>This server is where matchmaking gets done and where clients can get lists of rooms in lobbies.</summary>
        MasterServer,
        /// <summary>This server handles a number of rooms to execute and relay the messages between players (in a room).</summary>
        GameServer,
        /// <summary>This server is used initially to get the address (IP) of a Master Server for a specific region. Not used for Photon OnPremise (self hosted).</summary>
        NameServer
    }

    /// <summary>
    /// Defines how the communication gets encrypted.
    /// </summary>
    public enum EncryptionMode
    {
        /// <summary>
        /// This is the default encryption mode: Messages get encrypted only on demand (when you send operations with the "encrypt" parameter set to true).
        /// </summary>
        PayloadEncryption,
        /// <summary>
        /// With this encryption mode for UDP, the connection gets setup and all further datagrams get encrypted almost entirely. On-demand message encryption (like in PayloadEncryption) is unavailable.
        /// </summary>
        DatagramEncryption = 10,
        /// <summary>
        /// With this encryption mode for UDP, the connection gets setup with random sequence numbers and all further datagrams get encrypted almost entirely. On-demand message encryption (like in PayloadEncryption) is unavailable.
        /// </summary>
        DatagramEncryptionRandomSequence = 11,
        /// <summary>
        /// Same as above except that GCM mode is used to encrypt data
        /// </summary>
        DatagramEncryptionGCMRandomSequence = 12,
    }

    #endregion


    public class Client : IPhotonPeerListener
    {
        #region Private Field
        /// <summary>Name Server port per protocol (the UDP port is different than TCP, etc).</summary>
        private static readonly Dictionary<ConnectionProtocol, int> ProtocolToNameServerPort = new Dictionary<ConnectionProtocol, int>() { { ConnectionProtocol.Udp, 5058 }, { ConnectionProtocol.Tcp, 4533 }, { ConnectionProtocol.WebSocket, 9093 }, { ConnectionProtocol.WebSocketSecure, 19093 } }; //, { ConnectionProtocol.RHttp, 6063 } };
        /// <summary>Backing field for property.</summary>
        private ClientState state = ClientState.PeerCreated;
        /// <summary>Internally used cache for the server's token. Identifies a user/session and can be used to rejoin.</summary>
        private string tokenCache;
        /// <summary>Contains the list of names of friends to look up their state on the server.</summary>
        private string[] friendListRequested;

        /// <summary>Wraps up the target objects for a group of callbacks, so they can be called conveniently.</summary>
        /// <remarks>By using Add or Remove, objects can "subscribe" or "unsubscribe" for this group  of callbacks.</remarks>
        internal InRoomCallbacksContainer InRoomCallbackTargets;
        /// <summary>Wraps up the target objects for a group of callbacks, so they can be called conveniently.</summary>
        /// <remarks>By using Add or Remove, objects can "subscribe" or "unsubscribe" for this group  of callbacks.</remarks>
        public ConnectionCallbacksContainer ConnectionCallbackTargets;

        /// <summary>Wraps up the target objects for a group of callbacks, so they can be called conveniently.</summary>
        /// <remarks>By using Add or Remove, objects can "subscribe" or "unsubscribe" for this group  of callbacks.</remarks>
        public MatchMakingCallbacksContainer MatchMakingCallbackTargets;


        /// <summary>Wraps up the target objects for a group of callbacks, so they can be called conveniently.</summary>
        /// <remarks>By using Add or Remove, objects can "subscribe" or "unsubscribe" for this group  of callbacks.</remarks>
        internal LobbyCallbacksContainer LobbyCallbackTargets;

        /// <summary>Wraps up the target objects for a group of callbacks, so they can be called conveniently.</summary>
        /// <remarks>By using Add or Remove, objects can "subscribe" or "unsubscribe" for this group  of callbacks.</remarks>
        internal WebRpcCallbacksContainer WebRpcCallbackTargets;


        /// <summary>Wraps up the target objects for a group of callbacks, so they can be called conveniently.</summary>
        /// <remarks>By using Add or Remove, objects can "subscribe" or "unsubscribe" for this group  of callbacks.</remarks>
        internal ErrorInfoCallbacksContainer ErrorInfoCallbackTargets;
        #endregion


        #region Public Field



        /// <summary>Contains the list if enabled regions this client may use. Null, unless the client got a response to OpGetRegions.</summary>
        public RegionHandler RegionHandler;


        /// <summary>
        /// Defines a proxy URL for WebSocket connections. Can be the proxy or point to a .pac file.
        /// </summary>
        /// <remarks>
        /// This URL supports various definitions:
        ///
        /// "user:pass@proxyaddress:port"<br/>
        /// "proxyaddress:port"<br/>
        /// "system:"<br/>
        /// "pac:"<br/>
        /// "pac:http://host/path/pacfile.pac"<br/>
        ///
        /// Important: Don't define a protocol, except to point to a pac file. the proxy address should not begin with http:// or https://.
        /// </remarks>
        public string ProxyServerAddress;

        public int NameServerPortOverride;

        /// <summary>Name Server Host Name for Photon Cloud. Without port and without any prefix.</summary>
        public string NameServerHost = "ns.exitgames.com";

        /// <summary>Name Server for HTTP connections to the Photon Cloud. Includes prefix and port.</summary>
        public string NameServerHttp = "http://ns.exitgames.com:80/photon/n";

        #endregion


        #region Attributes

        ///<summary>Simplifies getting the token for connect/init requests, if this feature is enabled.</summary>
        private string TokenForInit
        {
            get
            {
                if (this.AuthMode == AuthModeOption.Auth)
                {
                    return null;
                }
                return (this.AuthValues != null) ? this.AuthValues.Token : null;
            }
        }

        /// <summary>
        /// 客户端使用LoadBalancingPeer作为API与服务器通信。
        /// </summary>
        public Peer Peer { get; private set; }

        /// <summary>
        /// Gets or sets the binary protocol version used by this client
        /// 获取或设置此客户端使用的二进制协议版本
        /// </summary>
        /// <remarks>
        /// Use this always instead of setting it via <see cref="Client.LoadBalancingPeer"/>
        /// (<see cref="PhotonPeer.SerializationProtocolType"/>) directly, especially when WSS protocol is used.
        /// </remarks>
        public SerializationProtocol SerializationProtocol
        {
            get { return this.Peer.SerializationProtocolType; }
            set { this.Peer.SerializationProtocolType = value; }
        }

        /// <summary>User authentication values to be sent to the Photon server right after connecting.</summary>
        /// <remarks>Set this property or pass AuthenticationValues by Connect(..., authValues).</remarks>
        public AuthenticationValues AuthValues { get; set; }

        /// <summary>Optionally contains a protocol which will be used on Master- and GameServer. 
        /// （可选）包含将在Master和GameServer上使用的协议。
        /// </summary>
        /// <remarks>
        /// When using AuthMode = AuthModeOption.AuthOnceWss, the client uses a wss-connection on the NameServer but another protocol on the other servers.
        /// As the NameServer sends an address, which is different per protocol, it needs to know the expected protocol.
        ///
        /// This is nullable by design. In many cases, the protocol on the NameServer is not different from the other servers.
        /// If set, the operation AuthOnce will contain this value and the OpAuth response on the NameServer will execute a protocol switch.
        /// </remarks>
        public ConnectionProtocol? ExpectedProtocol { get; private set; }

        /// <summary>Enables the new Authentication workflow.</summary>
        public AuthModeOption AuthMode = AuthModeOption.Auth;


        /// <summary>True if this client uses a NameServer to get the Master Server address.</summary>
        /// <remarks>This value is public, despite being an internal value, which should only be set by this client.</remarks>
        public bool IsUsingNameServer { get; set; }

        /// <summary>Name Server Address for Photon Cloud (based on current protocol). You can use the default values and usually won't have to set this value.</summary>
        public string NameServerAddress { get { return this.GetNameServerAddress(); } }

        /// <summary>Use the alternative ports for UDP connections in the Public Cloud (27000 to 27003).</summary>
        /// <remarks>
        /// This should be used when players have issues with connection stability.
        /// Some players reported better connectivity for Steam games.
        /// The effect might vary, which is why the alternative ports are not the new default.
        ///
        /// The alternative (server) ports are 27000 up to 27003.
        ///
        /// The values are appplied by replacing any incoming server-address string accordingly.
        /// You only need to set this to true though.
        ///
        /// This value does not affect TCP or WebSocket connections.
        /// </remarks>
        public bool UseAlternativeUdpPorts { get; set; }

        /// <summary>The currently used server address (if any). The type of server is define by Server property.</summary>
        public string CurrentServerAddress { get { return this.Peer.ServerAddress; } }

        /// <summary>Enables a fallback to another protocol in case a connect to the Name Server fails.</summary>
        /// <remarks>
        /// When connecting to the Name Server fails for a first time, the client will automatically select a different
        /// network protocol and try connecting once more.
        ///
        /// The fallback will use the default Name Server port as defined by ProtocolToNameServerPort.
        ///
        /// The fallback for TCP is UDP. All other protocols fallback to TCP.
        /// </remarks>
        public bool EnableProtocolFallback { get; set; }

        /// <summary>Your Master Server address. In PhotonCloud, call ConnectToRegionMaster() to find your Master Server.</summary>
        /// <remarks>
        /// In the Photon Cloud, explicit definition of a Master Server Address is not best practice.
        /// The Photon Cloud has a "Name Server" which redirects clients to a specific Master Server (per Region and AppId).
        /// </remarks>
        public string MasterServerAddress { get; set; }

        /// <summary>The game server's address for a particular room. In use temporarily, as assigned by master.</summary>
        public string GameServerAddress { get; protected internal set; }

        /// <summary>The server this client is currently connected or connecting to.</summary>
        /// <remarks>
        /// Each server (NameServer, MasterServer, GameServer) allow some operations and reject others.
        /// </remarks>
        public ServerConnection Server { get; private set; }

        /// <summary>Current state this client is in. Careful: several states are "transitions" that lead to other states.</summary>
        public ClientState State
        {
            get
            {
                return this.state;
            }

            set
            {
                if (this.state == value)
                {
                    return;
                }
                ClientState previousState = this.state;
                this.state = value;
                if (StateChanged != null) StateChanged(previousState, this.state);
            }
        }

        /// <summary>Returns if this client is currently connected or connecting to some type of server.</summary>
        /// <remarks>This is even true while switching servers. Use IsConnectedAndReady to check only for those states that enable you to send Operations.</remarks>
        public bool IsConnected { get { return this.Peer != null && this.State != ClientState.PeerCreated && this.State != ClientState.Disconnected; } }


        /// <summary>
        /// A refined version of IsConnected which is true only if your connection is ready to send operations.
        /// </summary>
        /// <remarks>
        /// Not all operations can be called on all types of servers. If an operation is unavailable on the currently connected server,
        /// this will result in a OperationResponse with ErrorCode != 0.
        ///
        /// Examples: The NameServer allows OpGetRegions which is not available anywhere else.
        /// The MasterServer does not allow you to send events (OpRaiseEvent) and on the GameServer you are unable to join a lobby (OpJoinLobby).
        ///
        /// To check which server you are on, use: <see cref="Server"/>.
        /// </remarks>
        public bool IsConnectedAndReady
        {
            get
            {
                if (this.Peer == null)
                {
                    return false;
                }

                switch (this.State)
                {
                    case ClientState.PeerCreated:
                    case ClientState.Disconnected:
                    case ClientState.Disconnecting:
                    case ClientState.DisconnectingFromGameServer:
                    case ClientState.DisconnectingFromMasterServer:
                    case ClientState.DisconnectingFromNameServer:
                    case ClientState.Authenticating:
                    case ClientState.ConnectingToGameServer:
                    case ClientState.ConnectingToMasterServer:
                    case ClientState.ConnectingToNameServer:
                    case ClientState.Joining:
                    case ClientState.Leaving:
                        return false;   // we are not ready to execute any operations
                }

                return true;
            }
        }


        /// <summary>Summarizes (aggregates) the different causes for disconnects of a client.</summary>
        /// <remarks>
        /// A disconnect can be caused by: errors in the network connection or some vital operation failing
        /// (which is considered "high level"). While operations always trigger a call to OnOperationResponse,
        /// connection related changes are treated in OnStatusChanged.
        /// The DisconnectCause is set in either case and summarizes the causes for any disconnect in a single
        /// state value which can be used to display (or debug) the cause for disconnection.
        /// </remarks>
        public DisconnectCause DisconnectedCause { get; protected set; }


        /// <summary>Internal value if the client is in a lobby.</summary>
        /// <remarks>This is used to re-set this.State, when joining/creating a room fails.</remarks>
        public bool InLobby
        {
            get { return this.State == ClientState.JoinedLobby; }
        }

        /// <summary>The lobby this client currently uses. Defined when joining a lobby or creating rooms</summary>
        public TypedLobby CurrentLobby { get; internal set; }

        /// <summary>
        /// If enabled, the client will get a list of available lobbies from the Master Server.
        /// </summary>
        /// <remarks>
        /// Set this value before the client connects to the Master Server. While connected to the Master
        /// Server, a change has no effect.
        ///
        /// Implement OptionalInfoCallbacks.OnLobbyStatisticsUpdate, to get the list of used lobbies.
        ///
        /// The lobby statistics can be useful if your title dynamically uses lobbies, depending (e.g.)
        /// on current player activity or such.
        /// In this case, getting a list of available lobbies, their room-count and player-count can
        /// be useful info.
        ///
        /// ConnectUsingSettings sets this to the PhotonServerSettings value.
        /// </remarks>
        public bool EnableLobbyStatistics;

        /// <summary>Internal lobby stats cache, used by LobbyStatistics.</summary>
        private readonly List<TypedLobbyInfo> lobbyStatistics = new List<TypedLobbyInfo>();


        /// <summary>The local player is never null but not valid unless the client is in a room, too. The ID will be -1 outside of rooms.</summary>
        public Player LocalPlayer { get; internal set; }

        /// <summary>
        /// The nickname of the player (synced with others). Same as client.LocalPlayer.NickName.
        /// </summary>
        public string NickName
        {
            get
            {
                return this.LocalPlayer.NickName;
            }

            set
            {
                if (this.LocalPlayer == null)
                {
                    return;
                }

                this.LocalPlayer.NickName = value;
            }
        }


        /// <summary>An ID for this user. Sent in OpAuthenticate when you connect. If not set, the PlayerName is applied during connect.</summary>
        /// <remarks>
        /// On connect, if the UserId is null or empty, the client will copy the PlayName to UserId. If PlayerName is not set either
        /// (before connect), the server applies a temporary ID which stays unknown to this client and other clients.
        ///
        /// The UserId is what's used in FindFriends and for fetching data for your account (with WebHooks e.g.).
        ///
        /// By convention, set this ID before you connect, not while being connected.
        /// There is no error but the ID won't change while being connected.
        /// </remarks>
        public string UserId
        {
            get
            {
                if (this.AuthValues != null)
                {
                    return this.AuthValues.UserId;
                }
                return null;
            }
            set
            {
                if (this.AuthValues == null)
                {
                    this.AuthValues = new AuthenticationValues();
                }
                this.AuthValues.UserId = value;
            }
        }

        /// <summary>The current room this client is connected to (null if none available).</summary>
        public Room CurrentRoom { get; set; }


        /// <summary>Is true while being in a room (this.state == ClientState.Joined).</summary>
        /// <remarks>
        /// Aside from polling this value, game logic should implement IMatchmakingCallbacks in some class
        /// and react when that gets called.<br/>
        /// OpRaiseEvent, OpLeave and some other operations can only be used (successfully) when the client is in a room..
        /// </remarks>
        public bool InRoom
        {
            get
            {
                return this.state == ClientState.Joined && this.CurrentRoom != null;
            }
        }

        /// <summary>Statistic value available on master server: Players on master (looking for games).</summary>
        public int PlayersOnMasterCount { get; internal set; }

        /// <summary>Statistic value available on master server: Players in rooms (playing).</summary>
        public int PlayersInRoomsCount { get; internal set; }

        /// <summary>Statistic value available on master server: Rooms currently created.</summary>
        public int RoomsCount { get; internal set; }
        /// <summary>Internal flag to know if the client currently fetches a friend list.</summary>
        public bool IsFetchingFriendList { get { return this.friendListRequested != null; } }


        /// <summary>The cloud region this client connects to. Set by ConnectToRegionMaster(). Not set if you don't use a NameServer!</summary>
        public string CloudRegion { get; private set; }

        /// <summary>The cluster name provided by the Name Server.</summary>
        /// <remarks>
        /// The value is provided by the OpResponse for OpAuthenticate/OpAuthenticateOnce.
        /// Default: null. This value only ever updates from the Name Server authenticate response.
        /// </remarks>
        public string CurrentCluster { get; private set; }


        #endregion


        #region Private Method
        /// <summary>
        /// Gets the NameServer Address (with prefix and port), based on the set protocol (this.LoadBalancingPeer.UsedProtocol).
        /// </summary>
        /// <returns>NameServer Address (with prefix and port).</returns>
        private string GetNameServerAddress()
        {
            var protocolPort = 0;
            ProtocolToNameServerPort.TryGetValue(this.Peer.TransportProtocol, out protocolPort);
            if (this.Peer.TransportProtocol == ConnectionProtocol.Udp && this.UseAlternativeUdpPorts)
            {
                protocolPort = 27000;
            }

            if (this.NameServerPortOverride != 0)
            {
                this.DebugReturn(DebugLevel.INFO, string.Format("Using NameServerPortOverride: {0}", this.NameServerPortOverride));
                protocolPort = this.NameServerPortOverride;
            }

            switch (this.Peer.TransportProtocol)
            {
                case ConnectionProtocol.Udp:
                case ConnectionProtocol.Tcp:
                    return string.Format("{0}:{1}", NameServerHost, protocolPort);
#if RHTTP
                case ConnectionProtocol.RHttp:
                    return NameServerHttp;
#endif
                case ConnectionProtocol.WebSocket:
                    return string.Format("ws://{0}:{1}", NameServerHost, protocolPort);
                case ConnectionProtocol.WebSocketSecure:
                    return string.Format("wss://{0}:{1}", NameServerHost, protocolPort);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        private bool CheckIfOpAllowedOnServer(byte opCode, ServerConnection serverConnection)
        {
            switch (serverConnection)
            {
                case ServerConnection.MasterServer:
                    switch (opCode)
                    {
                        case OperationCode.CreateGame:
                        case OperationCode.Authenticate:
                        case OperationCode.AuthenticateOnce:
                        case OperationCode.FindFriends:
                        case OperationCode.GetGameList:
                        case OperationCode.GetLobbyStats:
                        case OperationCode.JoinGame:
                        case OperationCode.JoinLobby:
                        case OperationCode.LeaveLobby:
                        case OperationCode.WebRpc:
                        case OperationCode.ServerSettings:
                        case OperationCode.JoinRandomGame:
                            return true;
                    }
                    break;
                case ServerConnection.GameServer:
                    switch (opCode)
                    {
                        case OperationCode.CreateGame:
                        case OperationCode.Authenticate:
                        case OperationCode.AuthenticateOnce:
                        case OperationCode.ChangeGroups:
                        case OperationCode.GetProperties:
                        case OperationCode.JoinGame:
                        case OperationCode.Leave:
                        case OperationCode.WebRpc:
                        case OperationCode.ServerSettings:
                        case OperationCode.SetProperties:
                        case OperationCode.RaiseEvent:
                            return true;
                    }
                    break;
                case ServerConnection.NameServer:
                    switch (opCode)
                    {
                        case OperationCode.Authenticate:
                        case OperationCode.AuthenticateOnce:
                        case OperationCode.GetRegions:
                        case OperationCode.ServerSettings:
                            return true;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException("serverConnection", serverConnection, null);
            }
            return false;
        }

        private bool CheckIfOpCanBeSent(byte opCode, ServerConnection serverConnection, string opName)
        {
            if (this.Peer == null)
            {
                this.DebugReturn(DebugLevel.ERROR, string.Format("Operation {0} ({1}) can't be sent because peer is null", opName, opCode));
                return false;
            }
            if (!this.CheckIfOpAllowedOnServer(opCode, serverConnection))
            {
                if (this.Peer.DebugOut >= DebugLevel.ERROR)
                {
                    this.DebugReturn(DebugLevel.ERROR, string.Format("Operation {0} ({1}) not allowed on current server ({2})", opName, opCode, serverConnection));
                }
                return false;
            }
            if (!this.CheckIfClientIsReadyToCallOperation(opCode))
            {
                if (this.Peer.DebugOut >= DebugLevel.ERROR)
                {
                    this.DebugReturn(DebugLevel.ERROR, string.Format("Operation {0} ({1}) not called because client is not connected or not ready yet, client state: {2}", opName, opCode, Enum.GetName(typeof(ClientState), this.State)));
                }
                return false;
            }
            if (this.Peer.PeerState != PeerStateValue.Connected)
            {
                this.DebugReturn(DebugLevel.ERROR, string.Format("Operation {0} ({1}) can't be sent because peer is not connected, peer state: {2}", opName, opCode, this.Peer.PeerState));
                return false;
            }
            return true;
        }

        private bool CheckIfClientIsReadyToCallOperation(byte opCode)
        {
            switch (opCode)
            {
                //case OperationCode.ServerSettings: // ??
                //case OperationCode.WebRpc: // WebRPC works on MS and GS and I think it does not need the client to be ready

                case OperationCode.Authenticate:
                case OperationCode.AuthenticateOnce:
                    return this.IsConnectedAndReady ||
                         this.State == ClientState.ConnectingToNameServer || // this is required since we do not set state to ConnectedToNameServer before authentication
                        this.State == ClientState.ConnectingToMasterServer || // this is required since we do not set state to ConnectedToMasterServer before authentication
                        this.State == ClientState.ConnectingToGameServer; // this is required since we do not set state to ConnectedToGameServer before authentication

                case OperationCode.ChangeGroups:
                case OperationCode.GetProperties:
                case OperationCode.SetProperties:
                case OperationCode.RaiseEvent:
                case OperationCode.Leave:
                    return this.InRoom;

                case OperationCode.JoinGame:
                case OperationCode.CreateGame:
                    return this.State == ClientState.ConnectedToMasterServer || this.InLobby || this.State == ClientState.ConnectedToGameServer; // CurrentRoom can be not null in case of quick rejoin

                case OperationCode.LeaveLobby:
                    return this.InLobby;

                case OperationCode.JoinRandomGame:
                case OperationCode.FindFriends:
                case OperationCode.GetGameList:
                case OperationCode.GetLobbyStats: // do we need to be inside lobby to call this?
                case OperationCode.JoinLobby: // You don't have to explicitly leave a lobby to join another (client can be in one max, at any time)
                    return this.State == ClientState.ConnectedToMasterServer || this.InLobby;
                case OperationCode.GetRegions:
                    return this.State == ClientState.ConnectedToNameServer;
            }
            return this.IsConnected;
        }


        /// <summary>Helper method to cast and apply a target per (interface) type.</summary>
        /// <typeparam name="T">Either of the interfaces for callbacks.</typeparam>
        /// <param name="change">The queued change to apply (add or remove) some target.</param>
        /// <param name="container">The container that calls callbacks on it's list of targets.</param>
        private void UpdateCallbackTarget<T>(CallbackTargetChange change, List<T> container) where T : class
        {
            T target = change.Target as T;
            if (target != null)
            {
                if (change.AddTarget)
                {
                    container.Add(target);
                }
                else
                {
                    container.Remove(target);
                }
            }
        }
        #endregion


        #region Protected Method
        protected internal bool OpSetPropertyOfRoom(byte propCode, object value)
        {
            Hashtable properties = new Hashtable();
            properties[propCode] = value;
            return this.OpSetPropertiesOfRoom(properties);
        }

        /// <summary>Internally used to cache and set properties (including well known properties).</summary>
        /// <remarks>Requires being in a room (because this attempts to send an operation which will fail otherwise).</remarks>
        protected internal bool OpSetPropertiesOfRoom(Hashtable gameProperties, Hashtable expectedProperties = null, WebFlags webFlags = null)
        {
            if (!this.CheckIfOpCanBeSent(OperationCode.SetProperties, this.Server, "SetProperties"))
            {
                return false;
            }
            if (gameProperties == null || gameProperties.Count == 0)
            {
                this.DebugReturn(DebugLevel.ERROR, "OpSetPropertiesOfRoom() failed. gameProperties must not be null nor empty.");
                return false;
            }
            bool res = this.Peer.OpSetPropertiesOfRoom(gameProperties, expectedProperties, webFlags);
            if (res && !this.CurrentRoom.BroadcastPropertiesChangeToAll && (expectedProperties == null || expectedProperties.Count == 0))
            {
                this.CurrentRoom.InternalCacheProperties(gameProperties);
                this.InRoomCallbackTargets.OnRoomPropertiesUpdate(gameProperties);
            }
            return res;
        }


        /// <summary>
        /// Applies queued callback cahnges from a queue to the actual containers. Will cause exceptions if used while callbacks execute.
        /// </summary>
        /// <remarks>
        /// There is no explicit check that this is not called during callbacks, however the implemented, private logic takes care of this.
        /// </remarks>
        protected internal void UpdateCallbackTargets()
        {
            while (this.callbackTargetChanges.Count > 0)
            {
                CallbackTargetChange change = this.callbackTargetChanges.Dequeue();

                if (change.AddTarget)
                {
                    if (this.callbackTargets.Contains(change.Target))
                    {
                        //Debug.Log("UpdateCallbackTargets skipped adding a target, as the object is already registered. Target: " + change.Target);
                        continue;
                    }

                    this.callbackTargets.Add(change.Target);
                }
                else
                {
                    if (!this.callbackTargets.Contains(change.Target))
                    {
                        //Debug.Log("UpdateCallbackTargets skipped removing a target, as the object is not registered. Target: " + change.Target);
                        continue;
                    }

                    this.callbackTargets.Remove(change.Target);
                }

                this.UpdateCallbackTarget<IInRoomCallbacks>(change, this.InRoomCallbackTargets);
                this.UpdateCallbackTarget<IConnectionCallbacks>(change, this.ConnectionCallbackTargets);
                this.UpdateCallbackTarget<IMatchmakingCallbacks>(change, this.MatchMakingCallbackTargets);
                this.UpdateCallbackTarget<ILobbyCallbacks>(change, this.LobbyCallbackTargets);
                this.UpdateCallbackTarget<IWebRpcCallback>(change, this.WebRpcCallbackTargets);
                this.UpdateCallbackTarget<IErrorInfoCallback>(change, this.ErrorInfoCallbackTargets);

                IOnEventCallback onEventCallback = change.Target as IOnEventCallback;
                if (onEventCallback != null)
                {
                    if (change.AddTarget)
                    {
                        EventReceived += onEventCallback.OnEvent;
                    }
                    else
                    {
                        EventReceived -= onEventCallback.OnEvent;
                    }
                }
            }
        }


        /// <summary>Internally used to cache and set properties (including well known properties).</summary>
        /// <remarks>Requires being in a room (because this attempts to send an operation which will fail otherwise).</remarks>
        protected internal bool OpSetPropertiesOfActor(int actorNr, Hashtable actorProperties, Hashtable expectedProperties = null, WebFlags webFlags = null)
        {
            if (!this.CheckIfOpCanBeSent(OperationCode.SetProperties, this.Server, "SetProperties"))
            {
                return false;
            }
            if (actorProperties == null || actorProperties.Count == 0)
            {
                this.DebugReturn(DebugLevel.ERROR, "OpSetPropertiesOfActor() failed. actorProperties must not be null nor empty.");
                return false;
            }
            bool res = this.Peer.OpSetPropertiesOfActor(actorNr, actorProperties, expectedProperties, webFlags);
            if (res && !this.CurrentRoom.BroadcastPropertiesChangeToAll && (expectedProperties == null || expectedProperties.Count == 0))
            {
                Player target = this.CurrentRoom.GetPlayer(actorNr);
                if (target != null)
                {
                    target.InternalCacheProperties(actorProperties);
                    this.InRoomCallbackTargets.OnPlayerPropertiesUpdate(target, actorProperties);
                }
            }
            return res;
        }

        #endregion


        #region Public Method


        #endregion


        #region Event
        /// <summary>Register a method to be called when this client's ClientState gets set.</summary>
        /// <remarks>This can be useful to react to being connected, joined into a room, etc.</remarks>
        public event Action<ClientState, ClientState> StateChanged;

        /// <summary>Register a method to be called when an event got dispatched. Gets called after the LoadBalancingClient handled the internal events first.</summary>
        /// <remarks>
        /// This is an alternative to extending LoadBalancingClient to override OnEvent().
        ///
        /// Note that OnEvent is calling EventReceived after it handled internal events first.
        /// That means for example: Joining players will already be in the player list but leaving
        /// players will already be removed from the room.
        /// </remarks>
        public event Action<EventData> EventReceived;

        /// <summary>Register a method to be called when an operation response is received.</summary>
        /// <remarks>
        /// This is an alternative to extending LoadBalancingClient to override OnOperationResponse().
        ///
        /// Note that OnOperationResponse gets executed before your Action is called.
        /// That means for example: The OpJoinLobby response already set the state to "JoinedLobby"
        /// and the response to OpLeave already triggered the Disconnect before this is called.
        /// </remarks>
        public event Action<OperationResponse> OpResponseReceived;
        #endregion


        #region Callbacks

        public void DebugReturn(DebugLevel level, string message)
        {

        }

        public void OnEvent(EventData eventData)
        {

        }

        public void OnOperationResponse(OperationResponse operationResponse)
        {

        }

        public void OnStatusChanged(StatusCode statusCode)
        {

        }

        #endregion


        private class CallbackTargetChange
        {
            public readonly object Target;
            /// <summary>Add if true, remove if false.</summary>
            public readonly bool AddTarget;

            public CallbackTargetChange(object target, bool addTarget)
            {
                this.Target = target;
                this.AddTarget = addTarget;
            }
        }
        private readonly Queue<CallbackTargetChange> callbackTargetChanges = new Queue<CallbackTargetChange>();
        private readonly HashSet<object> callbackTargets = new HashSet<object>();
    }




    /// <summary>
    /// Collection of "in room" callbacks for the Realtime Api to cover: Players entering or leaving, property updates and Master Client switching.
    /// </summary>
    /// <remarks>
    /// Classes that implement this interface must be registered to get callbacks for various situations.
    ///
    /// To register for callbacks, call <see cref="Client.AddCallbackTarget"/> and pass the class implementing this interface
    /// To stop getting callbacks, call <see cref="Client.RemoveCallbackTarget"/> and pass the class implementing this interface
    ///
    /// </remarks>
    /// \ingroup callbacks
    public interface IInRoomCallbacks
    {
        /// <summary>
        /// Called when a remote player entered the room. This Player is already added to the playerlist.
        /// </summary>
        /// <remarks>
        /// If your game starts with a certain number of players, this callback can be useful to check the
        /// Room.playerCount and find out if you can start.
        /// </remarks>
        void OnPlayerEnteredRoom(Player newPlayer);

        /// <summary>
        /// Called when a remote player left the room or became inactive. Check otherPlayer.IsInactive.
        /// </summary>
        /// <remarks>
        /// If another player leaves the room or if the server detects a lost connection, this callback will
        /// be used to notify your game logic.
        ///
        /// Depending on the room's setup, players may become inactive, which means they may return and retake
        /// their spot in the room. In such cases, the Player stays in the Room.Players dictionary.
        ///
        /// If the player is not just inactive, it gets removed from the Room.Players dictionary, before
        /// the callback is called.
        /// </remarks>
        void OnPlayerLeftRoom(Player otherPlayer);


        /// <summary>
        /// Called when a room's custom properties changed. The propertiesThatChanged contains all that was set via Room.SetCustomProperties.
        /// </summary>
        /// <remarks>
        /// Since v1.25 this method has one parameter: Hashtable propertiesThatChanged.<br/>
        /// Changing properties must be done by Room.SetCustomProperties, which causes this callback locally, too.
        /// </remarks>
        /// <param name="propertiesThatChanged"></param>
        void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged);

        /// <summary>
        /// Called when custom player-properties are changed. Player and the changed properties are passed as object[].
        /// </summary>
        /// <remarks>
        /// Changing properties must be done by Player.SetCustomProperties, which causes this callback locally, too.
        /// </remarks>
        /// <param name="targetPlayer">Contains Player that changed.</param>
        /// <param name="changedProps">Contains the properties that changed.</param>
        void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps);

        /// <summary>
        /// Called after switching to a new MasterClient when the current one leaves.
        /// </summary>
        /// <remarks>
        /// This is not called when this client enters a room.
        /// The former MasterClient is still in the player list when this method get called.
        /// </remarks>
        void OnMasterClientSwitched(Player newMasterClient);
    }

    /// <summary>
    /// Collection of "organizational" callbacks for the Realtime Api to cover: Connection and Regions.
    /// </summary>
    /// <remarks>
    /// Classes that implement this interface must be registered to get callbacks for various situations.
    ///
    /// To register for callbacks, call <see cref="Client.AddCallbackTarget"/> and pass the class implementing this interface
    /// To stop getting callbacks, call <see cref="Client.RemoveCallbackTarget"/> and pass the class implementing this interface
    ///
    /// </remarks>
    /// \ingroup callbacks
    public interface IConnectionCallbacks
    {
        /// <summary>
        /// Called to signal that the "low level connection" got established but before the client can call operation on the server.
        /// </summary>
        /// <remarks>
        /// After the (low level transport) connection is established, the client will automatically send
        /// the Authentication operation, which needs to get a response before the client can call other operations.
        ///
        /// Your logic should wait for either: OnRegionListReceived or OnConnectedToMaster.
        ///
        /// This callback is useful to detect if the server can be reached at all (technically).
        /// Most often, it's enough to implement OnDisconnected(DisconnectCause cause) and check for the cause.
        ///
        /// This is not called for transitions from the masterserver to game servers.
        /// </remarks>
        void OnConnected();

        /// <summary>
        /// Called when the client is connected to the Master Server and ready for matchmaking and other tasks.
        /// </summary>
        /// <remarks>
        /// The list of available rooms won't become available unless you join a lobby via Client.OpJoinLobby.
        /// You can join rooms and create them even without being in a lobby. The default lobby is used in that case.
        /// </remarks>
        void OnConnectedToMaster();

        /// <summary>
        /// Called after disconnecting from the Photon server. It could be a failure or an explicit disconnect call
        /// </summary>
        /// <remarks>
        ///  The reason for this disconnect is provided as DisconnectCause.
        /// </remarks>
        void OnDisconnected(DisconnectCause cause);

        /// <summary>
        /// Called when the Name Server provided a list of regions for your title.
        /// </summary>
        /// <remarks>Check the RegionHandler class description, to make use of the provided values.</remarks>
        /// <param name="regionHandler">The currently used RegionHandler.</param>
        void OnRegionListReceived(RegionHandler regionHandler);


        /// <summary>
        /// Called when your Custom Authentication service responds with additional data.
        /// </summary>
        /// <remarks>
        /// Custom Authentication services can include some custom data in their response.
        /// When present, that data is made available in this callback as Dictionary.
        /// While the keys of your data have to be strings, the values can be either string or a number (in Json).
        /// You need to make extra sure, that the value type is the one you expect. Numbers become (currently) int64.
        ///
        /// Example: void OnCustomAuthenticationResponse(Dictionary&lt;string, object&gt; data) { ... }
        /// </remarks>
        /// <see cref="https://doc.photonengine.com/en-us/realtime/current/reference/custom-authentication"/>
        void OnCustomAuthenticationResponse(Dictionary<string, object> data);

        /// <summary>
        /// Called when the custom authentication failed. Followed by disconnect!
        /// </summary>
        /// <remarks>
        /// Custom Authentication can fail due to user-input, bad tokens/secrets.
        /// If authentication is successful, this method is not called. Implement OnJoinedLobby() or OnConnectedToMaster() (as usual).
        ///
        /// During development of a game, it might also fail due to wrong configuration on the server side.
        /// In those cases, logging the debugMessage is very important.
        ///
        /// Unless you setup a custom authentication service for your app (in the [Dashboard](https://dashboard.photonengine.com)),
        /// this won't be called!
        /// </remarks>
        /// <param name="debugMessage">Contains a debug message why authentication failed. This has to be fixed during development.</param>
        void OnCustomAuthenticationFailed(string debugMessage);

    }


    /// <summary>
    /// Collection of "organizational" callbacks for the Realtime Api to cover the Lobby.
    /// </summary>
    /// <remarks>
    /// Classes that implement this interface must be registered to get callbacks for various situations.
    ///
    /// To register for callbacks, call <see cref="Client.AddCallbackTarget"/> and pass the class implementing this interface
    /// To stop getting callbacks, call <see cref="Client.RemoveCallbackTarget"/> and pass the class implementing this interface
    ///
    /// </remarks>
    /// \ingroup callbacks
    public interface ILobbyCallbacks
    {

        /// <summary>
        /// Called on entering a lobby on the Master Server. The actual room-list updates will call OnRoomListUpdate.
        /// </summary>
        /// <remarks>
        /// While in the lobby, the roomlist is automatically updated in fixed intervals (which you can't modify in the public cloud).
        /// The room list gets available via OnRoomListUpdate.
        /// </remarks>
        void OnJoinedLobby();

        /// <summary>
        /// Called after leaving a lobby.
        /// </summary>
        /// <remarks>
        /// When you leave a lobby, [OpCreateRoom](@ref OpCreateRoom) and [OpJoinRandomRoom](@ref OpJoinRandomRoom)
        /// automatically refer to the default lobby.
        /// </remarks>
        void OnLeftLobby();

        /// <summary>
        /// Called for any update of the room-listing while in a lobby (InLobby) on the Master Server.
        /// </summary>
        /// <remarks>
        /// Each item is a RoomInfo which might include custom properties (provided you defined those as lobby-listed when creating a room).
        /// Not all types of lobbies provide a listing of rooms to the client. Some are silent and specialized for server-side matchmaking.
        /// </remarks>
        void OnRoomListUpdate(List<RoomInfo> roomList);

        /// <summary>
        /// Called when the Master Server sent an update for the Lobby Statistics.
        /// </summary>
        /// <remarks>
        /// This callback has two preconditions:
        /// EnableLobbyStatistics must be set to true, before this client connects.
        /// And the client has to be connected to the Master Server, which is providing the info about lobbies.
        /// </remarks>
        void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics);
    }


    /// <summary>
    /// Collection of "organizational" callbacks for the Realtime Api to cover Matchmaking.
    /// </summary>
    /// <remarks>
    /// Classes that implement this interface must be registered to get callbacks for various situations.
    ///
    /// To register for callbacks, call <see cref="Client.AddCallbackTarget"/> and pass the class implementing this interface
    /// To stop getting callbacks, call <see cref="Client.RemoveCallbackTarget"/> and pass the class implementing this interface
    ///
    /// </remarks>
    /// \ingroup callbacks
    public interface IMatchmakingCallbacks
    {

        /// <summary>
        /// Called when the server sent the response to a FindFriends request.
        /// </summary>
        /// <remarks>
        /// After calling OpFindFriends, the Master Server will cache the friend list and send updates to the friend
        /// list. The friends includes the name, userId, online state and the room (if any) for each requested user/friend.
        ///
        /// Use the friendList to update your UI and store it, if the UI should highlight changes.
        /// </remarks>
        void OnFriendListUpdate(List<FriendInfo> friendList);

        /// <summary>
        /// Called when this client created a room and entered it. OnJoinedRoom() will be called as well.
        /// </summary>
        /// <remarks>
        /// This callback is only called on the client which created a room (see OpCreateRoom).
        ///
        /// As any client might close (or drop connection) anytime, there is a chance that the
        /// creator of a room does not execute OnCreatedRoom.
        ///
        /// If you need specific room properties or a "start signal", implement OnMasterClientSwitched()
        /// and make each new MasterClient check the room's state.
        /// </remarks>
        void OnCreatedRoom();

        /// <summary>
        /// Called when the server couldn't create a room (OpCreateRoom failed).
        /// </summary>
        /// <remarks>
        /// Creating a room may fail for various reasons. Most often, the room already exists (roomname in use) or
        /// the RoomOptions clash and it's impossible to create the room.
        ///
        /// When creating a room fails on a Game Server:
        /// The client will cache the failure internally and returns to the Master Server before it calls the fail-callback.
        /// This way, the client is ready to find/create a room at the moment of the callback.
        /// In this case, the client skips calling OnConnectedToMaster but returning to the Master Server will still call OnConnected.
        /// Treat callbacks of OnConnected as pure information that the client could connect.
        /// </remarks>
        /// <param name="returnCode">Operation ReturnCode from the server.</param>
        /// <param name="message">Debug message for the error.</param>
        void OnCreateRoomFailed(short returnCode, string message);

        /// <summary>
        /// Called when the Client entered a room, no matter if this client created it or simply joined.
        /// </summary>
        /// <remarks>
        /// When this is called, you can access the existing players in Room.Players, their custom properties and Room.CustomProperties.
        ///
        /// In this callback, you could create player objects. For example in Unity, instantiate a prefab for the player.
        ///
        /// If you want a match to be started "actively", enable the user to signal "ready" (using OpRaiseEvent or a Custom Property).
        /// </remarks>
        void OnJoinedRoom();

        /// <summary>
        /// Called when a previous OpJoinRoom call failed on the server.
        /// </summary>
        /// <remarks>
        /// Joining a room may fail for various reasons. Most often, the room is full or does not exist anymore
        /// (due to someone else being faster or closing the room).
        ///
        /// When joining a room fails on a Game Server:
        /// The client will cache the failure internally and returns to the Master Server before it calls the fail-callback.
        /// This way, the client is ready to find/create a room at the moment of the callback.
        /// In this case, the client skips calling OnConnectedToMaster but returning to the Master Server will still call OnConnected.
        /// Treat callbacks of OnConnected as pure information that the client could connect.
        /// </remarks>
        /// <param name="returnCode">Operation ReturnCode from the server.</param>
        /// <param name="message">Debug message for the error.</param>
        void OnJoinRoomFailed(short returnCode, string message);

        /// <summary>
        /// Called when a previous OpJoinRandom call failed on the server.
        /// </summary>
        /// <remarks>
        /// The most common causes are that a room is full or does not exist (due to someone else being faster or closing the room).
        ///
        /// This operation is only ever sent to the Master Server. Once a room is found by the Master Server, the client will
        /// head off to the designated Game Server and use the operation Join on the Game Server.
        ///
        /// When using multiple lobbies (via OpJoinLobby or a TypedLobby parameter), another lobby might have more/fitting rooms.<br/>
        /// </remarks>
        /// <param name="returnCode">Operation ReturnCode from the server.</param>
        /// <param name="message">Debug message for the error.</param>
        void OnJoinRandomFailed(short returnCode, string message);

        /// <summary>
        /// Called when the local user/client left a room, so the game's logic can clean up it's internal state.
        /// </summary>
        /// <remarks>
        /// When leaving a room, the Client will disconnect the Game Server and connect to the Master Server.
        /// This wraps up multiple internal actions.
        ///
        /// Wait for the callback OnConnectedToMaster, before you use lobbies and join or create rooms.
        /// </remarks>
        void OnLeftRoom();
    }

    /// <summary>
    /// Event callback for the Realtime Api. Covers events from the server and those sent by clients via OpRaiseEvent.
    /// </summary>
    /// <remarks>
    /// Classes that implement this interface must be registered to get callbacks for various situations.
    ///
    /// To register for callbacks, call <see cref="Client.AddCallbackTarget"/> and pass the class implementing this interface
    /// To stop getting callbacks, call <see cref="Client.RemoveCallbackTarget"/> and pass the class implementing this interface
    ///
    /// </remarks>
    /// \ingroup callbacks
    public interface IOnEventCallback
    {
        /// <summary>Called for any incoming events.</summary>
        /// <remarks>
        /// To receive events, implement IOnEventCallback in any class and register it via AddCallbackTarget
        /// (either in Client or PhotonNetwork).
        ///
        /// With the EventData.Sender you can look up the Player who sent the event.
        ///
        /// It is best practice to assign an eventCode for each different type of content and action, so the Code
        /// will be essential to read the incoming events.
        /// </remarks>
        void OnEvent(EventData photonEvent);
    }

    /// <summary>
    /// Interface for "WebRpc" callbacks for the Realtime Api. Currently includes only responses for Web RPCs.
    /// </summary>
    /// <remarks>
    /// Classes that implement this interface must be registered to get callbacks for various situations.
    ///
    /// To register for callbacks, call <see cref="Client.AddCallbackTarget"/> and pass the class implementing this interface
    /// To stop getting callbacks, call <see cref="Client.RemoveCallbackTarget"/> and pass the class implementing this interface
    ///
    /// </remarks>
    /// \ingroup callbacks
    public interface IWebRpcCallback
    {
        /// <summary>
        /// Called when the response to a WebRPC is available. See <see cref="Client.OpWebRpc"/>.
        /// </summary>
        /// <remarks>
        /// Important: The response.ReturnCode is 0 if Photon was able to reach your web-service.<br/>
        /// The content of the response is what your web-service sent. You can create a WebRpcResponse from it.<br/>
        /// Example: WebRpcResponse webResponse = new WebRpcResponse(operationResponse);<br/>
        ///
        /// Please note: Class OperationResponse is in a namespace which needs to be "used":<br/>
        /// using ExitGames.Client.Photon;  // includes OperationResponse (and other classes)
        /// </remarks>
        /// <example>
        /// public void OnWebRpcResponse(OperationResponse response)
        /// {
        ///    Debug.LogFormat("WebRPC operation response {0}", response.ToStringFull());
        ///    switch (response.ReturnCode)
        ///    {
        ///        case ErrorCode.Ok:
        ///            WebRpcResponse webRpcResponse = new WebRpcResponse(response);
        ///            Debug.LogFormat("Parsed WebRPC response {0}", response.ToStringFull());
        ///            if (string.IsNullOrEmpty(webRpcResponse.Name))
        ///            {
        ///                Debug.LogError("Unexpected: WebRPC response did not contain WebRPC method name");
        ///            }
        ///            if (webRpcResponse.ResultCode == 0) // success
        ///            {
        ///                switch (webRpcResponse.Name)
        ///                {
        ///                    // todo: add your code here
        ///                 case GetGameListWebRpcMethodName: // example
        ///                    // ...
        ///                    break;
        ///             }
        ///            }
        ///            else if (webRpcResponse.ResultCode == -1)
        ///            {
        ///                Debug.LogErrorFormat("Web server did not return ResultCode for WebRPC method=\"{0}\", Message={1}", webRpcResponse.Name, webRpcResponse.Message);
        ///            }
        ///            else
        ///            {
        ///                Debug.LogErrorFormat("Web server returned ResultCode={0} for WebRPC method=\"{1}\", Message={2}", webRpcResponse.ResultCode, webRpcResponse.Name, webRpcResponse.Message);
        ///            }
        ///            break;
        ///        case ErrorCode.ExternalHttpCallFailed: // web service unreachable
        ///            Debug.LogErrorFormat("WebRPC call failed as request could not be sent to the server. {0}", response.DebugMessage);
        ///            break;
        ///        case ErrorCode.HttpLimitReached: // too many WebRPCs in a short period of time
        ///                                         // the debug message should contain the limit exceeded
        ///           Debug.LogErrorFormat("WebRPCs rate limit exceeded: {0}", response.DebugMessage);
        ///            break;
        ///       case ErrorCode.InvalidOperation: // WebRPC not configured at all OR not configured properly OR trying to send on name server
        ///          if (PhotonNetwork.Server == ServerConnection.NameServer)
        ///         {
        ///             Debug.LogErrorFormat("WebRPC not supported on NameServer. {0}", response.DebugMessage);
        ///         }
        ///         else
        ///         {
        ///             Debug.LogErrorFormat("WebRPC not properly configured or not configured at all. {0}", response.DebugMessage);
        ///         }
        ///         break;
        ///     default:
        ///         // other unknown error, unexpected
        ///         Debug.LogErrorFormat("Unexpected error, {0} {1}", response.ReturnCode, response.DebugMessage);
        ///         break;
        ///  }
        /// }
        ///
        /// </example>
        void OnWebRpcResponse(OperationResponse response);
    }

    /// <summary>
    /// Interface for <see cref="EventCode.ErrorInfo"/> event callback for the Realtime Api.
    /// </summary>
    /// <remarks>
    /// Classes that implement this interface must be registered to get callbacks for various situations.
    ///
    /// To register for callbacks, call <see cref="Client.AddCallbackTarget"/> and pass the class implementing this interface
    /// To stop getting callbacks, call <see cref="Client.RemoveCallbackTarget"/> and pass the class implementing this interface
    ///
    /// </remarks>
    /// \ingroup callbacks
    public interface IErrorInfoCallback
    {
        /// <summary>
        /// Called when the client receives an event from the server indicating that an error happened there.
        /// </summary>
        /// <remarks>
        /// In most cases this could be either:
        /// 1. an error from webhooks plugin (if HasErrorInfo is enabled), read more here:
        /// https://doc.photonengine.com/en-us/realtime/current/gameplay/web-extensions/webhooks#options
        /// 2. an error sent from a custom server plugin via PluginHost.BroadcastErrorInfoEvent, see example here:
        /// https://doc.photonengine.com/en-us/server/current/plugins/manual#handling_http_response
        /// 3. an error sent from the server, for example, when the limit of cached events has been exceeded in the room
        /// (all clients will be disconnected and the room will be closed in this case)
        /// read more here: https://doc.photonengine.com/en-us/realtime/current/gameplay/cached-events#special_considerations
        ///
        /// If you implement <see cref="IOnEventCallback.OnEvent"/> or <see cref="Client.EventReceived"/> you will also get this event.
        /// </remarks>
        /// <param name="errorInfo">Object containing information about the error</param>
        void OnErrorInfo(ErrorInfo errorInfo);
    }




    /// <summary>
    /// Container type for callbacks defined by IInRoomCallbacks. See InRoomCallbackTargets.
    /// </summary>
    /// <remarks>
    /// While the interfaces of callbacks wrap up the methods that will be called,
    /// the container classes implement a simple way to call a method on all registered objects.
    /// </remarks>
    internal class InRoomCallbacksContainer : List<IInRoomCallbacks>, IInRoomCallbacks
    {
        private readonly Client client;

        public InRoomCallbacksContainer(Client client)
        {
            this.client = client;
        }

        public void OnPlayerEnteredRoom(Player newPlayer)
        {
            this.client.UpdateCallbackTargets();

            foreach (IInRoomCallbacks target in this)
            {
                target.OnPlayerEnteredRoom(newPlayer);
            }
        }

        public void OnPlayerLeftRoom(Player otherPlayer)
        {
            this.client.UpdateCallbackTargets();

            foreach (IInRoomCallbacks target in this)
            {
                target.OnPlayerLeftRoom(otherPlayer);
            }
        }

        public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            this.client.UpdateCallbackTargets();

            foreach (IInRoomCallbacks target in this)
            {
                target.OnRoomPropertiesUpdate(propertiesThatChanged);
            }
        }

        public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProp)
        {
            this.client.UpdateCallbackTargets();

            foreach (IInRoomCallbacks target in this)
            {
                target.OnPlayerPropertiesUpdate(targetPlayer, changedProp);
            }
        }

        public void OnMasterClientSwitched(Player newMasterClient)
        {
            this.client.UpdateCallbackTargets();

            foreach (IInRoomCallbacks target in this)
            {
                target.OnMasterClientSwitched(newMasterClient);
            }
        }
    }

    /// <summary>
    /// Container type for callbacks defined by IConnectionCallbacks. See LoadBalancingCallbackTargets.
    /// </summary>
    /// <remarks>
    /// While the interfaces of callbacks wrap up the methods that will be called,
    /// the container classes implement a simple way to call a method on all registered objects.
    /// </remarks>
    public class ConnectionCallbacksContainer : List<IConnectionCallbacks>, IConnectionCallbacks
    {
        private readonly Client client;

        public ConnectionCallbacksContainer(Client client)
        {
            this.client = client;
        }

        public void OnConnected()
        {
            this.client.UpdateCallbackTargets();

            foreach (IConnectionCallbacks target in this)
            {
                target.OnConnected();
            }
        }

        public void OnConnectedToMaster()
        {
            this.client.UpdateCallbackTargets();

            foreach (IConnectionCallbacks target in this)
            {
                target.OnConnectedToMaster();
            }
        }

        public void OnRegionListReceived(RegionHandler regionHandler)
        {
            this.client.UpdateCallbackTargets();

            foreach (IConnectionCallbacks target in this)
            {
                target.OnRegionListReceived(regionHandler);
            }
        }

        public void OnDisconnected(DisconnectCause cause)
        {
            this.client.UpdateCallbackTargets();

            foreach (IConnectionCallbacks target in this)
            {
                target.OnDisconnected(cause);
            }
        }

        public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
        {
            this.client.UpdateCallbackTargets();

            foreach (IConnectionCallbacks target in this)
            {
                target.OnCustomAuthenticationResponse(data);
            }
        }

        public void OnCustomAuthenticationFailed(string debugMessage)
        {
            this.client.UpdateCallbackTargets();

            foreach (IConnectionCallbacks target in this)
            {
                target.OnCustomAuthenticationFailed(debugMessage);
            }
        }
    }

    /// <summary>
    /// Container type for callbacks defined by IMatchmakingCallbacks. See MatchMakingCallbackTargets.
    /// </summary>
    /// <remarks>
    /// While the interfaces of callbacks wrap up the methods that will be called,
    /// the container classes implement a simple way to call a method on all registered objects.
    /// </remarks>
    public class MatchMakingCallbacksContainer : List<IMatchmakingCallbacks>, IMatchmakingCallbacks
    {
        private readonly Client client;

        public MatchMakingCallbacksContainer(Client client)
        {
            this.client = client;
        }

        public void OnCreatedRoom()
        {
            this.client.UpdateCallbackTargets();

            foreach (IMatchmakingCallbacks target in this)
            {
                target.OnCreatedRoom();
            }
        }

        public void OnJoinedRoom()
        {
            this.client.UpdateCallbackTargets();

            foreach (IMatchmakingCallbacks target in this)
            {
                target.OnJoinedRoom();
            }
        }

        public void OnCreateRoomFailed(short returnCode, string message)
        {
            this.client.UpdateCallbackTargets();

            foreach (IMatchmakingCallbacks target in this)
            {
                target.OnCreateRoomFailed(returnCode, message);
            }
        }

        public void OnJoinRandomFailed(short returnCode, string message)
        {
            this.client.UpdateCallbackTargets();

            foreach (IMatchmakingCallbacks target in this)
            {
                target.OnJoinRandomFailed(returnCode, message);
            }
        }

        public void OnJoinRoomFailed(short returnCode, string message)
        {
            this.client.UpdateCallbackTargets();

            foreach (IMatchmakingCallbacks target in this)
            {
                target.OnJoinRoomFailed(returnCode, message);
            }
        }

        public void OnLeftRoom()
        {
            this.client.UpdateCallbackTargets();

            foreach (IMatchmakingCallbacks target in this)
            {
                target.OnLeftRoom();
            }
        }

        public void OnFriendListUpdate(List<FriendInfo> friendList)
        {
            this.client.UpdateCallbackTargets();

            foreach (IMatchmakingCallbacks target in this)
            {
                target.OnFriendListUpdate(friendList);
            }
        }
    }

    /// <summary>
    /// Container type for callbacks defined by ILobbyCallbacks. See LobbyCallbackTargets.
    /// </summary>
    /// <remarks>
    /// While the interfaces of callbacks wrap up the methods that will be called,
    /// the container classes implement a simple way to call a method on all registered objects.
    /// </remarks>
    internal class LobbyCallbacksContainer : List<ILobbyCallbacks>, ILobbyCallbacks
    {
        private readonly Client client;

        public LobbyCallbacksContainer(Client client)
        {
            this.client = client;
        }

        public void OnJoinedLobby()
        {
            this.client.UpdateCallbackTargets();

            foreach (ILobbyCallbacks target in this)
            {
                target.OnJoinedLobby();
            }
        }

        public void OnLeftLobby()
        {
            this.client.UpdateCallbackTargets();

            foreach (ILobbyCallbacks target in this)
            {
                target.OnLeftLobby();
            }
        }

        public void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            this.client.UpdateCallbackTargets();

            foreach (ILobbyCallbacks target in this)
            {
                target.OnRoomListUpdate(roomList);
            }
        }

        public void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
        {
            this.client.UpdateCallbackTargets();

            foreach (ILobbyCallbacks target in this)
            {
                target.OnLobbyStatisticsUpdate(lobbyStatistics);
            }
        }
    }

    /// <summary>
    /// Container type for callbacks defined by IWebRpcCallback. See WebRpcCallbackTargets.
    /// </summary>
    /// <remarks>
    /// While the interfaces of callbacks wrap up the methods that will be called,
    /// the container classes implement a simple way to call a method on all registered objects.
    /// </remarks>
    internal class WebRpcCallbacksContainer : List<IWebRpcCallback>, IWebRpcCallback
    {
        private Client client;

        public WebRpcCallbacksContainer(Client client)
        {
            this.client = client;
        }

        public void OnWebRpcResponse(OperationResponse response)
        {
            this.client.UpdateCallbackTargets();

            foreach (IWebRpcCallback target in this)
            {
                target.OnWebRpcResponse(response);
            }
        }
    }


    /// <summary>
    /// Container type for callbacks defined by <see cref="IErrorInfoCallback"/>. See <see cref="Client.ErrorInfoCallbackTargets"/>.
    /// </summary>
    /// <remarks>
    /// While the interfaces of callbacks wrap up the methods that will be called,
    /// the container classes implement a simple way to call a method on all registered objects.
    /// </remarks>
    internal class ErrorInfoCallbacksContainer : List<IErrorInfoCallback>, IErrorInfoCallback
    {
        private Client client;

        public ErrorInfoCallbacksContainer(Client client)
        {
            this.client = client;
        }

        public void OnErrorInfo(ErrorInfo errorInfo)
        {
            this.client.UpdateCallbackTargets();
            foreach (IErrorInfoCallback target in this)
            {
                target.OnErrorInfo(errorInfo);
            }
        }
    }

    /// <summary>
    /// Class wrapping the received <see cref="EventCode.ErrorInfo"/> event.
    /// </summary>
    /// <remarks>
    /// This is passed inside <see cref="IErrorInfoCallback.OnErrorInfo"/> callback.
    /// If you implement <see cref="IOnEventCallback.OnEvent"/> or <see cref="Client.EventReceived"/> you will also get <see cref="EventCode.ErrorInfo"/> but not parsed.
    ///
    /// In most cases this could be either:
    /// 1. an error from webhooks plugin (if HasErrorInfo is enabled), read more here:
    /// https://doc.photonengine.com/en-us/realtime/current/gameplay/web-extensions/webhooks#options
    /// 2. an error sent from a custom server plugin via PluginHost.BroadcastErrorInfoEvent, see example here:
    /// https://doc.photonengine.com/en-us/server/current/plugins/manual#handling_http_response
    /// 3. an error sent from the server, for example, when the limit of cached events has been exceeded in the room
    /// (all clients will be disconnected and the room will be closed in this case)
    /// read more here: https://doc.photonengine.com/en-us/realtime/current/gameplay/cached-events#special_considerations
    /// </remarks>
    public class ErrorInfo
    {
        /// <summary>
        /// String containing information about the error.
        /// </summary>
        public readonly string Info;

        public ErrorInfo(EventData eventData)
        {
            this.Info = eventData[ParameterCode.Info] as string;
        }

        public override string ToString()
        {
            return string.Format("ErrorInfo: {0}", this.Info);
        }
    }
}
