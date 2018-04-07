using Plugin.Connectivity.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace BJUTDUHelperXamarin.Services
{
    public interface IIPInfo
    {
        string GetIPAddress();
        string GetCurrentSSID();
    }
    public class NetworkHelper
    {
        public static string GetIPAddress()
        {
            var ipInfo=Xamarin.Forms.DependencyService.Get<IIPInfo>();
            return ipInfo.GetIPAddress();
        }
        public static string GetCurrentSSID()
        {
            var ipInfo = Xamarin.Forms.DependencyService.Get<IIPInfo>();
            return ipInfo.GetCurrentSSID();

        }


        
        static NetworkHelper()
        {
            Plugin.Connectivity.CrossConnectivity.Current.ConnectivityTypeChanged += Current_ConnectivityTypeChanged;
            Plugin.Connectivity.CrossConnectivity.Current.ConnectivityChanged += Current_ConnectivityChanged;
        }

        

        public static event EventHandler ConnectionTypeChanged;
        public static event EventHandler ConnectionStatusChanged;
        private static void Current_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            ConnectionStatusChanged?.Invoke(sender, null);
        }
        private static void Current_ConnectivityTypeChanged(object sender, ConnectivityTypeChangedEventArgs e)
        {
            ConnectionTypeChanged?.Invoke(sender, null);
        }

        public static ConnectionType GetConnectType()
        {
            var types = Plugin.Connectivity.CrossConnectivity.Current.ConnectionTypes;
            if (types.Contains(Plugin.Connectivity.Abstractions.ConnectionType.WiFi))
            {
                return ConnectionType.WLAN;
            }
            if (types.Contains(Plugin.Connectivity.Abstractions.ConnectionType.Cellular))
            {
                return ConnectionType.Cellular;
            }
            return ConnectionType.None;
        }
        public static bool IsConected
        {
            get
            {
                return Plugin.Connectivity.CrossConnectivity.Current.IsConnected;
            }
        }
    }
    public enum ConnectionType
    {
        Cellular,
        WLAN,
        None
    }
    
}
