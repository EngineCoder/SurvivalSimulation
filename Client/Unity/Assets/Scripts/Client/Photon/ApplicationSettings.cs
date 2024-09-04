
namespace UFO.Realtime
{
    using System;
    using ExitGames.Client.Photon;
    using static UFO.Realtime.Peer;


#if !NETFX_CODE || SUPPORTED_UNITY
    [Serializable]
#endif
    public class ApplicationSettings
    {
        #region Photon Settings

        public string appIdRealtime;
        public string appIdChat;
        public string appIdVoice;

        public string appVersion; //应用程序版本

        public bool useNameServer = true;//如果为false，则应用程序将尝试连接到主服务器（这已作废，但有时仍是必需的）。
                                         //如果为true，则Server指向NameServer（或使用默认值，为null），否则指向MasterServer。

        public string fixedRegion;//可以设置为任何Photon Cloud的区域名称以直接连接到该区域。
        public string bestRegionSummaryFromStorage;//最佳存储区域摘要

        #endregion

        #region Client Settings

        #endregion
#if SUPPORTED_UNITY
        [NonSerialized]
#endif

        public string server;//要连接的服务器的地址（主机名或IP）。
        public int port;//如果不为null，则设置要连接的第一个Photon服务器的端口（将根据需要“转发”客户端）。
        public string proxyServer;//代理服务器的地址（主机名或IP和端口）。
        public ConnectionProtocol protocol = ConnectionProtocol.Udp;//要使用的网络级别协议。

        public bool enableProtocolFallback = true;//Enables a fallback to another protocol in case a connect to the Name Server fails.
                                                  //See: LoadBalancingClient.EnableProtocolFallback.

        public AuthModeOption authMode = AuthModeOption.Auth;//Defines how authentication is done. On each system, once or once via a WSS connection (safe).
        /// <summary>Defines how the communication gets encrypted.</summary>
        public EncryptionMode EncryptionMode = EncryptionMode.PayloadEncryption;

        public bool enableLobbyStatistics;//启用大厅统计，If true, the client will request the list of currently available lobbies.

        public DebugLevel networkLogging = DebugLevel.ERROR;//日志

        public bool IsMaterServerAddress { get { return !this.useNameServer; } }//如果为true，则“服务器”字段包含主服务器地址（如果有的话）。

        public bool IsBestRegion { get { return this.useNameServer && string.IsNullOrEmpty(this.fixedRegion); } }//如果为true，则客户端应从名称服务器获取区域列表，并找到具有最佳ping的列表。See "Best Region" in the online docs.

        /// <summary>If true, the default nameserver address for the Photon Cloud should be used.</summary>
        public bool IsDefaultNameServer { get { return this.useNameServer && string.IsNullOrEmpty(this.server); } }//如果为true，则应使用Photon Cloud的默认名称服务器地址。

        /// <summary>If true, the default ports for a protocol will be used.</summary>
        public bool IsDefaultPort { get { return this.port <= 0; } }//如果为true，将使用协议的默认端口。


        public string ToStringFull()//更多详细信息。
        {
            return string.Format(
               "appId {0}{1}{2}{3}" +
               "use ns: {4}, reg: {5}, {9}, " +
               "{6}{7}{8}" +
               "auth: {10}",
               String.IsNullOrEmpty(this.appIdRealtime) ? string.Empty : "rt: " + this.HideAppId(this.appIdRealtime) + ", ",
               String.IsNullOrEmpty(this.appIdChat) ? string.Empty : "chat: " + this.HideAppId(this.appIdChat) + ", ",
               String.IsNullOrEmpty(this.appIdVoice) ? string.Empty : "voice: " + this.HideAppId(this.appIdVoice) + ", ",
               String.IsNullOrEmpty(this.appVersion) ? string.Empty : "appV: " + this.appVersion + ", ",
               this.useNameServer,
               this.IsBestRegion ? "/best/" : this.fixedRegion,
               //this.BestRegionSummaryFromStorage,
               String.IsNullOrEmpty(this.server) ? string.Empty : "server: " + this.server + ", ",
               this.IsDefaultPort ? string.Empty : "port: " + this.port + ", ",
               String.IsNullOrEmpty(proxyServer) ? string.Empty : "proxy: " + this.proxyServer + ", ",
               this.protocol,
               this.authMode
           //this.EnableLobbyStatistics,
           //this.NetworkLogging,
           );
        }

        private string HideAppId(string appId)
        {
            return string.IsNullOrEmpty(appId) || appId.Length < 8
                ? appId
                : string.Concat(appId.Substring(0, 8), "***");
        }


        public ApplicationSettings CopyTo(ApplicationSettings appSettings)
        {
            appSettings.appIdRealtime = this.appIdRealtime;
            appSettings.appIdChat = this.appIdChat;
            appSettings.appIdVoice = this.appIdVoice;
            appSettings.appVersion = this.appVersion;
            appSettings.useNameServer = this.useNameServer;
            appSettings.fixedRegion = this.fixedRegion;
            appSettings.bestRegionSummaryFromStorage = this.bestRegionSummaryFromStorage;
            appSettings.server = this.server;
            appSettings.port = this.port;
            appSettings.proxyServer = this.proxyServer;
            appSettings.protocol = this.protocol;
            appSettings.authMode = this.authMode;
            appSettings.enableLobbyStatistics = this.enableLobbyStatistics;
            appSettings.networkLogging = this.networkLogging;
            appSettings.enableProtocolFallback = this.enableProtocolFallback;
            return appSettings;
        }
    }
}


