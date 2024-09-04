using Photon.Hive.Operations;
using Photon.LoadBalancing.Diagnostic;
using Photon.SocketServer;

namespace Photon.LoadBalancing.Operations
{
    public class MSCreateGameRequest : CreateGameRequest
    {
        private long startTravelTime;

        public MSCreateGameRequest(IRpcProtocol protocol, OperationRequest operationRequest, string userId)
        : base(protocol, operationRequest, userId)
        {
            
        }

        protected override void OnCompleted()
        {
            base.OnCompleted();
            var ts = GetTimestamp();
            if (this.StartTime != 0)
            {
                LBPerformanceCounters.MSCreateGameET.Increment((ts - this.StartTime) * 1000);
            }

            //LBPerformanceCounters.MSCreateGameTT.Increment((ts - this.startTravelTime) * 1000);
            LBPerformanceCounters.MSCreateGameEnq.Decrement();
        }

        /// <summary>
        /// we call it once we insert operation to fiber
        /// </summary>
        public void OnInserted()
        {
            this.startTravelTime = GetTimestamp();
            LBPerformanceCounters.MSCreateGameEnq.Increment();
        }
    }
}
