using System;
using System.Threading.Tasks;
using System.Linq;

using Foundation;
using UIKit;

using WindowsAzure.Messaging;


using Q42.HueApi;
using Q42.HueApi.ColorConverters;
using Q42.HueApi.ColorConverters.Original;

namespace BusyLights.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        SBNotificationHub Hub { get; set; }
        const string ConnectionString = "Endpoint=sb://busylights.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=C4qfCLSfVZM1oqvjwuawOVRhXOXi31xdOX47nxyipLA=";
        const string NotificationHubPath = "BusyLights";

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();

            LoadApplication(new App());

            UIApplication.SharedApplication.RegisterForRemoteNotifications();

            return base.FinishedLaunching(app, options);
        }

        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            Hub = new SBNotificationHub(ConnectionString, NotificationHubPath);


            Hub.UnregisterAllAsync(deviceToken, (error) =>
            {
                if (error != null)
                    return;

                Hub.RegisterNativeAsync(deviceToken, null, (registerError) =>
                {
                    if (registerError != null)
                    {
                        Console.WriteLine("successful register");
                    }
                });
            });
        }

        public override void ReceivedRemoteNotification(UIApplication application, NSDictionary userInfo)
        {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(async () => await TurnLightRed());
        }

        public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        {
            Settings.IWasHere = Settings.IWasHere + 1;

            //base.DidReceiveRemoteNotification(application, userInfo, completionHandler);

            //var lightMessage = new LightMessage();
            //Xamarin.Forms.MessagingCenter.Send(lightMessage, "turn_light_on");

            Xamarin.Forms.Device.BeginInvokeOnMainThread(async () => await TurnLightRed());

            completionHandler(UIBackgroundFetchResult.NewData);
        }

        async Task TurnLightRed()
        {


            Settings.IWasHere = Settings.IWasHere + 1;

            var bridgeLocator = new HttpBridgeLocator();
            var ips = await bridgeLocator.LocateBridgesAsync(TimeSpan.FromSeconds(30));

            var client = new LocalHueClient(ips.First().IpAddress);

            if (!client.IsInitialized && !string.IsNullOrEmpty(Settings.HueKey))
            {
                client.Initialize(Settings.HueKey);
            }
            else
            {
                //await DisplayAlert("Not paired", "App not paired to a bridge, hit the register button.", "OK");

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
        }
    }
}
