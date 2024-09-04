using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;

using ExitGames.Client.Photon;
using ExitGames.Logging;
using Newtonsoft.Json;

using NUnit.Framework;
using Photon.Common.Authentication.Data;
using Photon.Hive.Operations;
using Photon.LoadBalancing.UnifiedClient;
using Photon.LoadBalancing.UnitTests.UnifiedServer;
using Photon.Realtime;
using Photon.UnitTest.Utils.Basic;

using Hashtable = System.Collections.Hashtable;
using OperationCode = Photon.Realtime.OperationCode;

namespace Photon.LoadBalancing.UnitTests.TestsImpl
{
    public abstract class SecureTestsImpl : LoadBalancingUnifiedTestsBase
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private CustomAuthHttpListener customAuthHttpListener;

        protected SecureTestsImpl(ConnectPolicy policy) : base(policy)
        {
        }

        protected override void FixtureSetup()
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("Fixture setup");
            }

            base.FixtureSetup();

            if (!this.connectPolicy.IsRemote)
            {
                if (log.IsDebugEnabled)
                {
                    log.DebugFormat("setting up custom auth listener");
                }

                this.customAuthHttpListener = new CustomAuthHttpListener();
            }
            else
            {
                if (log.IsDebugEnabled)
                {
                    log.DebugFormat("setting up of custom auth listener skipped. policy is remote");
                }

            }
        }

        protected override void FixtureTearDown()
        {
            this.customAuthHttpListener?.Dispose();

            base.FixtureTearDown();
        }

        [Test]
        public void SecureParamsTest()
        {
            UnifiedTestClient client = null;
            UnifiedTestClient client2 = null;
            const int SleepTime = 350;
            try
            {
                client = (UnifiedTestClient)this.CreateTestClient();
                client.UserId = Player1;

                this.ConnectToServer(client, this.NameServerAddress);

                var response = client.Authenticate(Player1, 
                    new Dictionary<byte, object>
                    {
                        {(byte)ParameterKey.ClientAuthenticationParams, "username=yes&token=yes"},
                        {(byte)ParameterKey.ClientAuthenticationType, (byte)ClientAuthenticationType.Custom}
                    });

                Assert.AreEqual("nick", response[(byte)ParameterKey.Nickname]);
                Assert.IsNotNull(response[(byte)ParameterKey.Token]);

                this.ConnectAndAuthenticate(client, this.MasterAddress);

                var request = new OperationRequest
                {
                    OperationCode = OperationCode.CreateGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, "TestGame"},
                        {ParameterCode.Plugins, new []{"CheckSecurePlugin"}},
                    }
                };

                response = client.SendRequestAndWaitForResponse(request);
                client.Token = response[ParameterCode.Token];

                this.ConnectAndAuthenticate(client, (string)response[ParameterCode.Address], client.UserId);

                client.SendRequestAndWaitForResponse(request);

                Thread.Sleep(SleepTime);
                CheckSecure("CreateGameAuthCookie");

                client2 = this.CreateMasterClientAndAuthenticate("User2", 
                    new Dictionary<byte, object>
                    {
                        {(byte)ParameterKey.ClientAuthenticationParams, "username=yes&token=yes"},
                        {(byte)ParameterKey.ClientAuthenticationType, (byte)ClientAuthenticationType.Custom}
                    });

                request = new OperationRequest
                {
                    OperationCode = OperationCode.JoinGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, "TestGame"},
                    }
                };

                response = client2.SendRequestAndWaitForResponse(request);

                client2.Token = response[ParameterCode.Token];
                this.ConnectAndAuthenticate(client2, (string)response[ParameterCode.Address], client2.UserId);

                client2.SendRequestAndWaitForResponse(request);

                Thread.Sleep(SleepTime);
                CheckSecure("JoinGameAuthCookie");

                request = new OperationRequest
                {
                    OperationCode = OperationCode.RaiseEvent,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.Code, (byte)1},
                        {ParameterCode.EventForward, (byte)3},
                    }
                };

                client2.SendRequest(request);

                Thread.Sleep(SleepTime);
                CheckSecure("RaiseEventAuthCookie");

                //just to ensure that there is nothing on server for RaiseEventAuthCookie
                CheckSecure("RaiseEventAuthCookie", expectToFail: true);

                request = new OperationRequest
                {
                    OperationCode = OperationCode.RaiseEvent,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.Code, (byte)1},
                        {ParameterCode.EventForward, (byte)1},// we send request but without secure
                    }
                };

                client2.SendRequest(request);
                CheckSecure("RaiseEventAuthCookie", expectToFail: true);


                client.SendRequestAndWaitForResponse(new OperationRequest
                {
                    OperationCode = OperationCode.SetProperties,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.Properties, new Hashtable {{GamePropertyKey.IsOpen, true}}},
                        {ParameterCode.EventForward, (byte)3},
                    }
                });

                Thread.Sleep(SleepTime);
                CheckSecure("SetPropertiesAuthCookie");

                client.SendRequestAndWaitForResponse(new OperationRequest
                {
                    OperationCode = OperationCode.WebRpc,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.Data, new Hashtable {{GamePropertyKey.IsOpen, true}}},
                        {ParameterCode.EventForward, (byte)3},
                        {ParameterCode.UriPath, "RpcSecure"},
                    }
                });

                Thread.Sleep(SleepTime);
                CheckSecure("RpcAuthCookie");

                client.SendRequestAndWaitForResponse(new OperationRequest
                {
                    OperationCode = OperationCode.WebRpc,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.Data, new Hashtable {{GamePropertyKey.IsOpen, true}}},
                        {ParameterCode.UriPath, "RpcSecure"},
                    }
                });

                Thread.Sleep(SleepTime);
                CheckSecure("RpcAuthCookie", true);

                var client3 = this.CreateMasterClientAndAuthenticate("User3", 
                    new Dictionary<byte, object>
                    {
                        {(byte)ParameterKey.ClientAuthenticationParams, "username=yes&token=yes"},
                        {(byte)ParameterKey.ClientAuthenticationType, (byte)ClientAuthenticationType.Custom}
                    });
                client3.SendRequestAndWaitForResponse(new OperationRequest
                {
                    OperationCode = OperationCode.WebRpc,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.Data, new Hashtable {{GamePropertyKey.IsOpen, true}}},
                        {ParameterCode.UriPath, "RpcSecure"},
                        {ParameterCode.EventForward, (byte)3},
                    }
                });

                Thread.Sleep(SleepTime);
                CheckSecure("RpcAuthCookie");

                client3.SendRequestAndWaitForResponse(new OperationRequest
                {
                    OperationCode = OperationCode.WebRpc,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.Data, new Hashtable {{GamePropertyKey.IsOpen, true}}},
                        {ParameterCode.UriPath, "RpcSecure"},
                        {ParameterCode.EventForward, (byte)1},
                    }
                });

                Thread.Sleep(SleepTime);
                CheckSecure("RpcAuthCookie", expectToFail: true);


            }
            finally
            {
                DisposeClients(client, client2);
            }
        }

        private static void CheckSecure(string eventName, bool expectToFail = false)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://localhost:55557/realtime-webhooks-1.2/CheckSecure");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                var json = JsonConvert.SerializeObject(new {Check = eventName});

                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    var jsonResponse = JsonConvert.DeserializeObject<Dictionary<string, object>>(result);

                    Assert.AreEqual(0, jsonResponse["ResultCode"]);
                    Assert.AreEqual(eventName, jsonResponse["Debug"]);
                    if (expectToFail)
                    {
                        Assert.IsFalse(jsonResponse.ContainsKey("Data"));
                    }
                    else
                    {
                        var expected = JsonConvert.DeserializeObject<Dictionary<string, object>>("{\"Param1\":1,\"Param2\":\"2\"}");
                        var got = JsonConvert.DeserializeObject<Dictionary<string, object>>((string)jsonResponse["Data"]);

                        Assert.AreEqual(expected, got);
                    }
                }
            }
        }
    }
}
