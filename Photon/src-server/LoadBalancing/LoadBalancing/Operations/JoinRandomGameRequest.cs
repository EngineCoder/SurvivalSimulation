// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JoinRandomGameRequest.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the JoinRandomGameRequest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using ExitGames.Logging;

using Photon.LoadBalancing.Common;
using Photon.LoadBalancing.Diagnostic;
using Photon.LoadBalancing.MasterServer;
using Photon.SocketServer.Diagnostics;

using System;

namespace Photon.LoadBalancing.Operations
{
    #region using directives

    using Photon.Hive.Common;
    using Photon.Hive.Operations;
    using Photon.SocketServer;
    using Photon.SocketServer.Rpc;
    using Photon.SocketServer.Rpc.Protocols;

    using System.Collections;

    #endregion

    public class JoinRandomGameRequest : Operation
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        private static readonly LogCountGuard LogGuard = new LogCountGuard(new TimeSpan(0, 0, 0, 10));

        private long startTravelTime;

        public JoinRandomGameRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
            this.ValidateAndUpdateData();
            if (!this.IsValid)
            {
                return;
            }

            // special handling for game properties send by JS clients
            if (protocol.ProtocolType == ProtocolType.Json)
            {
                Utilities.ConvertAs3WellKnownPropertyKeys(this.GameProperties, null, operationRequest.RequestMetaData?[(byte)ParameterKey.GameProperties], null);
                if (this.GameProperties == null)
                {
                    return;
                }

                var propsCache = new WellKnownProperties();
                this.isValid = propsCache.TryGetProperties(this.GameProperties, out this.errorMessage, operationRequest.RequestMetaData?[(byte)ParameterKey.GameProperties]);
            }

            if (this.IsValid)
            {
                this.CheckQueryData();
            }
        }

        public JoinRandomGameRequest()
        {
        }

        [DataMember(Code = (byte)ParameterKey.GameProperties, IsOptional = true)]
        public Hashtable GameProperties { get; set; }

        [DataMember(Code = (byte)ParameterCode.MatchMakingType, IsOptional = true)]
        public byte JoinRandomType { get; set; }

        [DataMember(Code = (byte)ParameterKey.Data, IsOptional = true)]
        public string QueryData { get; set; }

        [DataMember(Code = (byte)ParameterCode.LobbyName, IsOptional = true)]
        public string LobbyName { get; set; }

        [DataMember(Code = (byte)ParameterCode.LobbyType, IsOptional = true)]
        public byte LobbyType { get; set; }

        [DataMember(Code = (byte)ParameterCode.AddUsers, IsOptional = true)]
        public string[] AddUsers { get; set; }

        #region required for CreateIfNotExist

        /// <summary>
        /// Gets or sets the name of the game (room).
        /// </summary>
        [DataMember(Code = (byte)ParameterKey.GameId, IsOptional = true)]
        public virtual string GameId { get; set; }

        [DataMember(Code = (byte)ParameterKey.JoinMode, IsOptional = true)]
        protected object InternalJoinMode { get; set; }

        // no ParameterKey:
        // for backward compatibility this is set through InternalJoinMode
        public byte JoinMode { get; set; }

        // no ParameterKey:
        // for backward compatibility this is set through InternalJoinMode
        public bool CreateIfNotExists
        {
            get
            {
                return this.JoinMode == JoinModes.CreateIfNotExists;
            }
        }

        /// <summary>
        /// GameProperties for game creation. If game has to be created GameProperties (key 248, used for filter) are replaced with these values.
        /// </summary>
        [DataMember(Code = (byte)ParameterKey.Properties, IsOptional = true)]
        public Hashtable Properties { get; set; }

        #endregion

        public void OnInsert()
        {
            this.startTravelTime = GetTimestamp();
            LBPerformanceCounters.MSJoinRandomGameEnq.Increment();
        }

        #region Methods

        protected override void OnCompleted()
        {
            base.OnCompleted();
            var ts = GetTimestamp();
            if (this.StartTime != 0)
            {
                LBPerformanceCounters.MSJoinRandomGameET.Increment((ts - this.StartTime) * 1000);
            }

            //LBPerformanceCounters.MSJoinRandomGameTT.Increment((ts - this.startTravelTime) * 1000);
            LBPerformanceCounters.MSJoinRandomGameEnq.Decrement();
        }

        private void CheckQueryData()
        {
            string errorMsg;
            if (!this.QueryHasSQLInjection(out errorMsg))
            {
                return;
            }

            if (!MasterServerSettings.Default.OnlyLogQueryDataErrors)
            {
                this.isValid = false;
                this.errorMessage = errorMsg;
            }

            log.WarnFormat(LogGuard, "QueryData contains SQL injection. query:{0}. ErrorMsg:{1}", this.QueryData, errorMsg);
        }

        private bool QueryHasSQLInjection(out string errorMsg)
        {
            if (string.IsNullOrEmpty(this.QueryData))
            {
                errorMsg = string.Empty;
                return false;
            }

            //we allow semicolon for multi query feature (sequential until game is found)
//            if (this.QueryData.Contains(";"))
//            {
//                errorMsg = LBErrorMessages.NotAllowedSemicolonInQuereyData;
//                return true;
//            }

            var wrongWords = MasterServerSettings.Default.SqlQueryBlockList.Split(';');
            foreach (var word in wrongWords)
            {
                if (this.QueryData.Contains(word))
                {
                    errorMsg = string.Format(LBErrorMessages.NotAllowedWordInQueryData, word);
                    return true;
                }
            }
            errorMsg = string.Empty;
            return false;
        }

        private void ValidateAndUpdateData()
        {
            if (!this.IsValid)
            {
                return;
            }

            if (!this.ValidateAndUpdateJoinMode())
            {
                return;
            }
        }

        private bool ValidateAndUpdateJoinMode()
        {
            var value = this.InternalJoinMode;
            if (value is bool bvalue && this.JoinMode == JoinModes.JoinOnly && bvalue)
            {
                this.JoinMode = JoinModes.CreateIfNotExists;
                return true;
            }

            try
            {
                this.JoinMode = Convert.ToByte(value);
            }
            catch (Exception e)
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