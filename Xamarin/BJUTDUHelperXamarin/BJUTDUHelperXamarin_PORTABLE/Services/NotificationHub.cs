
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BJUTDUHelperXamarin.Services
{
    public interface INotificationHub
    {
        Task<bool> InitNotificationHubAsync(string hubName, string hubConnectionString,string[] marks);
    }
    public class NotificationHub
    {
        public static INotificationHub GetNotificationHub()
        {
            return DependencyService.Get<INotificationHub>();
        }
    }
    public class ConfigurationSettings
    {
        public static readonly string HUB_NAME = "BJUTHelperNotify";
        public static readonly string HUB_CONNEXTION_STRING = "Endpoint=sb://bjuthelpernotify.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=T/js8sTjjnauQC/N+C4VjXEOaEh4fMedIjlsm9A5JYs=";
    }
}
