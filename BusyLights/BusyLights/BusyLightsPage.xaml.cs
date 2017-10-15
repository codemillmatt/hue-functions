using Xamarin.Forms;
using Q42.HueApi;
using Q42.HueApi.ColorConverters;
using Q42.HueApi.ColorConverters.Original;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BusyLights
{
    public partial class BusyLightsPage : ContentPage
    {
        public BusyLightsPage()
        {
            InitializeComponent();

            registerButton.Clicked += async (s, e) =>
            {
                var bridgeLocator = new HttpBridgeLocator();

                var ips = await bridgeLocator.LocateBridgesAsync(TimeSpan.FromSeconds(30));

                var client = new LocalHueClient(ips.First().IpAddress);

                if (!client.IsInitialized && !string.IsNullOrEmpty(Settings.HueKey))
                {
                    client.Initialize(Settings.HueKey);

                    await DisplayAlert("Paired", "Good to go", "OK");
                }
                else
                {
                    await DisplayAlert("Press button", "Press the button on the Hue Bridge", "OK");

                    for (int i = 0; i < 30; i++)
                    {
                        try
                        {
                            pairingIndicator.IsRunning = true;

                            await Task.Delay(TimeSpan.FromSeconds(1));

                            var appKey = await client.RegisterAsync("BusyLights", "simulator");
                            Settings.HueKey = appKey;

                            await DisplayAlert("Paired", "Good to go", "OK");

                            break;
                        }
                        catch (Exception ex)
                        {
                            // swallow it for now
                        }
                        finally
                        {
                            pairingIndicator.IsRunning = false;
                        }
                    }

                    if (!client.IsInitialized)
                    {
                        await DisplayAlert("No bridge", "Couldn't pair, try again.", "OK");
                    }
                }
            };

            busyButton.Clicked += async (sender, e) =>
            {
                var bridgeLocator = new HttpBridgeLocator();
                var ips = await bridgeLocator.LocateBridgesAsync(TimeSpan.FromSeconds(30));

                var client = new LocalHueClient(ips.First().IpAddress);

                if (!client.IsInitialized && !string.IsNullOrEmpty(Settings.HueKey))
                {
                    client.Initialize(Settings.HueKey);
                }
                else
                {
                    await DisplayAlert("Not paired", "App not paired to a bridge, hit the register button.", "OK");

                    return;
                }

                var command = new LightCommand();
                var redColor = new RGBColor(220, 82, 74);
                command.TurnOn().SetColor(redColor);

                var allLights = await client.GetLightsAsync();

                foreach (var light in allLights)
                {
                    if (light.Name.Equals("hue go 1", StringComparison.OrdinalIgnoreCase))
                    {
                        await client.SendCommandAsync(command, new[] { light.Id });
                    }
                }
            };

            freeButton.Clicked += async (sender, e) =>
            {
                var bridgeLocator = new HttpBridgeLocator();
                var ips = await bridgeLocator.LocateBridgesAsync(TimeSpan.FromSeconds(30));

                var client = new LocalHueClient(ips.First().IpAddress);

                if (!client.IsInitialized && !string.IsNullOrEmpty(Settings.HueKey))
                {
                    client.Initialize(Settings.HueKey);
                }
                else
                {
                    await DisplayAlert("Not paired", "App not paired to a bridge, hit the register button.", "OK");

                    return;
                }

                var command = new LightCommand();
                var freeColor = new RGBColor(124, 86, 187);
                command.TurnOn().SetColor(freeColor);

                var allLights = await client.GetLightsAsync();

                foreach (var light in allLights)
                {
                    if (light.Name.Equals("hue go 1", StringComparison.OrdinalIgnoreCase))
                    {
                        await client.SendCommandAsync(command, new[] { light.Id });
                    }
                }

            };

            offButton.Clicked += async (sender, e) =>
            {
                var bridgeLocator = new HttpBridgeLocator();
                var ips = await bridgeLocator.LocateBridgesAsync(TimeSpan.FromSeconds(30));

                var client = new LocalHueClient(ips.First().IpAddress);

                if (!client.IsInitialized && !string.IsNullOrEmpty(Settings.HueKey))
                {
                    client.Initialize(Settings.HueKey);
                }
                else
                {
                    await DisplayAlert("Not paired", "App not paired to a bridge, hit the register button.", "OK");

                    return;
                }

                var command = new LightCommand();
                command.TurnOff();

                var allLights = await client.GetLightsAsync();

                foreach (var light in allLights)
                {
                    if (light.Name.Equals("hue go 1", StringComparison.OrdinalIgnoreCase))
                    {
                        await client.SendCommandAsync(command, new[] { light.Id });
                    }

                }
            };
        }
    }
}
