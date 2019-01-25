using System;
using System.Collections.Generic;
using System.Data.SQLite;
using Dapper;
using System.Text;
using System.Linq;
using Newtonsoft.Json;

namespace EventStore
{
    public class EventStore : IEventStore
    {
        private const string SELECT_EVENTS_SQL = "select streamId, data, version, eventType, eventId from events where streamid = @streamId order by version";
        private const string INSERT_EVENT_FOR_STREAM_SQL = "insert into events (streamId, data, version) values (@streamId, @data, @version)";
        private const string UPDATE_STREAM_VERSION_SQL = "update streams set version = @version where streamId = @streamId";
        private const string INSERT_NEW_STREAM_SQL = "insert into streams (streamid, type, version) values (@streamId, @type, 0)";
        private const string GET_STREAM_VERSION_SQL = "select version from streams where streamId = @streamId";

        public EventStore()
        {

        }

        public IEnumerable<StreamEvent> GetEventsFor(string stream)
        {
            using (var connection = new SQLiteConnection("Data Source=../../eventstore.db;PRAGMA journal_mode=WAL"))
            {
                connection.Open();
                return connection.Query<StreamEvent>(
                    SELECT_EVENTS_SQL,
                    new { streamId = stream });
            }
        }

        public void SaveChanges(string stream, long originatingVersion, IEnumerable<EventData> events)
        {
            using (var connection = new SQLiteConnection("Data Source=../../eventstore.db;PRAGMA journal_mode=WAL"))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var version = connection.ExecuteScalar<int?>(GET_STREAM_VERSION_SQL, new { streamId = stream }, transaction);
                    if (version == null)
                    {
                        connection.Execute(INSERT_NEW_STREAM_SQL, new { streamId = stream, type = stream }, transaction);
                        version = 0;
                    }

                    if (originatingVersion != version)
                    {
                        transaction.Rollback();
                        throw new InvalidOperationException("concurrency problem");
                    }

                    var e = events
                        .Select((dynamic @event) => new
                        {
                            streamId = stream,
                            version = ++version,
                            data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event)),
                            eventType = @event.GetType(),
                            EventId = @event.EventId
                        }).ToArray();

                    connection.Execute(INSERT_EVENT_FOR_STREAM_SQL, e, transaction);
                    connection.Execute(UPDATE_STREAM_VERSION_SQL, new { streamId = stream, version = version }, transaction);
                    transaction.Commit();
                }
            }
        }
    }
}
