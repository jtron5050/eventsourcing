using System;

namespace EventStore
{
    public class EventData
    {
        public Guid EventId { get; }
        public string EventType { get; }
        public byte[] Data { get; }

        public EventData(Guid eventId, string eventType, byte[] data)
        {
            EventId = eventId;
            EventType = eventType;
            Data = data;
        }
    }
}
