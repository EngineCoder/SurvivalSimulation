namespace Photon.Common.Configuration
{
    public class HttpQueueSettings
    {
        public int HttpRequestTimeout { get; set; } = 30;

        public int MaxBackoffTime { get; set; } = 30;

        public int MaxConcurrentRequests { get; set; } = 1;

        public int MaxErrorRequests { get; set; } = 10;

        public int MaxQueuedRequests { get; set; } = 100;

        public int MaxTimedOutRequests { get; set; } = 10;

        public int QueueTimeout { get; set; } = 100;

        public int ReconnectInterval { get; set; } = 60;

        public int LimitHttpResponseMaxSize { get; set; } = 20_000;
    }
}
