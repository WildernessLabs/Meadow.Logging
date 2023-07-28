using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Meadow.Devices;
using Meadow.Hardware;

namespace Meadow.Logging.Samples;

public class MeadowApp : App<F7FeatherV2>
{
    public override async Task Initialize()
    {
        Resolver.Log.Info($"Initializing...");
        var cloudLogger = new CloudLogger();
        Resolver.Log.AddProvider(cloudLogger);
        Resolver.Services.Add(cloudLogger);

        // make sure to set creds in wifi.config.yaml

        var count = 1;
        Random r = new Random();
        while (true)
        {
            // send a cloud log
            Resolver.Log.Info($"log loop {count++}");
            
            // send a cloud event
            var cl = Resolver.Services.Get<CloudLogger>();
            cl.LogEvent(0, "my first event", new Dictionary<string, object>()
            {
                { "temperature", r.Next(80, 110) },
                { "city", "log angeles" }
            });
            
            await Task.Delay(60 * 1000);
        }
    }
}