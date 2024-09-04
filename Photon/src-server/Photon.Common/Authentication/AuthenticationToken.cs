
using System;
using System.Collections.Generic;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace Photon.Common.Authentication
{
    public class AuthenticationToken : DataContract
    {
        private static readonly IRpcProtocol serializationProtocol = Protocol.GpBinaryV18;

        protected AuthenticationToken(IRpcProtocol protocol, IDictionary<byte, object> dataMembers)
            : base(protocol, dataMembers)
        {
        }

        // we are using the Version as "EventCode". 
        public AuthenticationToken()
        {
        }

        public byte Version => 1;

        // converts the internal "ValidToTicks" expiration timestamp to a DateTime (based on UTC.Now) 
        public DateTime ExpireAt => new DateTime(this.ExpireAtTicks, DateTimeKind.Utc);

        public DateTime FinalExpireAt => new DateTime(this.FinalExpireAtTicks, DateTimeKind.Utc);

        public bool IsFinalExpireAtUsed => this.FinalExpireAtTicks != 0;

        [DataMember(Code = 1, IsOptional = false)]
        public long ExpireAtTicks { get; set; }

        [DataMember(Code = 2, IsOptional = true)]
        public string ApplicationId { get; set; }

        [DataMember(Code = 3, IsOptional = true)]
        public string ApplicationVersion { get; set; }

        [DataMember(Code = 4, IsOptional = true)]
        public string UserId { get; set; }

        [DataMember(Code = 8, IsOptional = true)]
        public Dictionary<string, object> AuthCookie { get; set; }

        [DataMember(Code = 10, IsOptional = true)]
        public string SessionId { get; set; }

        [DataMember(Code = 11, IsOptional = true)]
        public int Flags { get; set; }

        [DataMember(Code = 13, IsOptional = true)]
        public Dictionary<byte, object> EncryptionData { get; set; }

        /// <summary>
        /// When this point in time reached we do not extend token validity time
        /// </summary>
        [DataMember(Code = 14, IsOptional = true)]
        public long FinalExpireAtTicks { get; set; }

        [DataMember(Code = 15, IsOptional = true)]
        public string TokenIssuer { get; set; }

        /// <summary>
        /// what custom auth provider was used for this client if any
        /// </summary>
        [DataMember(Code = 16, IsOptional = true)]
        public byte? CustomAuthProvider { get; set; }

        [DataMember(Code = 115, IsOptional = true)]
        public bool NoTokenAuthOnMaster { get; set; }

        [DataMember(Code = 116, IsOptional = true)]
        public string ExpectedGS { get; set; }

        [DataMember(Code = 117, IsOptional = true)]
        public string ExpectedGameId { get; set; }

        [DataMember(Code = 118, IsOptional = true)]
        public bool CustomAuthUserIdUsed { get; set; }

        public virtual bool AreEqual(AuthenticationToken rhs)
        {
            return this.UserId == rhs.UserId && this.SessionId == rhs.SessionId;
        }

        public virtual byte[] Serialize()
        {
            return serializationProtocol.SerializeEventData(new EventData(this.Version, this));
        }

        public static bool TryDeserialize(byte[] data, out AuthenticationToken token, out string errorMsg)
        {
            token = null;
            if (!serializationProtocol.TryParseEventData(data, out var eventData, out errorMsg))
            {
                return false;
            }

            // code = version
            switch (eventData.Code)
            {
                default:
                    errorMsg = $"Unknown version of Token: {eventData.Code}";
                    return false;

                case 1:
                    token = new AuthenticationToken(serializationProtocol, eventData.Parameters);
                    return true;
            }
        }
    }
}
