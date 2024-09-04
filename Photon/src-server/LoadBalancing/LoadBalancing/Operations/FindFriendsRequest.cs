
using Photon.LoadBalancing.Diagnostic;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace Photon.LoadBalancing.Operations
{
    public static class FindFriendsOptions
    {
        public const int Default     = 0x00;
        public const int CreatedOnGS = 0x01;
        public const int Visible     = 0x02;
        public const int Open        = 0x04;
    }
    public class FindFriendsRequest : Operation
    {
        private long startTravelTime;
        public FindFriendsRequest()
        {
        }

        public FindFriendsRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }

        [DataMember(Code=1, IsOptional=false)]
        public string[] UserList { get; set; }

        [DataMember(Code=2, IsOptional=true)]
        public int OperationOptions { get; set; }


        /// <summary>
        /// we call it once we insert operation to fiber
        /// </summary>
        public void OnInserted()
        {
            this.startTravelTime = GetTimestamp();
            LBPerformanceCounters.MSFindFriendsEnq.Increment();
        }

        protected override void OnCompleted()
        {
            base.OnCompleted();

            var ts = GetTimestamp();
            if (this.StartTime != 0)
            {
                LBPerformanceCounters.MSFindFriendsET.Increment((ts - this.StartTime) * 1000);
            }

            //LBPerformanceCounters.MSFindFriendsTT.Increment((ts - this.startTravelTime) * 1000);
            LBPerformanceCounters.MSFindFriendsEnq.Decrement();

        }
    }
}
