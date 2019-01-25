namespace EventStore
{
    public class StreamEvent
    {
        public string StreamId { get; }
        public byte[] Data { get; }
        public long Version { get; }
        public string EventType { get; }
        public string EventId { get; }

        public StreamEvent(string streamId, byte[] data, long version, string eventType, string eventId)
        {
            StreamId = streamId;
            Data = data;
            Version = version;
            EventType = eventType;
            EventId = eventId;
        }
    }
}
