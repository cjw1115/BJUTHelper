
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
        public static readonly string HUB_NAME = "use yourself hub name from Azure Notification Center";
        public static readonly string HUB_CONNEXTION_STRING = "use yourself connection string from Azure Notification Center";
    }
}
