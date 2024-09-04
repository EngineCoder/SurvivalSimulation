// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SetPropertiesRequest.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The set properties operation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;

using Photon.Hive.Common;
using Photon.Hive.Plugin;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using Photon.SocketServer.Rpc.Protocols;


namespace Photon.Hive.Operations
{
    [Flags]
    public enum ValuesUpdateFlags
    {
        NoChanges,
        ThereAreChanges,
        ExpectedUsers,
    }
    /// <summary>
    /// The set properties operation.
    /// </summary>
    public class SetPropertiesRequest : Operation, ISetPropertiesRequest
    {
        #region .ctr

        /// <summary>
        /// Initializes a new instance of the <see cref="SetPropertiesRequest"/> class.
        /// </summary>
        /// <param name="protocol">
        /// The protocol.
        /// </param>
        /// <param name="operationRequest">
        /// Operation request containing the operation parameters.
        /// </param>
        public SetPropertiesRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
            if (!this.DataValidation())
            {
                return;
            }
            // special handling for game and actor properties send by AS3/Flash (Amf3 protocol) or JSON clients
            if (protocol.ProtocolType == ProtocolType.Json)
            {
                if (this.UpdatingGameProperties)
                {
                    Utilities.ConvertAs3WellKnownPropertyKeys(this.Properties, null, operationRequest.RequestMetaData?[(byte)ParameterKey.Properties], null);
                    Utilities.ConvertAs3WellKnownPropertyKeys(this.ExpectedValues, null, null, null);
                }
                else
                {
                    Utilities.ConvertAs3WellKnownPropertyKeys(null, this.Properties, null, operationRequest.RequestMetaData?[(byte)ParameterKey.Properties]);
                    Utilities.ConvertAs3WellKnownPropertyKeys(null, this.ExpectedValues, null, null);
                }
            }

            this.wellKnownPropertiesCache = new WellKnownProperties();
            if (this.UpdatingGameProperties)
            {
                this.isValid = this.wellKnownPropertiesCache.TryGetProperties(this.Properties, out this.errorMessage, operationRequest.RequestMetaData?[(byte)ParameterKey.Properties]);
                if (this.IsValid && this.ExpectedValues != null)
                {
                    this.isValid = WellKnownProperties.UpdateNumericWellKnownPropertiesTypes(this.ExpectedValues,
                        out this.errorMessage, operationRequest.RequestMetaData?[(byte)ParameterKey.ExpectedValues]);
                }
            }
        }

        public SetPropertiesRequest(int actorNr, Hashtable properties, Hashtable expected, bool broadcast)
        {
            this.ActorNumber = actorNr;
            this.Properties = properties;
            this.ExpectedValues = expected;
            this.Broadcast = broadcast;

            if (!this.DataValidation())
            {
                return;
            }

            if (this.UpdatingGameProperties)
            {
                this.wellKnownPropertiesCache = new WellKnownProperties();
                this.isValid = this.wellKnownPropertiesCache.TryGetProperties(this.Properties, out this.errorMessage, null);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SetPropertiesRequest"/> class.
        /// </summary>
        public SetPropertiesRequest()
        {
            this.wellKnownPropertiesCache = new WellKnownProperties();
        }

        #endregion

        #region Serializable properties

        /// <summary>
        /// Gets or sets ActorNumber.
        /// </summary>
        [DataMember(Code = (byte)ParameterKey.ActorNr, IsOptional = true)]
        public int ActorNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Broadcast.
        /// </summary>
        [DataMember(Code = (byte)ParameterKey.Broadcast, IsOptional = true)]
        public bool Broadcast { get; set; }

        /// <summary>
        /// Gets or sets Properties.
        /// </summary>
        [DataMember(Code = (byte)ParameterKey.Properties)]
        public Hashtable Properties { get; set; }

        /// <summary>
        /// Expected values for properties, which we are going to set
        /// </summary>
        [DataMember(Code = (byte)ParameterKey.ExpectedValues, IsOptional = true)]
        public Hashtable ExpectedValues { get; set; }
        [DataMember(Code = (byte)ParameterKey.WebFlags, IsOptional = true)]
        private object internalWebFlags { get; set; }

        #endregion

        #region Helper properties and fields

        public bool HttpForward
        {
            get => Plugins.Common.WebFlags.ShouldHttpForward(this.WebFlags);
            set => this.WebFlags = value ? Plugins.Common.WebFlags.HttpForward : (byte)0x0;
        }

        public byte WebFlags { get; set; }

        public byte OperationCode => this.OperationRequest.OperationCode;

        public Dictionary<byte, object> Parameters => this.OperationRequest.Parameters;

        public bool UsingCAS => this.ExpectedValues != null && this.ExpectedValues.Count != 0;

        public bool UpdatingGameProperties => this.ActorNumber <= 0;

        public WellKnownProperties wellKnownPropertiesCache { get; }

        //cached values of game properties which this request contains
        public byte? newMaxPlayer => this.wellKnownPropertiesCache.MaxPlayer;
        public bool? newIsOpen => this.wellKnownPropertiesCache.IsOpen;
        public bool? newIsVisible => this.wellKnownPropertiesCache.IsVisible;
        public int? MasterClientId => this.wellKnownPropertiesCache.MasterClientId;
        public string[] ExpectedUsers => this.wellKnownPropertiesCache.ExpectedUsers;
        public object[] newLobbyProperties => this.wellKnownPropertiesCache?.LobbyProperties;

        public Hashtable newGameProperties;
        public Actor SenderActor;
        public Actor TargetActor;
        public IEnumerable<Actor> PublishTo;

        /// <summary>
        /// whether we updated any of property values during this call
        /// </summary>
        public ValuesUpdateFlags ValuesUpdated { get; set; }

        /// <summary>
        /// whether wel known properties will be updated
        /// </summary>
        public bool UpdateMasterOnWelKnownsUpdate { get; set; }

        #endregion

        #region Methods

        private bool DataValidation()
        {
            if (!this.IsValid)
            {
                return false;
            }

            if (this.ActorNumber < 0)
            {
                this.ActorNumber = 0;
            }

            return this.ValidateWebFlags();
        }

        private bool ValidateWebFlags()
        {
            var value = this.internalWebFlags;
            if (value == null)
            {
                return true;
            }

            if (value is bool bvalue)
            {
                this.WebFlags = bvalue ? Plugins.Common.WebFlags.HttpForward : (byte)0x0;
                return true;
            }

            try
            {
                WebFlags = Convert.ToByte(value);
            }
            catch(Exception e)
            {
                this.isValid = false;
                this.errorMessage = e.Message;
                return false;
            }

            return true;
        }

        #endregion
    }
}