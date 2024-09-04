// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HiveApplication.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Main photon application. This application is started from the photon server.
//   This class creates <see cref="LitePeer" />s for new clients.
//   Operation dispatch logic is handled by the <see cref="LitePeer" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Photon.Hive.Common;
using Photon.Hive.Plugin;
using Photon.Hive.WebRpc;
using Photon.Hive.WebRpc.Configuration;

namespace Photon.Hive
{
    using System.Collections.Generic;
    using System.IO;
    using ExitGames.Logging;
    using ExitGames.Logging.Log4Net;
    using Photon.Hive.Diagnostics;
    using log4net.Config;
    using Photon.SocketServer;
    using Photon.SocketServer.Diagnostics;

    /// <summary>
    /// Main photon application. This application is started from the photon server.
    /// This class creates <see cref="HivePeer"/>s for new clients.
    /// Operation dispatch logic is handled by the <see cref="HivePeer"/>. 
    /// </summary>
    public class HiveApplication : ApplicationBase
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly WebRpcManager webRpcManager;
        public HiveApplication()
        : base(LoadConfiguration())
        {
            var env = new Dictionary<string, object>
            {
                {"AppId", this.HwId},
                {"AppVersion", ""},
                {"Region", ""},
                {"Cloud", ""},
            };


            this.webRpcManager = new WebRpcManager(env);
        }

        /// <summary>
        /// Creates a <see cref="HivePeer"/> to handle <see cref="OperationRequest"/>s.
        /// </summary>
        /// <param name="initRequest">
        /// The initialization request.
        /// </param>
        /// <returns>
        /// A new <see cref="HivePeer"/> instance.
        /// </returns>
        protected override PeerBase CreatePeer(InitRequest initRequest)
        {
            var peer = new HivePeer(initRequest);

            if (this.webRpcManager.IsRpcEnabled)
            {
                peer.WebRpcHandler = this.webRpcManager.GetWebRpcHandler();
            }

            return peer;
        }

        /// <summary>
        /// Application initialization.
        /// </summary>
        protected override void Setup()
        {
            log4net.GlobalContext.Properties["Photon:ApplicationLogPath"] = Path.Combine(this.ApplicationRootPath, "log");

            // log4net
            string path = Path.Combine(this.BinaryPath, "log4net.config");
            var file = new FileInfo(path);
            if (file.Exists)
            {
                LogManager.SetLoggerFactory(Log4NetLoggerFactory.Instance);
                var logRepository = log4net.LogManager.GetRepository(Assembly.GetEntryAssembly());
                XmlConfigurator.ConfigureAndWatch(logRepository, file);
            }

            log.InfoFormat("Created application Instance: type={0}", Instance.GetType());

            this.InitCorePerformanceCounters();
            Initialize();
        }

        protected void Initialize()
        {
            Protocol.AllowRawCustomValues = true;
        }

        /// <summary>
        /// Called when the server shuts down.
        /// </summary>
        protected override void TearDown()
        {
        }

        private static IConfiguration LoadConfiguration()
        {
            var cb = new ConfigurationBuilder();
            var cbpath = Path.GetDirectoryName(typeof(HiveApplication).Assembly.Location);
            cb.AddXmlFile(Path.Combine(cbpath, "Hive.xml.config"));
            return cb.Build();
        }

    }
}