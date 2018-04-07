using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using BJUTDUHelperXamarin.Services;
using UserNotifications;
using Xamarin.Forms;
using BJUTDUHelperXamarin.iOS.Services;
using WindowsAzure.Messaging;
using Prism;
using Prism.Unity;

namespace BJUTDUHelperXamarin.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
       
        private SBNotificationHub Hub { get; set; }
        public static event EventHandler<RegistedEventArgs> Registed;

        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            LoadApplication(new App(new iOSInitializer()));

            

            //// Request Permissions
            //if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            //{
            //    // Request Permissions
            //    UNUserNotificationCenter.Current.RequestAuthorization(UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound, (granted, error) =>
            //    {
            //        // Do something if needed
            //    });
            //}
            //else if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            //{
            //    var notificationSettings = UIUserNotificationSettings.GetSettingsForTypes(
            //    UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound, null);

            //    app.RegisterUserNotificationSettings(notificationSettings);
            //}

            DependencyService.Register<ToastNotificator>(); // Register your dependency

            return base.FinishedLaunching(app, options);
        }

        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            Registed?.Invoke(this, new RegistedEventArgs() { DeviceToken = deviceToken });
        }
        public override void ReceivedRemoteNotification(UIApplication application, NSDictionary userInfo)
        {
            ProcessNotification(userInfo, false);
        }
        void ProcessNotification(NSDictionary options, bool fromFinishedLaunching)
        {
            // Check to see if the dictionary has the aps key.  This is the notification payload you would have sent
            if (null != options && options.ContainsKey(new NSString("aps")))
            {
                //Get the aps dictionary
                NSDictionary aps = options.ObjectForKey(new NSString("aps")) as NSDictionary;

                string alert = string.Empty;

                //Extract the alert text
                // NOTE: If you're using the simple alert by just specifying
                // "  aps:{alert:"alert msg here"}  ", this will work fine.
                // But if you're using a complex alert with Localization keys, etc.,
                // your "alert" object from the aps dictionary will be another NSDictionary.
                // Basically the JSON gets dumped right into a NSDictionary,
                // so keep that in mind.
                if (aps.ContainsKey(new NSString("alert")))
                    alert = (aps[new NSString("alert")] as NSString).ToString();

                //If this came from the ReceivedRemoteNotification while the app was running,
                // we of course need to manually process things like the sound, badge, and alert.
                if (!fromFinishedLaunching)
                {
                    //Manually show an alert
                    if (!string.IsNullOrEmpty(alert))
                    {
                        UIAlertView avAlert = new UIAlertView("Notification", alert, null, "OK", null);
                        avAlert.Show();
                    }
                }
            }
        }
    }

    public class RegistedEventArgs : EventArgs
    {
        public NSData DeviceToken { get; set; }
    }
    public class iOSInitializer : IPlatformInitializer
    {
        public void RegisterTypes(Microsoft.Practices.Unity.IUnityContainer container)
        {
            
        }
    }

}
