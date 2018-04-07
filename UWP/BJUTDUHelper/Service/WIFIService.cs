using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.WiFi;
using Windows.Networking.Connectivity;

namespace BJUTDUHelper.Service
{
    public class WIFIService : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName]string propertyName="")
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private string _connectionStatus;
        public string ConnectionStatus
        {
            get
            {
                return _connectionStatus;
            }
            set
            {
                _connectionStatus = value;
                OnPropertyChanged();
            }
        }


        public static IList<string> BJUTWifiList = new List<string>
        { "bjut_wifi","Tushuguan"};

        public ObservableCollection<WiFiAvailableNetwork> WifiList = new ObservableCollection<WiFiAvailableNetwork>();

        public WIFIService()
        {   
        }
        public async void ScanWifiList()
        {
            WifiList.Clear();
            var auth = await Windows.Devices.WiFi.WiFiAdapter.RequestAccessAsync();
            if (auth != Windows.Devices.WiFi.WiFiAccessStatus.Allowed)
            {
                throw new Exception("没有足够的权限");
            }
            var adpters = await Windows.Devices.WiFi.WiFiAdapter.FindAllAdaptersAsync();

            foreach (var adpter in adpters)
            {

                await adpter.ScanAsync();
                var networks = adpter.NetworkReport.AvailableNetworks;
                foreach (var item in BJUTWifiList)
                {
                    var sel=networks.Where(m => m.Ssid == item).OrderByDescending(m => m.SignalBars).FirstOrDefault();
                    if (sel != null)
                    {
                        WifiList.Add(sel);
                    }
                }
                
            }

        }
        public async Task<bool> ConnectWifi(WiFiAvailableNetwork wifi)
        {
            var adpters = await Windows.Devices.WiFi.WiFiAdapter.FindAllAdaptersAsync();

            if (adpters!=null&&adpters.Count > 0)
            {
                //默认选择第一个适配器
                var adpter = adpters[0];
                var profile = await adpter.NetworkAdapter.GetConnectedProfileAsync();
                if (profile != null&& wifi.Ssid == profile.ProfileName)
                {
                    return true;
                }
                if (wifi.SecuritySettings.NetworkEncryptionType == NetworkEncryptionType.None)
                {
                    var connectionResult = await adpter.ConnectAsync(wifi, WiFiReconnectionKind.Automatic);
                    if (connectionResult.ConnectionStatus == WiFiConnectionStatus.Success)
                    {
                        //await Task.Delay((6 - Convert.ToInt32(wifi.SignalBars)) * 1000);
                        return true;
                    }
                }
            }
            return false;
        }

        public async static Task<IList<Windows.Devices.Radios.Radio>> GetWifiRadios()
        {
            var list=await Windows.Devices.Enumeration.DeviceInformation.FindAllAsync(Windows.Devices.Radios.Radio.GetDeviceSelector());

               var wifiradiosList = new List<Windows.Devices.Radios.Radio>();
            
            var access = await Windows.Devices.Radios.Radio.RequestAccessAsync();
            
            if (access != Windows.Devices.Radios.RadioAccessStatus.Allowed)
            {
                return null;
            }
            var radios = await Windows.Devices.Radios.Radio.GetRadiosAsync();
            foreach (var item in radios)
            {
                if (item.Kind == Windows.Devices.Radios.RadioKind.WiFi)
                    wifiradiosList.Add(item);
            }
            return wifiradiosList;
        }
        public static bool GetWifiRadiosState(Windows.Devices.Radios.Radio radio)
        {

            if (radio.Kind == Windows.Devices.Radios.RadioKind.WiFi)
            {
                if (radio.State == Windows.Devices.Radios.RadioState.On)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
        public async static void SetWifiRadiosState(Windows.Devices.Radios.Radio radio, bool isopen)
        {
            if (radio.Kind == Windows.Devices.Radios.RadioKind.WiFi)
            {
                if (radio.State != Windows.Devices.Radios.RadioState.On)
                {
                    if (isopen)
                    {
                        await radio.SetStateAsync(Windows.Devices.Radios.RadioState.On);
                    }
                }
                else
                {
                    if (!isopen)
                    {
                        await radio.SetStateAsync(Windows.Devices.Radios.RadioState.Off);
                    }
                }
                return;
            }
        }

        
        public static NetworkType GetNetworkType()
        {
            var profile = Windows.Networking.Connectivity.NetworkInformation.GetInternetConnectionProfile();
            if (profile == null)
            {
                return NetworkType.None;
            }
            var interfaceType = profile.NetworkAdapter.IanaInterfaceType;

            if (interfaceType == 71)
            {
                return NetworkType.Wireless;
            }
            else if (interfaceType == 6)
            {
                return NetworkType.Ethernet;
            }
            else if (interfaceType == 243 || interfaceType == 244)
            {
                return NetworkType.Mobile;
            }
            else
            {
                return NetworkType.Other;
            }

        }
        public static string GetCurrentWifiSsid()
        {
            var profile = Windows.Networking.Connectivity.NetworkInformation.GetInternetConnectionProfile();
            if (profile != null&& profile.NetworkAdapter.IanaInterfaceType == 71)
            {
                return profile.ProfileName;
            }
            return null;
        }

    }
    public enum NetworkType
    {
        None,Ethernet,Wireless,Mobile,Other
    }
}
