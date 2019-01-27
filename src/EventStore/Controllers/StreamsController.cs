using System.Collections;
using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MediatR;
using System.Threading.Tasks;
using System.Threading;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Text;

namespace EventStore
{
    [Route("[controller]")]
    [ApiController]
    public class StreamsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StreamsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("{stream}")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> WriteEvents(string stream, [FromBody]JObject body)
        {                        
            var o = (long)body["originatingVersion"];
            JArray eventsArray = (JArray)body["events"];
            var events = eventsArray.Select(e => 
            {
                var eventIdSpan = ((string)e["eventId"]).AsSpan();
                if (Guid.TryParse(eventIdSpan, out var eventId))
                {
                    var eventType = (string)e["eventType"];
                    var data = Encoding.UTF8.GetBytes(e["data"].ToString(Formatting.None));
                    return new EventData(eventId, eventType, data);
                }
                else
                {
                    return null;
                }
            }).ToList();

            var cmd = new WriteEventCommand(stream, o, events);
            var result = await _mediator.Send(cmd);

            if (result)
                return Created("http://localhost:5000/streams/" + stream, events);
            else
                return BadRequest();
        }
    }

    public class WriteEventCommand : IRequest<bool>
    {
        public string Stream { get; }
        public long OriginatingVersion { get; }
        public IEnumerable<EventData> Events { get; }

        public WriteEventCommand(string stream, long originatingVersion, IEnumerable<EventData> events)
        {
            Stream = stream;
            OriginatingVersion = originatingVersion;
            Events = events;
        } 
    }

    public class WriteEventsCommandHandler : IRequestHandler<WriteEventCommand, bool>
    {
        private readonly IEventStore _eventStore;

        public WriteEventsCommandHandler(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }
        public Task<bool> Handle(WriteEventCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _eventStore.SaveChanges(request.Stream, request.OriginatingVersion, request.Events);
                return Task.FromResult(true);
            }
            catch (Exception)
            {
                return Task.FromResult(false);
            }
        }
    }
}