namespace Photon.Hive.Common
{
    public class EventCacheSettings
    {
        public int SlicesCount { get; set; } = 1000;

        public int EventsCount { get; set; } = 50000;

        public int ActorEventsCount { get; set; } = 50000;
    }
}
