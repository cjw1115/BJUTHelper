using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Com.Baidu.Android.Pushservice;
using WindowsAzure.Messaging;
using BJUTDUHelperXamarin.Services;

[assembly: Dependency(typeof(BJUTDUHelperXamarin.Droid.Services.NotificationHubDroid))]
namespace BJUTDUHelperXamarin.Droid.Services
{
    public class BiaduConfigurationSettings
    {
        public static string API_KEY = "User yourself api key from Baidu";
    }
    public class NotificationHubDroid: INotificationHub
    {
        WindowsAzure.Messaging.NotificationHub hub;
        string[] userMarks;

        bool? isRegisted = null;
        public static void InitNotification()
        {
            PushManager.StartWork(Xamarin.Forms.Forms.Context, PushConstants.LoginTypeApiKey, BiaduConfigurationSettings.API_KEY);
        }
        public async Task<bool> InitNotificationHubAsync(string hubName, string hubConnectionString, string[] marks)
        {
            isRegisted = null;
            userMarks = marks;


            //Java.Lang.JavaSystem.LoadLibrary("libbdpush_V2_7");

            NotifyMessageReceiver.Binded += BaiduBinded;
            PushSettings.EnableDebugMode(Xamarin.Forms.Forms.Context, true);
            PushManager.StartWork(Xamarin.Forms.Forms.Context, PushConstants.LoginTypeApiKey, BiaduConfigurationSettings.API_KEY);
            
            
            

            while (isRegisted == null)
            {
                await Task.Delay(50);
            }
            return isRegisted.Value;
        }
        public async void BaiduBinded(object sender,BindedEventArgs e)
        {
            //PushManager.SetTags(Xamarin.Forms.Forms.Context, userMarks);

            if (hub == null)
            {
                hub = new WindowsAzure.Messaging.NotificationHub(ConfigurationSettings.HUB_NAME, ConfigurationSettings.HUB_CONNEXTION_STRING,Xamarin.Forms.Forms.Context);
            }
            await Task.Run(() =>
            {
                using (var re = hub.RegisterBaidu(e.UserID, e.ChannelID, userMarks))
                {
                    if (!string.IsNullOrEmpty(re.RegistrationId))
                    {
                        isRegisted = true;
                    }
                    else
                    {
                        isRegisted = false;
                    }
                }
            });

            NotifyMessageReceiver.Binded -= BaiduBinded;
        }
    }
}
