using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.WiFi;
using Windows.Networking.Connectivity;
using Windows.UI.Xaml.Controls;

namespace BJUTHelper.Model
{

    public class Wifi:INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }
        private string _conStatus;
       
        public string ConStatus
        {
            get
            {
                return _conStatus;
            }
            set
            {
                _conStatus = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ConStatus"));
            }
        }


        public List<string> BjutWifiList = new List<string>
        { "bjut_wifi","Tushuguan"};
              
        public ObservableCollection<WiFiAvailableNetwork> WifiList = new ObservableCollection<WiFiAvailableNetwork>();
        
        public Wifi()
        {
            ScanWifiList();
            //Timer timer = new Timer(new TimerCallback(ScanWifiList), null, 0, 10000);
        }
        public async void ScanWifiList()
        {
            WifiList.Clear();
            var auth = await Windows.Devices.WiFi.WiFiAdapter.RequestAccessAsync();
            if (auth != Windows.Devices.WiFi.WiFiAccessStatus.Allowed)
            {
                //await new MessageDialog("设备的锅！").ShowAsync();
                return;
            }
            var adpters = await Windows.Devices.WiFi.WiFiAdapter.FindAllAdaptersAsync();
            
            foreach (var item in adpters)
            {
                
                await item.ScanAsync();

                //一段比较恶心的逻辑，用来过滤重复的wifi信号
                foreach(var network in item.NetworkReport.AvailableNetworks)
                {
                    //if (BjutWifiList.Contains(network.Ssid))
                    //{
                        int i;
                        for(i=0;i<WifiList.Count;i++)
                        {
                            if (WifiList[i].Ssid == network.Ssid)
                            {
                                if (WifiList[i].SignalBars > network.SignalBars)
                                    break;
                                else
                                {
                                    WifiList[i] = network;
                                    break;
                                }
                            } 
                        }
                        if(i>=WifiList.Count)
                            WifiList.Add(network);
                    //}   
                }                
            }

        }

       
        public async Task<bool> ConnectWifi(WiFiAvailableNetwork wifi)
        {
            
            if(wifi==null)
            {
                throw new Exception("未指定明确的ssid");
            }

            var adpters = await Windows.Devices.WiFi.WiFiAdapter.FindAllAdaptersAsync();
            
            if (adpters.Count>0)
            {
                
                //默认选择第一个适配器
                var adpter = adpters[0];
                if (wifi.SecuritySettings.NetworkEncryptionType == NetworkEncryptionType.None)
                {
                    var connectionResult = await adpter.ConnectAsync(wifi, WiFiReconnectionKind.Manual);
                    if (connectionResult.ConnectionStatus == WiFiConnectionStatus.Success)
                    {
                        await Task.Delay((6-Convert.ToInt32(wifi.SignalBars))*1000);
                        return true;
                    }
                }
            }
            else
            {
                throw new Exception("没有可用的无线适配器(网卡)，请安装网卡或安装驱动程序！");
            }
            return false;
        }


    }
}
