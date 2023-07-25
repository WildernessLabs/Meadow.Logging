using System;
using System.Threading.Tasks;
using Meadow.Devices;
using Meadow.Hardware;

namespace Meadow.Logging.Samples;

public class MeadowApp : App<F7FeatherV2>
{
    public override async Task Initialize()
    {
        Resolver.Log.Info($"Initializing...");
        Resolver.Log.AddProvider(new CloudLogger());

        // make sure to set creds in wifi.config.yaml
        
        var count = 1;
        while (true)
        {
            Resolver.Log.Info($"log loop {count++} - {DateTime.UtcNow}");
            await Task.Delay(60 * 1000);
        }
    }
}