using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

using WindowsAzure.Messaging;

namespace BusyLights.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        SBNotificationHub Hub { get; set; }
        const string ConnectionString = "Endpoint=sb://busylights.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=";
        const string NotificationHubPath = "BusyLights";

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();

            LoadApplication(new App());

            //var settings = UIUserNotificationSettings.GetSettingsForTypes(UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound, new NSSet());

            //UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);

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
            //base.ReceivedRemoteNotification(application, userInfo);
        }

        public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        {
            //base.DidReceiveRemoteNotification(application, userInfo, completionHandler);
        }
    }
}
