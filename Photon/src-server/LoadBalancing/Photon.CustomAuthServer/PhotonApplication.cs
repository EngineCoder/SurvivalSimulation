using System.IO;
using System.Reflection;
using ExitGames.Logging;
using ExitGames.Logging.Log4Net;

using log4net;
using log4net.Config;

using Microsoft.Extensions.Configuration;

using Photon.SocketServer;

using LogManager = ExitGames.Logging.LogManager;

namespace Photon.CustomAuthServer
{
    /// <summary>
    /// to not deal with external services that may close we created this very basic and simple application that
    /// handles very basic custom auth request. It can be used as sample
    /// </summary>
    public class PhotonApplication : ApplicationBase
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly CustomAuthHttpListener listener = new CustomAuthHttpListener(false);

        static PhotonApplication()
        {
            LogManager.SetLoggerFactory(Log4NetLoggerFactory.Instance);
        }

        public PhotonApplication() : base(new ConfigurationBuilder().Build())
        {
        }

        protected override PeerBase CreatePeer(InitRequest initRequest)
        {
            return null;
        }

        protected override void Setup()
        {
            this.SetupLog4net();

            log.Info("Starting http listener");
            this.listener.Start();
            log.Info("Custom auth server started");
        }

        protected override void TearDown()
        {
            log.Info("Custom auth server stopped");
        }

        private void SetupLog4net()
        {
            // Rolling Logfile Appender: 
            GlobalContext.Properties["Photon:ApplicationLogPath"] = Path.Combine(this.ApplicationRootPath, "log");
            GlobalContext.Properties["Photon:UnmanagedLogDirectory"] = this.UnmanagedLogPath;
            GlobalContext.Properties["LogFileName"] = this.ApplicationName;

#if NETSTANDARD2_0 || NETCOREAPP
            var logRepository = log4net.LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.ConfigureAndWatch(logRepository, new FileInfo(Path.Combine(this.BinaryPath, "log4net.config")));
#else
            XmlConfigurator.ConfigureAndWatch(new FileInfo(Path.Combine(this.BinaryPath, "log4net.config")));
#endif
        }

    }
}
