using System;
using System.Reflection;
using System.Threading;
using Photon.Realtime;
using NUnit.Framework;
using Photon.Common.Authentication;
using Photon.Hive.Operations;
using Photon.LoadBalancing.UnifiedClient;
using Photon.LoadBalancing.UnitTests.UnifiedServer;
using Photon.UnitTest.Utils.Basic;

namespace Photon.LoadBalancing.UnitTests.TestsImpl
{
    public abstract class LBAsyncJoinTestImpl : LoadBalancingUnifiedTestsBase
    {
        protected AuthTokenFactory tokenFactory;

        protected LBAsyncJoinTestImpl(ConnectPolicy policy) : base(policy)
        {
        }

        protected override void FixtureSetup()
        {
            base.FixtureSetup();

            this.tokenFactory = this.CreateAuthTokenFactory();

            var hmacKey = Settings.Default.HMACTokenKey;

            this.tokenFactory.Initialize(Settings.Default.AuthTokenKey, hmacKey,
                TimeSpan.FromSeconds(Settings.Default.AuthTokenExpirationSeconds), Environment.MachineName);

        }

        [Test]
        public void AsyncJoinToNonExistingGame()
        {
            UnifiedTestClient client = null;

            try
            {
                var roomName = GenerateRandomizedRoomName(MethodBase.GetCurrentMethod().Name);

                // try join game on master
                client = this.CreateMasterClientAndAuthenticate(this.Player1);
                client.JoinGame(roomName);

                // try join game on gameServer
                this.ConnectAndAuthenticate(client, this.GameServerAddress);
                client.JoinGame(roomName, ErrorCode.GameDoesNotExist);
            }
            finally
            {
                DisposeClients(client);
            }
        }

        [Test]
        public void AsyncJoinToExistingGame()
        {
            if (this.IsOnline)
            {
                Assert.Ignore("this test works only in offline mode");
            }

            UnifiedTestClient client = null;
            UnifiedTestClient client2 = null;

            try
            {
                var roomName = GenerateRandomizedRoomName(MethodBase.GetCurrentMethod().Name);

                client = this.CreateMasterClientAndAuthenticate(this.Player1);

                // we update token to pass host and game check on GS
                this.UpdateTokensGSAndGame(client, "localhost", roomName);

                // try join game on gameServer
                this.ConnectAndAuthenticate(client, this.GameServerAddress);
                client.CreateGame(roomName);

                // try join game on master
                client2 = this.CreateMasterClientAndAuthenticate(this.Player2);
                client2.JoinGame(roomName);

                // we update token to pass host and game check on GS
                this.UpdateTokensGSAndGame(client, "localhost", roomName);

                // try join game on gameServer
                this.ConnectAndAuthenticate(client2, this.GameServerAddress);
                client2.JoinGame(roomName);
            }
            finally
            {
                DisposeClients(client, client2);
            }
        }


        [Test]
        public void AsyncJoinToSavedGame()
        {
            if (this.IsOnline)
            {
                Assert.Ignore("this test works only in offline mode");
            }

            UnifiedTestClient client = null;
            UnifiedTestClient client2 = null;

            try
            {
                var roomName = GenerateRandomizedRoomName(MethodBase.GetCurrentMethod().Name);

                client = this.CreateMasterClientAndAuthenticate(this.Player1);

                // we update token to pass host and game check on GS
                this.UpdateTokensGSAndGame(client, "localhost", roomName);

                // try join game on gameServer
                this.ConnectAndAuthenticate(client, this.GameServerAddress);

                client.CreateRoom(roomName, new RoomOptions(), TypedLobby.Default, null, true, "SaveLoadStateTestPlugin");

                Thread.Sleep(1000);
                client.Disconnect();
                Thread.Sleep(1000);

                // try join game on master
                client2 = this.CreateMasterClientAndAuthenticate(this.Player2);
                client2.JoinGame(roomName);

                // we update token to pass host and game check on GS
                this.UpdateTokensGSAndGame(client, "localhost", roomName);

                // try join game on gameServer
                this.ConnectAndAuthenticate(client2, this.GameServerAddress);
                var joinGameRequest = new JoinGameRequest
                {
                    GameId = roomName,
                    Plugins = new[] {"SaveLoadStateTestPlugin"}
                };
                client2.JoinGame(joinGameRequest);
            }
            finally
            {
                DisposeClients(client, client2);
            }
        }



        #region Methods

        protected virtual AuthTokenFactory CreateAuthTokenFactory()
        {
            return new AuthTokenFactory();
        }

        private static string GenerateRandomizedRoomName(string roomName)
        {
            return roomName + Guid.NewGuid().ToString().Substring(0, 6);
        }
        protected void UpdateTokensGSAndGame(UnifiedTestClient client, string gs, string game)
        {
            Assert.That(this.tokenFactory.DecryptAuthenticationToken((string)client.Token, out var authToken, out var errorMsg), Is.True, errorMsg);

            authToken.ExpectedGS = gs;
            authToken.ExpectedGameId = game;

            client.Token = this.tokenFactory.EncryptAuthenticationToken(authToken, true);
        }

        #endregion

    }
}
