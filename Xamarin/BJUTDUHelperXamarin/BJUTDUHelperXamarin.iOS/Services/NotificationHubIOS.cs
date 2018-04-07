using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Foundation;
using UIKit;
using WindowsAzure.Messaging;
using BJUTDUHelperXamarin.iOS;
using BJUTDUHelperXamarin.Services;
using BJUTDUHelperXamarin.iOS.Services;
using UserNotifications;

[assembly:Xamarin.Forms.Dependency(typeof(NotificationHubIOS))]
namespace BJUTDUHelperXamarin.iOS.Services
{
    class NotificationHubIOS : INotificationHub
    {
        SBNotificationHub hub;
        string[] userMarks;
        bool? isRegisted = null;
        public async Task<bool> InitNotificationHubAsync(string hubName, string hubConnectionString, string[] marks)
        {
            AppDelegate.Registed += Registed;
            userMarks = marks;


            

            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                var pushSettings = UIUserNotificationSettings.GetSettingsForTypes(
                       UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound,
                       new NSSet());

                UIApplication.SharedApplication.RegisterUserNotificationSettings(pushSettings);
                UIApplication.SharedApplication.RegisterForRemoteNotifications();
            }
            else
            {

                UIRemoteNotificationType notificationTypes = UIRemoteNotificationType.Alert | UIRemoteNotificationType.Badge | UIRemoteNotificationType.Sound;
                UIApplication.SharedApplication.RegisterForRemoteNotificationTypes(notificationTypes);
            }


            await Task.Run(async ()=> 
            {
                while (isRegisted == null)
                {
                    await Task.Delay(50);
                }
            });

            return isRegisted.Value;
        }

        public void Registed(object sender,RegistedEventArgs e)
        {
            hub = new SBNotificationHub(ConfigurationSettings.HUB_CONNEXTION_STRING, ConfigurationSettings.HUB_NAME);
            hub.UnregisterAllAsync(e.DeviceToken, (error) =>
            {
                if (error != null)
                {
                    Console.WriteLine("Error calling Unregister: {0}", error.ToString());
                    return;
                }

                NSSet tags =new NSSet(userMarks); // create tags if you want
                
                hub.RegisterNative(e.DeviceToken, tags, out NSError regError);
                if (regError ==null)
                {
                    isRegisted = true;
                }
                else
                {
                    isRegisted = false;
                }
            });
            AppDelegate.Registed -= Registed;
        }
    }
}