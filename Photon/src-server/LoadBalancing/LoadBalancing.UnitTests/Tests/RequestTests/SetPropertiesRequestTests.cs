using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Photon.Hive.Common;
using Photon.Hive.Operations;
using Photon.SocketServer;

namespace Photon.LoadBalancing.UnitTests.Tests.RequestTests
{
    [TestFixture]
    public class SetPropertiesRequestTests
    {
        [Test]
        public void WellKnownPropsFromJson()
        {
            const double EmptyRoomTTLValue = 1;
            const double ExpectedEmptyRoomTTLValue = 2;

            const double MasterClientIdValue = 3;
            const double ExpectedMasterClientIdValue = 4;

            const double PlayerTTLValue = 5;
            const double ExpectedPlayerTTLValue = 6;

            const double MaxPlayerValue = 7;
            const double ExpectedMaxPlayerValue = 8;

            var opRequest = new OperationRequest
            {
                OperationCode = (byte)OperationCode.SetProperties,
                Parameters = new Dictionary<byte, object>
                {
                    {
                        251, new Hashtable
                        {
                            { Utilities.amf3EmptyRoomTTLPropertyKey, EmptyRoomTTLValue },
                            { Utilities.amf3MasterClientIdPropertyKey, MasterClientIdValue },
                            { Utilities.amf3PlayerTTLPropertyKey, PlayerTTLValue},
                            { Utilities.amf3MaxPlayerPropertyKey, MaxPlayerValue}
                        }
                    },
                    {
                        231, new Hashtable
                        {
                            { Utilities.amf3EmptyRoomTTLPropertyKey, ExpectedEmptyRoomTTLValue },
                            { Utilities.amf3MasterClientIdPropertyKey, ExpectedMasterClientIdValue },
                            { Utilities.amf3PlayerTTLPropertyKey, ExpectedPlayerTTLValue},
                            { Utilities.amf3MaxPlayerPropertyKey, ExpectedMaxPlayerValue}
                        }
                    }
                }
            };

            var request = new SetPropertiesRequest(Protocol.Json, opRequest);
            Assert.That(request.IsValid, Is.True, request.GetErrorMessage());

            // we check that string keys were removed
            Assert.That(request.Properties, Does.Not.ContainKey(Utilities.amf3EmptyRoomTTLPropertyKey));
            Assert.That(request.Properties, Does.Not.ContainKey(Utilities.amf3MasterClientIdPropertyKey));
            Assert.That(request.Properties, Does.Not.ContainKey(Utilities.amf3PlayerTTLPropertyKey));
            Assert.That(request.Properties, Does.Not.ContainKey(Utilities.amf3MaxPlayerPropertyKey));

            // we check that byte keys were added
            Assert.That(request.Properties, Does.ContainKey((byte)GameParameter.EmptyRoomTTL));
            Assert.That(request.Properties, Does.ContainKey((byte)GameParameter.MasterClientId));
            Assert.That(request.Properties, Does.ContainKey((byte)GameParameter.PlayerTTL));
            Assert.That(request.Properties, Does.ContainKey((byte)GameParameter.MaxPlayers));

            // we check that string keys were removed for ExpectedValues
            Assert.That(request.ExpectedValues, Does.Not.ContainKey(Utilities.amf3EmptyRoomTTLPropertyKey));
            Assert.That(request.ExpectedValues, Does.Not.ContainKey(Utilities.amf3MasterClientIdPropertyKey));
            Assert.That(request.ExpectedValues, Does.Not.ContainKey(Utilities.amf3PlayerTTLPropertyKey));
            Assert.That(request.ExpectedValues, Does.Not.ContainKey(Utilities.amf3MaxPlayerPropertyKey));

            // we check that byte keys were added for ExpectedValues
            Assert.That(request.ExpectedValues, Does.ContainKey((byte)GameParameter.EmptyRoomTTL));
            Assert.That(request.ExpectedValues, Does.ContainKey((byte)GameParameter.MasterClientId));
            Assert.That(request.ExpectedValues, Does.ContainKey((byte)GameParameter.PlayerTTL));
            Assert.That(request.ExpectedValues, Does.ContainKey((byte)GameParameter.MaxPlayers));

            //we check that properties and expected values were converted to correct types

            Assert.That(request.Properties[(byte)GameParameter.EmptyRoomTTL], Is.EqualTo((int)EmptyRoomTTLValue));
            Assert.That(request.Properties[(byte)GameParameter.MasterClientId], Is.EqualTo((int)MasterClientIdValue));
            Assert.That(request.Properties[(byte)GameParameter.PlayerTTL], Is.EqualTo((int)PlayerTTLValue));
            Assert.That(request.Properties[(byte)GameParameter.MaxPlayers], Is.EqualTo((byte)MaxPlayerValue));

            Assert.That(request.ExpectedValues[(byte)GameParameter.EmptyRoomTTL], Is.EqualTo((int)ExpectedEmptyRoomTTLValue));
            Assert.That(request.ExpectedValues[(byte)GameParameter.MasterClientId], Is.EqualTo((int)ExpectedMasterClientIdValue));
            Assert.That(request.ExpectedValues[(byte)GameParameter.PlayerTTL], Is.EqualTo((int)ExpectedPlayerTTLValue));
            Assert.That(request.ExpectedValues[(byte)GameParameter.MaxPlayers], Is.EqualTo((byte)ExpectedMaxPlayerValue));
        }

        [Test]
        public void WellKnownPropsFromJsonShortcuts()
        {
            const double EmptyRoomTTLValue = 1;
            const double MasterClientIdValue = 3;
            const double PlayerTTLValue = 5;
            const double MaxPlayerValue = 7;

            var opRequest = new OperationRequest
            {
                OperationCode = (byte)OperationCode.SetProperties,
                Parameters = new Dictionary<byte, object>
                {
                    {
                        251, new Hashtable
                        {
                            { Utilities.amf3EmptyRoomTTLPropertyKey, EmptyRoomTTLValue },
                            { Utilities.amf3MasterClientIdPropertyKey, MasterClientIdValue },
                            { Utilities.amf3PlayerTTLPropertyKey, PlayerTTLValue},
                            { Utilities.amf3MaxPlayerPropertyKey, MaxPlayerValue}
                        }
                    },
                }
            };

            var request = new SetPropertiesRequest(Protocol.Json, opRequest);
            Assert.That(request.IsValid, Is.True, request.GetErrorMessage());

            Assert.That(request.wellKnownPropertiesCache.EmptyRoomTTL, Is.EqualTo((int)EmptyRoomTTLValue));
            Assert.That(request.wellKnownPropertiesCache.MasterClientId, Is.EqualTo((int)MasterClientIdValue));
            Assert.That(request.wellKnownPropertiesCache.PlayerTTL, Is.EqualTo((int)PlayerTTLValue));
            Assert.That(request.wellKnownPropertiesCache.MaxPlayer, Is.EqualTo((byte)MaxPlayerValue));
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        public void WellKnownPropsFromJsonWrongValues(int testCase)
        {
            string key;

            switch (testCase)
            {
                case 1:
                    key = Utilities.amf3EmptyRoomTTLPropertyKey;
                    break;
                case 2:
                    key = Utilities.amf3MasterClientIdPropertyKey;
                    break;
                case 3:
                    key = Utilities.amf3PlayerTTLPropertyKey;
                    break;
                case 4:
                    key = Utilities.amf3MaxPlayerPropertyKey;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(testCase));
            }

            var opRequest = new OperationRequest
            {
                OperationCode = (byte)OperationCode.SetProperties,
                Parameters = new Dictionary<byte, object>
                {
                    {
                        251, new Hashtable
                        {
                            { key, "wrong value"}
                        }
                    },
                }
            };

            var request = new SetPropertiesRequest(Protocol.Json, opRequest);
            Assert.That(request.IsValid, Is.False, request.GetErrorMessage());
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        public void WellKnownPropsFromJsonWrongExpectedValues(int testCase)
        {
            string key;

            switch (testCase)
            {
                case 1:
                    key = Utilities.amf3EmptyRoomTTLPropertyKey;
                    break;
                case 2:
                    key = Utilities.amf3MasterClientIdPropertyKey;
                    break;
                case 3:
                    key = Utilities.amf3PlayerTTLPropertyKey;
                    break;
                case 4:
                    key = Utilities.amf3MaxPlayerPropertyKey;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(testCase));
            }

            var opRequest = new OperationRequest
            {
                OperationCode = (byte)OperationCode.SetProperties,
                Parameters = new Dictionary<byte, object>
                {
                    {
                        231, new Hashtable
                        {
                            { key, "wrong value"}
                        }
                    },
                }
            };

            var request = new SetPropertiesRequest(Protocol.Json, opRequest);
            Assert.That(request.IsValid, Is.False, request.GetErrorMessage());
        }

    }
}
