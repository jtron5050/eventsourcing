using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace EventStore
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();

            // var logger = host.Services.GetRequiredService<ILogger<Program>>();
            // var e = new EventStore();

            // var events = e.GetEventsFor("new-stream");

            // foreach (var item in events)
            // {
            //     logger.LogInformation("streamId: {streamid}, data: {data}, version: {version}, eventType: {eventType}, eventId: {eventId}",
            //        item.StreamId, Encoding.UTF8.GetString(item.Data), item.Version, item.EventType, item.EventId);
            // }

            // var orig = events.Last().Version;

            // var data = JsonConvert.SerializeObject(new DerpHasBeenDerpedEvent()); 
            // var ed = new EventData(Guid.NewGuid(), d.GetType().Name, Encoding.ASCII.GetBytes(data));
            // e.SaveChanges("new-stream", orig, new List<EventData> { ed });

            // events = e.GetEventsFor("new-stream");

            //  foreach (var item in events)
            //  {
            //      logger.LogInformation("streamId: {streamid}, data: {data}, version: {version}",
            //         item.StreamId, Encoding.UTF8.GetString(item.Data), item.Version);
            //  }

            host.Run();           

        }

        static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }

    public class DerpHasBeenDerpedEvent
    {
        public Guid Id { get; }
        public DateTime TimeStamp { get; }

        public DerpHasBeenDerpedEvent()
        {
            Id = Guid.NewGuid();
            TimeStamp = DateTime.Now;
        }
    }
}
