namespace EventStore
{
    public class Stream
    {
        public string StreamId { get; }
        public string Type { get; }
        public long Version { get; }

        public Stream(string streamId, string type, long version)
        {
            StreamId = streamId;
            Type = type;
            Version = version;
        }
    }
}
