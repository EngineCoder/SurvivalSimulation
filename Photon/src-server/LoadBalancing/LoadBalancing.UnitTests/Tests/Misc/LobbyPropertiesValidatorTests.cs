using NUnit.Framework;

using Photon.Hive.Common;
using Photon.Hive.Operations;
using Photon.Hive.Plugin;
using Photon.LoadBalancing.GameServer;
using Photon.LoadBalancing.Operations;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using Photon.SocketServer.Rpc.Protocols;

using System.Collections;
using System.Collections.Generic;

using OperationCode = Photon.Hive.Operations.OperationCode;

namespace Photon.LoadBalancing.UnitTests.Tests.Misc
{
    [TestFixture]
    public class LobbyPropertiesValidatorTests
    {
        private readonly PropertiesLimitsSettings propertyLimits = new PropertiesLimitsSettings
        {
            MaxLobbyPropertiesCount = 5
        };

        private readonly InboundController inboundController;

        public LobbyPropertiesValidatorTests()
        {
            this.inboundController = new InboundController(
                (byte)Operations.OperationCode.LoadBalancingMin,
                (byte)Operations.OperationCode.LoadBalancingMax,
                (byte)ParameterCode.ParameterCodeMin,
                (byte)ParameterCode.ParameterCodeMax,
                GameServerSettings.Default.Limits.Inbound.Operations.BlockNonRegisteredOps
            );

            this.inboundController.SetupOperationParameter((byte)OperationCode.SetProperties,
                (byte)ParameterKey.Properties, new ParameterData(
                    InboundController.PROVIDE_SIZE_OF_SUB_KEYS,
                    GameServerSettings.Default.Limits.Inbound.Properties.MaxPropertiesSizePerRequest,
                    GameServerSettings.Default.Limits.Inbound.Properties.MaxPropertiesPerRequest));

            this.inboundController.SetupOperationParameter((byte)OperationCode.SetProperties,
                (byte)ParameterKey.ExpectedValues, new ParameterData(
                    InboundController.PROVIDE_SIZE_OF_SUB_KEYS,
                    GameServerSettings.Default.Limits.Inbound.Properties.MaxPropertiesSizePerRequest,
                    GameServerSettings.Default.Limits.Inbound.Properties.MaxPropertiesPerRequest));

            this.inboundController.SetupOperationParameter((byte)Hive.Operations.OperationCode.CreateGame,
                (byte)Hive.Operations.ParameterKey.GameProperties,
                new ParameterData(InboundController.PROVIDE_SIZE_OF_SUB_KEYS));
            this.inboundController.SetupOperationParameter((byte)Hive.Operations.OperationCode.CreateGame,
                (byte)Hive.Operations.ParameterKey.AddUsers, new ParameterData(InboundController.PROVIDE_SIZE));

        }

        #region SetProperties request tests

        [Test]
        public void ValidCountWithFilter()
        {
            var propertiesOk = new Hashtable
            {
                { (byte)GameParameter.LobbyProperties, new []{"a", "b", "c"} },
                { "a", new byte[100] },
                { "b", new byte[100] },
                { "c", new byte[100] },
            };

            var opRequest = new OperationRequest((byte)OperationCode.SetProperties)
            {
                Parameters = new Dictionary<byte, object>
                {
                    {(byte)ParameterKey.Properties, propertiesOk}
                }
            };

            opRequest = this.UpdateRequestMetaData(opRequest);

            var request = new SetPropertiesRequest(Protocol.GpBinaryV18, opRequest);

            var gameProperties = new PropertyBag<object>();

            Assert.That(
                LobbyPropertiesValidator.ValidateLimits(
                    request,
                    gameProperties,
                    this.propertyLimits,
                    out var errorMsg),
                Is.True,
                errorMsg);
        }

        [Test]
        public void ValidCountWithFilterTooManyProperties()
        {
            var propertiesOk = new Hashtable
            {
                { (byte)GameParameter.LobbyProperties, new []{"a", "b", "c"} },
            };

            var opRequest = new OperationRequest((byte)OperationCode.SetProperties)
            {
                Parameters = new Dictionary<byte, object>
                {
                    {(byte)ParameterKey.Properties, propertiesOk}
                }
            };
            opRequest = this.UpdateRequestMetaData(opRequest);

            var request = new SetPropertiesRequest(Protocol.GpBinaryV18, opRequest);

            var gameProperties = new PropertyBag<object>();
            gameProperties.Set("a", 1);
            gameProperties.Set("b", 1);
            gameProperties.Set("c", 1);
            gameProperties.Set("d", 1);
            gameProperties.Set("e", 1);
            gameProperties.Set("f", 1);
            gameProperties.Set("g", 1);
            gameProperties.Set("i", 1);
            gameProperties.Set("h", 1);
            gameProperties.Set("k", 1);
            gameProperties.Set("l", 1);

            Assert.That(
                LobbyPropertiesValidator.ValidateLimits(
                    request,
                    gameProperties,
                    this.propertyLimits,
                    out var errorMsg),
                Is.True,
                errorMsg);
        }

        [Test]
        public void ValidCountNoFilter()
        {
            var propertiesOk = new Hashtable
            {
                { "a", new byte[100] },
                { "b", new byte[100] },
                { "c", new byte[100] },
            };

            var opRequest = new OperationRequest((byte)OperationCode.SetProperties)
            {
                Parameters = new Dictionary<byte, object>
                {
                    {(byte)ParameterKey.Properties, propertiesOk}
                }
            };

            opRequest = this.UpdateRequestMetaData(opRequest);

            var request = new SetPropertiesRequest(Protocol.GpBinaryV18, opRequest);

            var gameProperties = new PropertyBag<object>();
            gameProperties.Set("x", "y");

            Assert.That(
                LobbyPropertiesValidator.ValidateLimits(
                    request,
                    gameProperties,
                    this.propertyLimits,
                    out var errorMsg),
                Is.True,
                errorMsg);
        }

        [Test]
        public void ValidCountNoFilterLobbyFilterPreviouslySet()
        {
            var propertiesOk = new Hashtable
            {
                { "a", new byte[100] },
                { "b", new byte[100] },
                { "c", new byte[100] },
            };

            var opRequest = new OperationRequest((byte)OperationCode.SetProperties)
            {
                Parameters = new Dictionary<byte, object>
                {
                    {(byte)ParameterKey.Properties, propertiesOk}
                }
            };
            opRequest = this.UpdateRequestMetaData(opRequest);

            var request = new SetPropertiesRequest(Protocol.GpBinaryV18, opRequest);

            var gameProperties = new PropertyBag<object>();

            gameProperties.Set((byte)GameParameter.LobbyProperties, new[] { "a", "b", "c" });
            gameProperties.Set(1, "z");
            gameProperties.Set(2, "y");
            gameProperties.Set(3, "z");
            gameProperties.Set(4, "z");
            gameProperties.Set(5, "z");
            gameProperties.Set(6, "z");

            Assert.That(
                LobbyPropertiesValidator.ValidateLimits(
                    request,
                    gameProperties,
                    this.propertyLimits,
                    out var errorMsg),
                Is.True,
                errorMsg);
        }

        [Test]
        public void ValidCountNoFilterWithIntersection()
        {
            var propertiesOk = new Hashtable
            {
                { "a", new byte[100] },
                { "b", new byte[100] },
                { "c", new byte[100] },
            };

            var opRequest = new OperationRequest((byte)OperationCode.SetProperties)
            {
                Parameters = new Dictionary<byte, object>
                {
                    {(byte)ParameterKey.Properties, propertiesOk}
                }
            };
            opRequest = this.UpdateRequestMetaData(opRequest);

            var request = new SetPropertiesRequest(Protocol.GpBinaryV18, opRequest);

            var gameProperties = new PropertyBag<object>();
            gameProperties.Set("x", "y");
            gameProperties.Set("a", "a");
            gameProperties.Set("b", "b");

            Assert.That(
                LobbyPropertiesValidator.ValidateLimits(
                    request,
                    gameProperties,
                    this.propertyLimits,
                    out var errorMsg),
                Is.True,
                errorMsg);
        }

        [Test]
        public void InvalidCountWithFilter()
        {
            var properties = new Hashtable
            {
                { (byte)GameParameter.LobbyProperties, new []{"a", "b", "c", "d", "e", "f"} },
            };

            var opRequest = new OperationRequest((byte)OperationCode.SetProperties)
            {
                Parameters = new Dictionary<byte, object>
                {
                    {(byte)ParameterKey.Properties, properties}
                }
            };

            var request = new SetPropertiesRequest(Protocol.GpBinaryV18, opRequest);

            var gameProperties = new PropertyBag<object>();

            Assert.That(
                LobbyPropertiesValidator.ValidateLimits(
                    request,
                    gameProperties,
                    this.propertyLimits,
                    out _),
                Is.False);
        }

        [Test]
        public void InvalidCountNoFilter()
        {
            var propertiesOk = new Hashtable
            {
                { "a", new byte[100] },
                { "b", new byte[100] },
                { "c", new byte[100] },
            };

            var opRequest = new OperationRequest((byte)OperationCode.SetProperties)
            {
                Parameters = new Dictionary<byte, object>
                {
                    {(byte)ParameterKey.Properties, propertiesOk}
                }
            };

            var request = new SetPropertiesRequest(Protocol.GpBinaryV18, opRequest);

            var gameProperties = new PropertyBag<object>();
            gameProperties.Set("x", "x");
            gameProperties.Set("y", "y");
            gameProperties.Set("z", "z");

            Assert.That(
                LobbyPropertiesValidator.ValidateLimits(
                    request,
                    gameProperties,
                    this.propertyLimits,
                    out _),
                Is.False);
        }

        [Test]
        public void InvalidCountNoFilterWithIntersection()
        {
            var properties = new Hashtable
            {
                { "a", new byte[100] },
                { "b", new byte[100] },
                { "c", new byte[100] },
            };

            var opRequest = new OperationRequest((byte)OperationCode.SetProperties)
            {
                Parameters = new Dictionary<byte, object>
                {
                    {(byte)ParameterKey.Properties, properties}
                }
            };

            var request = new SetPropertiesRequest(Protocol.GpBinaryV18, opRequest);

            var gameProperties = new PropertyBag<object>();
            gameProperties.Set("x", "x");
            gameProperties.Set("y", "y");
            gameProperties.Set("z", "z");
            gameProperties.Set("a", "a");
            gameProperties.Set("b", "b");

            Assert.That(
                LobbyPropertiesValidator.ValidateLimits(
                    request,
                    gameProperties,
                    this.propertyLimits,
                    out _),
                Is.False);
        }

        /// <summary>
        /// new properties with correct size and filter including only new properties
        /// </summary>
        [Test]
        public void ValidSizeWithFilter()
        {
            var propertiesOk = new Hashtable
            {
                { (byte)GameParameter.LobbyProperties, new []{"a", "b", "c"} },
                { "a", new byte[100] },
                { "b", new byte[100] },
                { "c", new byte[100] },
            };

            var opRequest = new OperationRequest((byte)OperationCode.SetProperties)
            {
                Parameters = new Dictionary<byte, object>
                {
                    {(byte)ParameterKey.Properties, propertiesOk}
                }
            };
            opRequest = this.UpdateRequestMetaData(opRequest);

            var request = new SetPropertiesRequest(Protocol.GpBinaryV18, opRequest);

            var gameProperties = new PropertyBag<object>();

            Assert.That(
                LobbyPropertiesValidator.ValidateLimits(
                    request,
                    gameProperties,
                    this.propertyLimits,
                    out var errorMsg),
                Is.True,
                errorMsg);
        }

        /// <summary>
        /// Some values already were set to game
        /// filter includes both already set and new
        /// </summary>
        [Test]
        public void ValidSizeWithFilterSomePropertiesAlreadySetFilterIncludesBoth()
        {
            var propertiesOk = new Hashtable
            {
                { (byte)GameParameter.LobbyProperties, new []{"a", "b", "x"} },
                { "a", new byte[100] },
                { "b", new byte[100] },
                { "c", new byte[100] },
            };

            var opRequest = new OperationRequest((byte)OperationCode.SetProperties)
            {
                Parameters = new Dictionary<byte, object>
                {
                    {(byte)ParameterKey.Properties, propertiesOk}
                }
            };
            opRequest = this.UpdateRequestMetaData(opRequest);

            var request = new SetPropertiesRequest(Protocol.GpBinaryV18, opRequest);

            var gameProperties = new PropertyBag<object>();
            gameProperties.Set("x", 1, new KeyValuePair<int, int>(50, 50));

            Assert.That(
                LobbyPropertiesValidator.ValidateLimits(
                    request,
                    gameProperties,
                    this.propertyLimits,
                    out var errorMsg),
                Is.True,
                errorMsg);
        }

        /// <summary>
        /// some properties are already set
        /// filter includes only new
        /// </summary>
        [Test]
        public void ValidSizeWithFilterSomePropertiesAlreadySet()
        {
            var propertiesOk = new Hashtable
            {
                { (byte)GameParameter.LobbyProperties, new []{"a", "b", "c"} },
                { "a", new byte[100] },
                { "b", new byte[100] },
                { "c", new byte[100] },
            };

            var opRequest = new OperationRequest((byte)OperationCode.SetProperties)
            {
                Parameters = new Dictionary<byte, object>
                {
                    {(byte)ParameterKey.Properties, propertiesOk}
                }
            };
            opRequest = this.UpdateRequestMetaData(opRequest);

            var request = new SetPropertiesRequest(Protocol.GpBinaryV18, opRequest);

            var gameProperties = new PropertyBag<object>();
            gameProperties.Set("x", 1, new KeyValuePair<int, int>(50, 50));

            Assert.That(
                LobbyPropertiesValidator.ValidateLimits(
                    request,
                    gameProperties,
                    this.propertyLimits,
                    out var errorMsg),
                Is.True,
                errorMsg);
        }

        [Test]
        public void ValidSizeNoFilter()
        {
            var propertiesOk = new Hashtable
            {
                { "a", new byte[100] },
                { "b", new byte[100] },
                { "c", new byte[100] },
            };

            var opRequest = new OperationRequest((byte)OperationCode.SetProperties)
            {
                Parameters = new Dictionary<byte, object>
                {
                    {(byte)ParameterKey.Properties, propertiesOk}
                }
            };
            opRequest = this.UpdateRequestMetaData(opRequest);

            var request = new SetPropertiesRequest(Protocol.GpBinaryV18, opRequest);

            var gameProperties = new PropertyBag<object>();

            Assert.That(
                LobbyPropertiesValidator.ValidateLimits(
                    request,
                    gameProperties,
                    this.propertyLimits,
                    out var errorMsg),
                Is.True,
                errorMsg);
        }

        /// <summary>
        /// new request contains only new properties
        /// and some properties are already set
        /// </summary>
        [Test]
        public void ValidSizeNoFilterSomePropertiesAlreadySet()
        {
            var propertiesOk = new Hashtable
            {
                { "a", new byte[100] },
                { "b", new byte[100] },
                { "c", new byte[100] },
            };

            var opRequest = new OperationRequest((byte)OperationCode.SetProperties)
            {
                Parameters = new Dictionary<byte, object>
                {
                    {(byte)ParameterKey.Properties, propertiesOk}
                }
            };
            opRequest = this.UpdateRequestMetaData(opRequest);

            var request = new SetPropertiesRequest(Protocol.GpBinaryV18, opRequest);

            var gameProperties = new PropertyBag<object>();
            gameProperties.Set("x", 1, new KeyValuePair<int, int>(30, 50));

            Assert.That(
                LobbyPropertiesValidator.ValidateLimits(
                    request,
                    gameProperties,
                    this.propertyLimits,
                    out var errorMsg),
                Is.True,
                errorMsg);
        }

        /// <summary>
        /// incoming request contains new properties and update for existing
        /// </summary>
        [Test]
        public void ValidSizeNoFilterUpdateSomeProperties()
        {
            var propertiesOk = new Hashtable
            {
                { "a", new byte[100] },
                { "b", new byte[100] },
                { "c", new byte[100] },
                { "x", new byte[70] }
            };

            var opRequest = new OperationRequest((byte)OperationCode.SetProperties)
            {
                Parameters = new Dictionary<byte, object>
                {
                    {(byte)ParameterKey.Properties, propertiesOk}
                }
            };
            opRequest = this.UpdateRequestMetaData(opRequest);

            var request = new SetPropertiesRequest(Protocol.GpBinaryV18, opRequest);

            var gameProperties = new PropertyBag<object>();
            gameProperties.Set("x", 1, new KeyValuePair<int, int>(50, 50));

            Assert.That(
                LobbyPropertiesValidator.ValidateLimits(
                    request,
                    gameProperties,
                    this.propertyLimits,
                    out var errorMsg),
                Is.True,
                errorMsg);
        }

        [Test]
        public void InvalidSizeWithFilter()
        {
            var properties = new Hashtable
            {
                { (byte)GameParameter.LobbyProperties, new []{"a", "b", "c"} },
                { "a", new byte[100] },
                { "b", new byte[200] },
                { "c", new byte[200] },
            };

            var opRequest = new OperationRequest((byte)OperationCode.SetProperties)
            {
                Parameters = new Dictionary<byte, object>
                {
                    {(byte)ParameterKey.Properties, properties}
                }
            };

            opRequest = this.UpdateRequestMetaData(opRequest);

            var request = new SetPropertiesRequest(Protocol.GpBinaryV18, opRequest);

            var gameProperties = new PropertyBag<object>();

            Assert.That(
                LobbyPropertiesValidator.ValidateLimits(
                    request,
                    gameProperties,
                    this.propertyLimits,
                    out _),
                Is.False);
        }

        [Test]
        public void InvalidSizeNoFilter()
        {
            var properties = new Hashtable
            {
                { "a", new byte[100] },
                { "b", new byte[200] },
                { "c", new byte[200] },
            };

            var opRequest = new OperationRequest((byte)OperationCode.SetProperties)
            {
                Parameters = new Dictionary<byte, object>
                {
                    {(byte)ParameterKey.Properties, properties}
                }
            };

            opRequest = this.UpdateRequestMetaData(opRequest);

            var request = new SetPropertiesRequest(Protocol.GpBinaryV18, opRequest);

            var gameProperties = new PropertyBag<object>();

            Assert.That(
                LobbyPropertiesValidator.ValidateLimits(
                    request,
                    gameProperties,
                    this.propertyLimits,
                    out _),
                Is.False);
        }

        /// <summary>
        /// new request contains only new properties and their size
        /// plus existing are too big
        /// </summary>
        [Test]
        public void InvalidSizeNoFilterAddTooMuchToExistingProperties()
        {
            var properties = new Hashtable
            {
                { "a", new byte[200] },
            };

            var opRequest = new OperationRequest((byte)OperationCode.SetProperties)
            {
                Parameters = new Dictionary<byte, object>
                {
                    {(byte)ParameterKey.Properties, properties}
                }
            };

            opRequest = this.UpdateRequestMetaData(opRequest);

            var request = new SetPropertiesRequest(Protocol.GpBinaryV18, opRequest);

            var gameProperties = new PropertyBag<object>();
            gameProperties.Set(1, 1, new KeyValuePair<int, int>(100, 200));

            Assert.That(
                LobbyPropertiesValidator.ValidateLimits(
                    request,
                    gameProperties,
                    this.propertyLimits,
                    out _),
                Is.False);
        }

        [Test]
        public void InvalidSizeWithIntersection()
        {
            var properties = new Hashtable
            {
                { "a", new byte[200] },
            };

            var opRequest = new OperationRequest((byte)OperationCode.SetProperties)
            {
                Parameters = new Dictionary<byte, object>
                {
                    {(byte)ParameterKey.Properties, properties}
                }
            };

            opRequest = this.UpdateRequestMetaData(opRequest);

            var request = new SetPropertiesRequest(Protocol.GpBinaryV18, opRequest);

            var gameProperties = new PropertyBag<object>();
            gameProperties.Set(1, 1, new KeyValuePair<int, int>(100, 200));
            gameProperties.Set("a", 1, new KeyValuePair<int, int>(10, 20));

            Assert.That(
                LobbyPropertiesValidator.ValidateLimits(
                    request,
                    gameProperties,
                    this.propertyLimits,
                    out _),
                Is.False);
        }

        [Test]
        public void InvalidSizeAfterFilterChange()
        {
            var properties = new Hashtable
            {
                { (byte)GameParameter.LobbyProperties, new []{"a", "b", "c"} },
            };

            var opRequest = new OperationRequest((byte)OperationCode.SetProperties)
            {
                Parameters = new Dictionary<byte, object>
                {
                    {(byte)ParameterKey.Properties, properties}
                }
            };

            opRequest = this.UpdateRequestMetaData(opRequest);

            var request = new SetPropertiesRequest(Protocol.GpBinaryV18, opRequest);

            var gameProperties = new PropertyBag<object>();
            gameProperties.Set(1, 1, new KeyValuePair<int, int>(100, 200));
            gameProperties.Set("a", 1, new KeyValuePair<int, int>(10, 200));
            gameProperties.Set("b", 1, new KeyValuePair<int, int>(10, 200));
            gameProperties.Set("c", 1, new KeyValuePair<int, int>(10, 200));
            gameProperties.Set((byte)GameParameter.LobbyProperties, new[] { "a" }, new KeyValuePair<int, int>(10, 200));

            Assert.That(
                LobbyPropertiesValidator.ValidateLimits(
                    request,
                    gameProperties,
                    this.propertyLimits,
                    out _),
                Is.False);
        }

        #endregion

        #region CreateGame request tests

        [Test]
        public void CreateGameNoProperties()
        {
            var opRequest = new OperationRequest((byte)OperationCode.CreateGame)
            {
                Parameters = new Dictionary<byte, object>
                {
                    {(byte)ParameterKey.GameId, "test"},
                }
            };

            opRequest = this.UpdateRequestMetaData(opRequest);

            var createGameRequest = new CreateGameRequest(Protocol.GpBinaryV18, opRequest, "x");

            var result = LobbyPropertiesValidator.ValidateLimits(createGameRequest, this.propertyLimits, out var errorMsg);

            Assert.That(result, Is.True, errorMsg);
        }

        [Test]
        public void CreateGameEmptyProperties()
        {
            var opRequest = new OperationRequest((byte)OperationCode.CreateGame)
            {
                Parameters = new Dictionary<byte, object>
                {
                    {(byte) ParameterKey.GameId, "test"},
                    {(byte) ParameterKey.GameProperties, new Hashtable()}
                }
            };

            opRequest = this.UpdateRequestMetaData(opRequest);

            var createGameRequest = new CreateGameRequest(Protocol.GpBinaryV18, opRequest, "x");

            var result = LobbyPropertiesValidator.ValidateLimits(createGameRequest, this.propertyLimits, out var errorMsg);

            Assert.That(result, Is.True, errorMsg);
        }

        [Test]
        public void CreateGameWithPropertiesNoLobbyFilter()
        {
            var opRequest = new OperationRequest((byte)OperationCode.CreateGame)
            {
                Parameters = new Dictionary<byte, object>
                {
                    {(byte) ParameterKey.GameId, "test"},
                    {
                        (byte) ParameterKey.GameProperties, new Hashtable
                        {
                            {1, 1},
                            {2, 2},
                            {3, 3},
                        }
                    }
                }
            };

            opRequest = this.UpdateRequestMetaData(opRequest);

            var createGameRequest = new CreateGameRequest(Protocol.GpBinaryV18, opRequest, "x");

            var result = LobbyPropertiesValidator.ValidateLimits(createGameRequest, this.propertyLimits, out var errorMsg);

            Assert.That(result, Is.True, errorMsg);
        }

        [Test]
        public void CreateGameWithPropertiesWithFilter()
        {
            var opRequest = new OperationRequest((byte)OperationCode.CreateGame)
            {
                Parameters = new Dictionary<byte, object>
                {
                    {(byte) ParameterKey.GameId, "test"},
                    {
                        (byte) ParameterKey.GameProperties, new Hashtable
                        {
                            {"1", 1},
                            {"2", 2},
                            {"3", 3},
                            {"4", new byte[500]},
                            {(byte)GameParameter.LobbyProperties, new []{"1", "2", "3"}}
                        }
                    }
                }
            };

            opRequest = this.UpdateRequestMetaData(opRequest);

            var createGameRequest = new CreateGameRequest(Protocol.GpBinaryV18, opRequest, "x");

            var result = LobbyPropertiesValidator.ValidateLimits(createGameRequest, this.propertyLimits, out var errorMsg);

            Assert.That(result, Is.True, errorMsg);
        }

        [Test]
        public void CreateGameWithTooManyPropertiesNoLobbyFilter()
        {
            var opRequest = new OperationRequest((byte)OperationCode.CreateGame)
            {
                Parameters = new Dictionary<byte, object>
                {
                    {(byte) ParameterKey.GameId, "test"},
                    {
                        (byte) ParameterKey.GameProperties, new Hashtable
                        {
                            {"1", 1},
                            {"2", 2},
                            {"3", 3},
                            {"4", 4},
                            {"5", 5},
                            {"6", 6},
                        }
                    }
                }
            };

            opRequest = this.UpdateRequestMetaData(opRequest);

            var createGameRequest = new CreateGameRequest(Protocol.GpBinaryV18, opRequest, "x");

            var result = LobbyPropertiesValidator.ValidateLimits(createGameRequest, this.propertyLimits, out _);

            Assert.That(result, Is.False);
        }

        [Test]
        public void CreateGameWithTooManyPropertiesWithFilter()
        {
            var opRequest = new OperationRequest((byte)OperationCode.CreateGame)
            {
                Parameters = new Dictionary<byte, object>
                {
                    {(byte) ParameterKey.GameId, "test"},
                    {
                        (byte) ParameterKey.GameProperties, new Hashtable
                        {
                            {"1", 1},
                            {"2", 2},
                            {"3", 3},
                            {"4", 4},
                            {"5", 5},
                            {"6", 6},
                            {(byte)GameParameter.LobbyProperties, new []{"1", "2", "3", "4", "5", "6"}},
                        }
                    }
                }
            };

            opRequest = this.UpdateRequestMetaData(opRequest);

            var createGameRequest = new CreateGameRequest(Protocol.GpBinaryV18, opRequest, "x");

            var result = LobbyPropertiesValidator.ValidateLimits(createGameRequest, this.propertyLimits, out _);

            Assert.That(result, Is.False);
        }

        [Test]
        public void CreateGameWithTooBigPropertiesNoLobbyFilter()
        {
            var opRequest = new OperationRequest((byte)OperationCode.CreateGame)
            {
                Parameters = new Dictionary<byte, object>
                {
                    {(byte) ParameterKey.GameId, "test"},
                    {
                        (byte) ParameterKey.GameProperties, new Hashtable
                        {
                            {"1", new byte[500]},
                        }
                    }
                }
            };

            opRequest = this.UpdateRequestMetaData(opRequest);

            var createGameRequest = new CreateGameRequest(Protocol.GpBinaryV18, opRequest, "x");

            var result = LobbyPropertiesValidator.ValidateLimits(createGameRequest, this.propertyLimits, out _);

            Assert.That(result, Is.False);
        }

        [Test]
        public void CreateGameEmptyWithTooBigPropertiesWithFilter()
        {
            var opRequest = new OperationRequest((byte)OperationCode.CreateGame)
            {
                Parameters = new Dictionary<byte, object>
                {
                    {(byte) ParameterKey.GameId, "test"},
                    {
                        (byte) ParameterKey.GameProperties, new Hashtable
                        {
                            {"1", new byte[500]},
                            {(byte)GameParameter.LobbyProperties, new []{"1"}},
                        }
                    }
                }
            };

            opRequest = this.UpdateRequestMetaData(opRequest);

            var createGameRequest = new CreateGameRequest(Protocol.GpBinaryV18, opRequest, "x");

            var result = LobbyPropertiesValidator.ValidateLimits(createGameRequest, this.propertyLimits, out _);

            Assert.That(result, Is.False);
        }

        #endregion

        #region .privates

        private OperationRequest UpdateRequestMetaData(OperationRequest opRequest)
        {
            var data = Protocol.GpBinaryV18.SerializeOperationRequest(opRequest,
                new SerializeSetup(null, this.inboundController));
            Protocol.GpBinaryV18.TryParseOperationRequest(data, out opRequest, out _, out _,
                new SerializeSetup(null, this.inboundController));
            return opRequest;
        }

        #endregion
    }
}
