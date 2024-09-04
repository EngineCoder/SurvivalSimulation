using NUnit.Framework;
using Photon.Hive.Operations;
using Photon.SocketServer;
using Photon.SocketServer.Rpc.Protocols;
using System.Collections;
using Photon.Hive.Plugin;

namespace Photon.LoadBalancing.UnitTests.Tests.RequestTests
{
    [TestFixture]
    public class JoinGameRequestTests
    {
        [Test]
        public void JoinGameRequestTest()
        {
            var request = new SocketServer.OperationRequest((byte) OperationCode.JoinGame)
            {
                Parameters = new System.Collections.Generic.Dictionary<byte, object>()
                {
                    {(byte)ParameterKey.GameProperties, new Hashtable {{(byte)GameParameter.EmptyRoomTTL, 0}}},
                    {(byte)ParameterKey.GameId, null},
                    {(byte)ParameterKey.RoomOptionFlags, null},
                },
                RequestMetaData = new SocketServer.Rpc.Protocols.RequestMetaData()
            };

            var requestObj = new CreateGameRequest(Photon.SocketServer.Protocol.GpBinaryV162, request, "");
            Assert.IsTrue(requestObj.IsValid, requestObj.GetErrorMessage());
        }

        [Test]
        public void JoinModeTest()
        {
            var request = new SocketServer.OperationRequest((byte) OperationCode.JoinGame)
            {
                Parameters = new System.Collections.Generic.Dictionary<byte, object>()
                {
                    {(byte)ParameterKey.GameId, "null"},
                    {(byte)ParameterKey.JoinMode, JoinModeConstants.JoinOnly},
                },
                RequestMetaData = new RequestMetaData()
            };

            var requestObj = new JoinGameRequest(Protocol.GpBinaryV162, request, "");
            Assert.IsTrue(requestObj.IsValid, requestObj.GetErrorMessage());

            Assert.IsFalse(requestObj.IsRejoining);
            Assert.That(requestObj.JoinMode, Is.EqualTo(JoinModeConstants.JoinOnly));
        }

        [Test]
        public void CreateIfNotExistsModeTest()
        {
            var request = new SocketServer.OperationRequest((byte) OperationCode.JoinGame)
            {
                Parameters = new System.Collections.Generic.Dictionary<byte, object>()
                {
                    {(byte)ParameterKey.GameId, "null"},
                    {(byte)ParameterKey.JoinMode, JoinModeConstants.CreateIfNotExists},
                },
                RequestMetaData = new RequestMetaData()
            };

            var requestObj = new JoinGameRequest(Protocol.GpBinaryV162, request, "");
            Assert.IsTrue(requestObj.IsValid, requestObj.GetErrorMessage());

            Assert.IsFalse(requestObj.IsRejoining);
            Assert.IsTrue(requestObj.CreateIfNotExists);
            Assert.That(requestObj.JoinMode, Is.EqualTo(JoinModeConstants.CreateIfNotExists));
        }

        [Test]
        public void ReJoinModeTest()
        {
            var request = new SocketServer.OperationRequest((byte) OperationCode.JoinGame)
            {
                Parameters = new System.Collections.Generic.Dictionary<byte, object>()
                {
                    {(byte)ParameterKey.GameId, "null"},
                    {(byte)ParameterKey.JoinMode, JoinModeConstants.RejoinOnly},
                },
                RequestMetaData = new RequestMetaData()
            };

            var requestObj = new JoinGameRequest(Protocol.GpBinaryV162, request, "");
            Assert.IsTrue(requestObj.IsValid, requestObj.GetErrorMessage());

            Assert.IsTrue(requestObj.IsRejoining);
            Assert.That(requestObj.JoinMode, Is.EqualTo(JoinModeConstants.RejoinOnly));
        }

        [Test]
        public void ReJoinOrJoinModeTest()
        {
            var request = new SocketServer.OperationRequest((byte) OperationCode.JoinGame)
            {
                Parameters = new System.Collections.Generic.Dictionary<byte, object>()
                {
                    {(byte)ParameterKey.GameId, "null"},
                    {(byte)ParameterKey.JoinMode, JoinModeConstants.RejoinOrJoin},
                },
                RequestMetaData = new RequestMetaData()
            };

            var requestObj = new JoinGameRequest(Protocol.GpBinaryV162, request, "");
            Assert.IsTrue(requestObj.IsValid, requestObj.GetErrorMessage());

            Assert.IsTrue(requestObj.IsRejoining);
            Assert.That(requestObj.JoinMode, Is.EqualTo(JoinModeConstants.RejoinOrJoin));
        }

        [Test]
        public void JoinGameRequestTextPropertiesTest()
        {
            var controller = new InboundController((byte)OperationCode.JoinGame, (byte)OperationCode.JoinGame, 
                (byte)ParameterKey.GameProperties, (byte)ParameterKey.GameProperties);
            controller.SetupOperationParameter((byte)OperationCode.JoinGame,
                (byte)ParameterKey.GameProperties, new ParameterData(InboundController.PROVIDE_SIZE_OF_SUB_KEYS));
            Protocol.InboundController = controller;

            var request = new SocketServer.OperationRequest((byte) OperationCode.JoinGame)
            {
                Parameters = new System.Collections.Generic.Dictionary<byte, object>()
                {
                    {
                        (byte)ParameterKey.GameProperties, new Hashtable
                        {
                            {(byte)GameParameter.EmptyRoomTTL, 1}, 
                            {250, new [] {"test1", "test2"}}
                        }
                    },
                    {(byte)ParameterKey.GameId, "test"},
                    {(byte)ParameterKey.RoomOptionFlags, null},
                },
                RequestMetaData = new SocketServer.Rpc.Protocols.RequestMetaData()
            };

            var data = Protocol.GpBinaryV162.SerializeOperationRequest(request);
            Protocol.GpBinaryV162.TryParseOperationRequest(data, out request, out _, out _);

            var requestObj = new JoinGameRequest(Photon.SocketServer.Protocol.GpBinaryV162, request, "");
            Assert.IsTrue(requestObj.IsValid, requestObj.GetErrorMessage());

            var paramMetaData = request.RequestMetaData[(byte) ParameterKey.GameProperties];
            foreach (var v in requestObj.GameProperties.Keys)
            {
                Assert.IsTrue(paramMetaData.SubtypeMetaData.ContainsKey(v));
            }
        }
    }
}
