using Photon.Hive.Operations;
using Photon.LoadBalancing.Diagnostic;
using Photon.SocketServer;

namespace Photon.LoadBalancing.Operations
{
    public class MSJoinGameRequest : JoinGameRequest
    {
        private long startTravelTime;

        public MSJoinGameRequest(IRpcProtocol protocol, OperationRequest operationRequest, string userId)
        : base(protocol, operationRequest, userId)
        {
            
        }

        public void OnInsert()
        {
            this.startTravelTime = GetTimestamp();
            LBPerformanceCounters.MSJoinGameEnq.Increment();
        }

        protected override void OnCompleted()
        {
            base.OnCompleted();

            var ts = GetTimestamp();
            if (this.StartTime != 0)
            {
                LBPerformanceCounters.MSJoinGameET.Increment((ts - this.StartTime) * 1000);
            }

            //LBPerformanceCounters.MSJoinGameTT.Increment((ts - this.startTravelTime) * 1000);
            LBPerformanceCounters.MSJoinGameEnq.Decrement();
        }
    }
}
