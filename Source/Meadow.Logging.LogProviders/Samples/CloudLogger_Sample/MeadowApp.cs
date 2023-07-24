using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Meadow;
using Meadow.Devices;
using Meadow.Hardware;
using Meadow.Logging;

namespace ClassLibrary1;

public class MeadowApp : App<F7FeatherV2>
{
    public override async Task Initialize()
    {
        Resolver.Log.Info($"Initializing...");
        Resolver.Log.AddProvider(new CloudLogger());
        
        await ConnectWifi();

        var count = 1;
        while (true)
        {
            Resolver.Log.Info($"log loop {count++}");
            await Task.Delay(60 * 1000);
        }
    }
    
    private async Task ConnectWifi()
    {
        var SSID = "ssid";
        var PASSCODE = "pass";

        Resolver.Log.Info("Connecting to network...");

        try
        {
            var wifi = Device.NetworkAdapters.Primary<IWiFiNetworkAdapter>();
            await wifi.Connect(SSID, PASSCODE);

            wifi.NetworkConnected += async (s, e) =>
            {
                Resolver.Log.Info("Network connected");
            };
        }
        catch (Exception ex)
        {
            Resolver.Log.Error($"Error connecting to network: {ex.Message}");
        }
    }
}