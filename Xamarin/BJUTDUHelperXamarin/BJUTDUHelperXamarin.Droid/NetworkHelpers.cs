using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Net;
using BJUTDUHelperXamarin.Services;
using Xamarin.Forms;
using BJUTDUHelperXamarin.Droid;
using Android.Net.Wifi;

[assembly: Dependency(typeof(NetworkHelpers))]
namespace BJUTDUHelperXamarin.Droid
{
    public class NetworkHelpers: IIPInfo
    {
        public string GetIPAddress()
        {
            IPAddress[] adresses = Dns.GetHostAddresses(Dns.GetHostName());


            if (adresses != null && adresses[0] != null)
            {
                return adresses[0].ToString();
            }
            else
            {
                return null;
            }
        }
        public string GetCurrentSSID()
        {
            
            WifiManager wifiManager = (WifiManager)Xamarin.Forms.Forms.Context.GetSystemService(Context.WifiService);
            
            WifiInfo wifiInfo = wifiManager.ConnectionInfo;
            if (wifiInfo != null)
            {
                return wifiInfo.SSID;
            }
            else
            {
                return null;
            }
        }
    }
}