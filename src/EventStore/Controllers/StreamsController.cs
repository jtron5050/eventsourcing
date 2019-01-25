using System.Collections;
using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MediatR;
using System.Threading.Tasks;
using System.Threading;

namespace EventStore
{
    [Route("streams")]
    [ApiController]
    public class StreamsController : ControllerBase
    {
        private readonly IMediator _mediator;
        
        public StreamsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Route("{stream}")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> WriteEvent(string stream, [FromBody]WriteEventCommand command)
        {
            var result = await _mediator.Send(command);

            if (result)
                return Created("", new { });
            else
                return BadRequest();
        }
    }


    public class WriteEventCommand : IRequest<bool>
    {
        public string Stream { get; internal set; }
        public long ExpectedVersion { get; internal set; }
        public List<EventData> Data { get; internal set; }
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
            _eventStore.SaveChanges(request.Stream, request.ExpectedVersion, request.Data);
            return Task.FromResult(true);
        }
    }
}