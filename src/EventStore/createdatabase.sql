PRAGMA foreign_keys=OFF;
BEGIN TRANSACTION;
CREATE TABLE events(
    eventId text primary key not null,
    streamId text not null,
    data blob not null,
    version integer not null, 
    eventType text not null);

CREATE TABLE streams(
    streamId text primary key not null,
    type text not null,
    version integer not null);

CREATE INDEX event_stream_index on events (streamId);
COMMIT;
