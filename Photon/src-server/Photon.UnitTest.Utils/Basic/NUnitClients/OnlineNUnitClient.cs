//// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OnlineNUnitClient.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Plain Photon Client for online tests
// </summary>
// --------------------------------------------------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

using ExitGames.Client.Photon;
using ExitGames.Client.Photon.Encryption;
using ExitGames.Concurrency.Fibers;
using ExitGames.Logging;
using ExitGames.Threading;

using Photon.SocketServer;
using Photon.SocketServer.Collections;
using Photon.SocketServer.Security;

using EventData = ExitGames.Client.Photon.EventData;
using OperationRequest = ExitGames.Client.Photon.OperationRequest;
using OperationResponse = ExitGames.Client.Photon.OperationResponse;

namespace Photon.UnitTest.Utils.Basic.NUnitClients
{
    /// <summary>
    ///   The test client.
    /// </summary>
    public class OnlineNUnitClient : INUnitClient, IPhotonPeerListener
    {
        #region Constants and Fields

        /// <summary>
        ///   The log.
        /// </summary>
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private static int currentId;

        /// <summary>
        ///   The auto reset event init.
        /// </summary>
        private readonly AutoResetEvent autoResetEventDisconnect;
        private readonly AutoResetEvent autoResetEventDisconnectMsg;
        private readonly AutoResetEvent autoResetEventConnect;
        private readonly AutoResetEvent autoResetEventEncrypt;

        /// <summary>
        ///   The event queue.
        /// </summary>
        private WaitableQueue<EventData> EventQueue { get; }

        private bool enableOnMessageReceiveBuffer;
        public bool EnableOnMessageReceiveBuffer
        {
            get => this.enableOnMessageReceiveBuffer;
            set
            {
                if (value && this.OnMessageBuffer == null)
                {
                    this.OnMessageBuffer = new Dictionary<long, object>();
                }

                this.enableOnMessageReceiveBuffer = value;
            }
        }
        public Dictionary<long, object> OnMessageBuffer { get; set; }


        /// <summary>
        ///   The fiber.
        /// </summary>
        private readonly PoolFiber fiber = new PoolFiber(new FailSafeBatchExecutor());

        /// <summary>
        ///   The operation response queue.
        /// </summary>
        private WaitableQueue<OperationResponse> OperationResponseQueue { get; }

        // TODO: currently the client is sending debug messages after 
        // it is disconnected. The logging for this client may went to the next
        // test currently running. 
        // The Resharper 8 nunit test plugin for VisualStudio for example hangs if 
        // logging comes from a test not currently running.
        public readonly bool LogPhotonClientMessages;

        /// <summary>
        ///   The service.
        /// </summary>
        private long service;

        #endregion

        #region Constructors and Destructors

        public OnlineNUnitClient(ConnectPolicy policy, bool logPhotonClientMessages = false)
            : this(policy.Protocol, logPhotonClientMessages)
        {
            this.Policy = policy;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "OnlineNUnitClient" /> class.
        /// </summary>
        public OnlineNUnitClient(ConnectionProtocol connectionProtocol, bool logPhotonClientMessages = false)
        {
            this.Id = Interlocked.Increment(ref currentId);
            this.LogPhotonClientMessages = logPhotonClientMessages;

            this.autoResetEventDisconnect = new AutoResetEvent(false);
            this.autoResetEventDisconnectMsg = new AutoResetEvent(false);
            this.autoResetEventConnect = new AutoResetEvent(false);
            this.autoResetEventEncrypt = new AutoResetEvent(false);

            this.OperationResponseQueue = new WaitableQueue<OperationResponse>();
            this.EventQueue = new WaitableQueue<EventData>();

            this.Peer = new PhotonPeer(this, connectionProtocol)
            {
                DebugOut = DebugLevel.ALL,
                SerializationProtocolType = SerializationProtocol.GpBinaryV18,
            };
            this.Peer.OnDisconnectMessage += this.OnDisconnectMessage;

            if (log.IsInfoEnabled)
            {
                log.Info($"-- Peer Created. peerId:{this.Peer.PeerID}, clientId={this.Id}");
            }

            ServicePointManager.ServerCertificateValidationCallback = this.ValidateServerCert;

            this.fiber.Start();
        }

        #endregion

        #region Events

        /// <summary>
        ///   The connected.
        /// </summary>
        public static event Action<OnlineNUnitClient> Connected;

        /// <summary>
        ///   The disconnected.
        /// </summary>
        public static event Action<OnlineNUnitClient> Disconnected;

        /// <summary>
        ///   The event received.
        /// </summary>
        public static event Action<OnlineNUnitClient, EventData> EventReceived;

        /// <summary>
        ///   The response received.
        /// </summary>
        public static event Action<OnlineNUnitClient, OperationResponse> ResponseReceived;

        #endregion

        #region Properties

        public NetworkProtocolType NetworkProtocol => (NetworkProtocolType)this.Peer.UsedProtocol;

        public ConnectPolicy Policy { get; }

        public int Id { get; }

        /// <summary>
        ///   Gets the underling <see cref = "Peer" />.
        /// </summary>
        public PhotonPeer Peer { get; }

#pragma warning disable CS3003 // Type is not CLS-compliant
        public int DisconnectError { get; private set; }
#pragma warning restore CS3003 // Type is not CLS-compliant

        public string DisconnectErrorDetails { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        ///   The close.
        /// </summary>
        public void CloseConnection(int waitForDisconnect = ConnectPolicy.WaitTime)
        {
            this.Disconnect();
            this.WaitForDisconnect(waitForDisconnect);
            this.StopService();
            this.EventQueue.Clear();
            this.OperationResponseQueue.Clear();
        }

        public void Disconnect()
        {
            this.autoResetEventDisconnect.Reset();
            this.fiber.Enqueue(
                () =>
                {
                    if (log.IsDebugEnabled)
                    {
                        log.DebugFormat("Disconnecting peer. pid:{0}, cid:{1}, ConnState:{2}", this.Peer.PeerID, this.Id, this.Peer.PeerState);
                    }
                    this.Peer.Disconnect();
                });
        }

        /// <summary>
        ///   The connect.
        /// </summary>
        /// <param name = "address">
        ///     The server address (format:  IP:Port or http://hostname:port, depending on protocol)
        /// </param>
        /// <param name = "applicationId">
        ///     The application Id.
        /// </param>
        /// <param name="token">authentication token</param>
        /// <param name="custom"></param>
        public void Connect(string address, string applicationId, byte[] token = null, object custom = null)
        {
            this.autoResetEventConnect.Reset();
            this.fiber.Enqueue(
                () =>
                {
                    if (log.IsInfoEnabled)
                    {
                        log.Info($"--Connecting too {address}. peerId:{this.Peer.PeerID}, clientId={this.Id}, ts:{DateTime.Now}, {this.Peer.PeerState}");
                    }
                    this.Peer.Connect(address, applicationId, custom);
                    this.StartService();
                });
        }

        /// <summary>
        ///   The send operation request.
        /// </summary>
        /// <param name = "operationRequest">
        ///   The operation request.
        /// </param>
        public bool SendOperationRequest(OperationRequest operationRequest)
        {
            NUnit.Framework.Assert.IsTrue(
                this.Peer.SendOperation(
                    operationRequest.OperationCode,
                    operationRequest.Parameters,
                    new SendOptions { Reliability = true }),
                    "Operation not sent.");
            return true;
        }

        /// <summary>
        ///   The wait for connect.
        /// </summary>
        /// <param name = "millisecondsWaitTime">
        ///   The milliseconds wait time.
        /// </param>
        /// <returns>
        ///   true if connected.
        /// </returns>
        public bool WaitForConnect(int millisecondsWaitTime)
        {
            return this.autoResetEventConnect.WaitOne(millisecondsWaitTime);
        }

        public bool WaitForDisconnect(int millisecondsWaitTime)
        {
            return this.autoResetEventDisconnect.WaitOne(millisecondsWaitTime);
        }

        public bool WaitForDisconnectMessage(int millisecondsWaitTime = ConnectPolicy.WaitTime)
        {
            return this.autoResetEventDisconnectMsg.WaitOne(millisecondsWaitTime);
        }

        /// <summary>
        ///   The wait for event.
        /// </summary>
        /// <param name = "millisecondsWaitTime">
        ///   The milliseconds wait time.
        /// </param>
        /// <returns>
        ///   the event
        /// </returns>
        public EventData WaitForEvent(int millisecondsWaitTime)
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("Peer({0}) waits for event. Events in queue:{1}", this.Id, this.EventQueue.Count);
            }
            return this.EventQueue.Dequeue(millisecondsWaitTime);
        }

        public EventData WaitEvent(byte eventCode, int millisecondsWaitTime = ConnectPolicy.WaitTime)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug($"Waiting for event {eventCode}, clientId={this.Id}");
            }
            return this.EventQueue.DequeueIf(data => data.Code == eventCode, millisecondsWaitTime);
        }

        /// <summary>
        ///   The wait for operation response.
        /// </summary>
        /// <param name = "millisecondsWaitTime">
        ///   The milliseconds wait time.
        /// </param>
        /// <returns>
        ///   the response
        /// </returns>
        public OperationResponse WaitForOperationResponse(int millisecondsWaitTime)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug($"Waiting for response. Responses in queue:{this.EventQueue.Count}, clientId={this.Id}");
            }
            return this.OperationResponseQueue.Dequeue(millisecondsWaitTime);
        }

        public bool SendRequest(OperationRequest op, bool encrypted)
        {
            return this.Peer.SendOperation(op.OperationCode, op.Parameters,
                new SendOptions { Channel = 0, Reliability = true, Encrypt = encrypted });
        }

        #endregion

        #region Implemented Interfaces

        #region IDisposable

        /// <summary>
        ///   The dispose.
        /// </summary>
        public void Dispose()
        {
            this.fiber.Enqueue(
                () =>
                {
                    this.Peer.Disconnect();
                    this.StopService();
                    this.fiber.Dispose();
                });
        }

        #endregion

        #region IPhotonPeerListener

        /// <summary>
        ///   The debug return.
        /// </summary>
        /// <param name = "debugLevel">
        ///   The debug Level.
        /// </param>
        /// <param name = "debug">
        ///   The debug message.
        /// </param>
        public void DebugReturn(DebugLevel debugLevel, string debug)
        {
            if (this.LogPhotonClientMessages == false)
            {
                return;
            }

            switch (debugLevel)
            {
                case DebugLevel.ALL:
                    if (log.IsDebugEnabled)
                    {
                        log.DebugFormat("DebugReturn({2}): id={0}, msg={1}", this.Id, debug, debugLevel);
                    }
                    break;

                case DebugLevel.INFO:
                    log.InfoFormat("DebugReturn({2}): id={0}, msg={1}", this.Id, debug, debugLevel);
                    break;

                case DebugLevel.WARNING:
                    log.WarnFormat("DebugReturn({2}): id={0}, msg={1}", this.Id, debug, debugLevel);
                    break;

                case DebugLevel.ERROR:
                    log.ErrorFormat("DebugReturn({2}): id={0}, msg={1}", this.Id, debug, debugLevel);
                    break;
            }
        }

        public void OnEvent(EventData @event)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug($"Peer({this.Id}) got event: {Newtonsoft.Json.JsonConvert.SerializeObject(@event)}");//, 
            }

            this.EventQueue.Enqueue(@event);

            OnEventReceived(this, @event);
        }

        public void OnOperationResponse(OperationResponse operationResponse)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug($"Peer({this.Id}) got OperationResponse: {Newtonsoft.Json.JsonConvert.SerializeObject(operationResponse)}");
            }

            this.OperationResponseQueue.Enqueue(operationResponse);

            OnResponseReceived(this, operationResponse);
        }

        public void OnMessage(object messages)
        {
            if(!this.EnableOnMessageReceiveBuffer)
            {
                return;
            }

            this.OnMessageBuffer.Add(DateTime.Now.Ticks, messages);
        }

        public void SendMessage(object message)
        {
            this.Peer.SendMessage(message, new SendOptions { Reliability = true, Channel = 0, Encrypt = false });
        }

        public void OnStatusChanged(StatusCode returnCode)
        {
            switch (returnCode)
            {
                case StatusCode.Connect:
                    {
                        if (log.IsInfoEnabled)
                        {
                            log.Info($"--Connected. peerId:{this.Peer.PeerID}, clientId={this.Id}");
                        }
                        this.autoResetEventConnect.Set();
                        OnConnected(this);
                        break;
                    }

                case StatusCode.Disconnect:
                    {
                        if (log.IsInfoEnabled)
                        {
                            log.Info($"--Disconnected.  peerId:{this.Peer.PeerID}, clientId={this.Id}");
                        }
                        this.autoResetEventDisconnect.Set();
                        OnDisconnected(this);
                        break;
                    }

                case StatusCode.DisconnectByServerLogic:
                case StatusCode.DisconnectByServerUserLimit:
                    {
                        if (log.IsWarnEnabled)
                        {
                            log.Warn($"--DisconnectByServerLogic or DisconnectByServerUserLimit.  peerId:{this.Peer.PeerID}, clientId={this.Id}");
                        }
                        this.autoResetEventDisconnect.Set();
                        OnDisconnected(this);
                        break;
                    }

                case StatusCode.EncryptionEstablished:
                case StatusCode.EncryptionFailedToEstablish:
                {
                    if (log.IsWarnEnabled)
                    {
                        log.Warn($"--EncryptionEstablished or EncryptionFailedToEstablish. peerId:{this.Peer.PeerID}, clientId={this.Id}");
                    }
                    this.autoResetEventEncrypt.Set();
                    break;
                }
                case StatusCode.TimeoutDisconnect:
                {
                    if (log.IsWarnEnabled)
                    {
                        log.Warn($"--TimeoutDisconnect. peerId:{this.Peer.PeerID}, clientId={this.Id}");
                    }
                    this.autoResetEventDisconnect.Set();
                    break;
                }
                default:
                    {
                        if (log.IsWarnEnabled)
                        {
                            log.Warn($"--unknown {returnCode}. peerId:{this.Peer.PeerID}, clientId={this.Id}");
                        }
                        log.Warn(returnCode);
                        break;
                    }
            }
        }

        #endregion

        #region INUnitClient

        bool INUnitClient.Connected => this.Peer.PeerState == PeerStateValue.Connected;

        public string RemoteEndPoint => this.Peer.ServerAddress;

        public void EventQueueClear()
        {
            this.EventQueue.Clear();
        }

        public void OperationResponseQueueClear()
        {
            this.OperationResponseQueue.Clear();
        }

        public void InitEncryption()
        {
            this.autoResetEventEncrypt.Reset();
            this.Peer.EstablishEncryption();
            this.autoResetEventEncrypt.WaitOne(3000);
        }

        void INUnitClient.Connect(string address, byte[] token, object custom)
        {
            if (this.Policy != null)
            {
                this.Policy.ConnectToServer(this, address, custom);
            }
            else
            {
                throw new Exception("Policy is not set");
            }
        }
        #endregion

        #endregion

        #region Methods

        private bool ValidateServerCert(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslpolicyerrors)
        {
            return true;
        }

        private void OnDisconnectMessage(DisconnectMessage msg)
        {
            this.DisconnectErrorDetails = msg.DebugMessage;
            this.DisconnectError = msg.Code;

            this.autoResetEventDisconnectMsg.Set();

            this.Disconnect();
        }

        /// <summary>
        ///   The log operation response.
        /// </summary>
        /// <param name = "response">
        ///   The response.
        /// </param>
        private static void LogOperationResponse(OperationResponse response)
        {
            foreach (var item in response.Parameters)
            {
                log.DebugFormat("{0}({1}): {2}", item.Key, item.Key, item.Value);
            }
        }

        /// <summary>
        ///   The on connected.
        /// </summary>
        /// <param name = "client">
        ///   The client.
        /// </param>
        private static void OnConnected(OnlineNUnitClient client)
        {
            Action<OnlineNUnitClient> connected = Connected;
            if (connected != null)
            {
                connected(client);
            }
        }

        /// <summary>
        ///   invokes <see cref = "Disconnected" />.
        /// </summary>
        /// <param name = "client">
        ///   The client.
        /// </param>
        private static void OnDisconnected(OnlineNUnitClient client)
        {
            Action<OnlineNUnitClient> handler = Disconnected;
            if (handler != null)
            {
                handler(client);
            }
        }

        /// <summary>
        ///   The on event received.
        /// </summary>
        /// <param name = "client">
        ///   The client.
        /// </param>
        /// <param name = "data">
        ///   The data.
        /// </param>
        private static void OnEventReceived(OnlineNUnitClient client, EventData data)
        {
            Action<OnlineNUnitClient, EventData> received = EventReceived;
            if (received != null)
            {
                received(client, data);
            }
        }

        /// <summary>
        ///   The on response received.
        /// </summary>
        /// <param name = "client">
        ///   The client.
        /// </param>
        /// <param name = "response">
        ///   The response.
        /// </param>
        private static void OnResponseReceived(OnlineNUnitClient client, OperationResponse response)
        {
            Action<OnlineNUnitClient, OperationResponse> received = ResponseReceived;
            if (received != null)
            {
                received(client, response);
            }
        }

        /// <summary>
        ///   The service.
        /// </summary>
        private void Service()
        {
            if (Interlocked.Read(ref this.service) == 1)
            {
                this.Peer.Service();
                this.fiber.Schedule(this.Service, 50);
            }
        }

        /// <summary>
        ///   The start service.
        /// </summary>
        private void StartService()
        {
            Interlocked.Exchange(ref this.service, 1);
            this.fiber.Enqueue(this.Service);
        }

        /// <summary>
        ///   The stop service.
        /// </summary>
        private void StopService()
        {
            Interlocked.Exchange(ref this.service, 0);
        }

        public void SetupEncryption(Dictionary<byte, object> encryptionData)
        {
            if (this.NetworkProtocol == NetworkProtocolType.SecureWebSocket)
            {
                return;
            }

            var encryptionMode = (byte)encryptionData[EncryptionDataParameters.EncryptionMode];
            var mode = (EncryptionModes)encryptionMode;
            byte[] hmacSecret;
            byte[] secret;
            switch (mode)
            {
                case EncryptionModes.PayloadEncryption:
                    //this.Peer.PayloadEncryptorType = typeof(DiffieHellmanCryptoProvider);
                    this.Peer.InitPayloadEncryption((byte[])encryptionData[EncryptionDataParameters.EncryptionSecret]);
                    return;

                case EncryptionModes.DatagramEncryptionWithRandomInitialNumbers:
                case EncryptionModes.DatagramEncryption:
                    {
                        secret = (byte[])encryptionData[EncryptionDataParameters.EncryptionSecret];
                        hmacSecret = (byte[])encryptionData[EncryptionDataParameters.AuthSecret];
                        break;
                    }
                case EncryptionModes.DatagramEncryptionGCM:
                    {
                        secret = (byte[])encryptionData[EncryptionDataParameters.EncryptionSecret];
                        hmacSecret = null;
                        break;
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (this.Peer.EncryptorType == null)
            {
                this.Peer.EncryptorType = typeof(EncryptorNative);
            }
            this.Peer.InitDatagramEncryption(secret, hmacSecret,
                mode == EncryptionModes.DatagramEncryptionWithRandomInitialNumbers ||
                mode == EncryptionModes.DatagramEncryptionGCM,
                mode == EncryptionModes.DatagramEncryptionGCM);
        }

        #endregion
    }
}
