using System.Collections.Generic;

namespace EventStore
{
    public interface IEventStore
    {
        IEnumerable<StreamEvent> GetEventsFor(string stream);
        void SaveChanges(string stream, long originatingVersion, IEnumerable<EventData> events);

    }
}
