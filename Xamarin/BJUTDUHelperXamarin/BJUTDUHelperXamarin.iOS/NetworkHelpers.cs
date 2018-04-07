using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using BJUTDUHelperXamarin.Services;
using Xamarin.Forms;
using BJUTDUHelperXamarin.iOS;
using SystemConfiguration;

[assembly: Dependency(typeof(NetworkHelpers))]
namespace BJUTDUHelperXamarin.iOS
{

    public class NetworkHelpers : IIPInfo
    {
        public string GetIPAddress()
        {
            String ipAddress = "";


            foreach (var netInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (netInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                  netInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    foreach (var addrInfo in netInterface.GetIPProperties().UnicastAddresses)
                    {
                        if (addrInfo.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            ipAddress = addrInfo.Address.ToString();
                            break;
                        }
                    }
                }
            }

            return ipAddress;
        }

        public string GetCurrentSSID()
        {
            string[] infos;
            SystemConfiguration.CaptiveNetwork.TryGetSupportedInterfaces(out infos);
            foreach (var item in infos)
            {
                Foundation.NSDictionary dic;
                SystemConfiguration.CaptiveNetwork.TryCopyCurrentNetworkInfo(item, out dic);
                if (dic.Count > 0) 
                {
                    return dic.Values[0].ToString();
                }
                
            }
            return null;
        }
    }
}