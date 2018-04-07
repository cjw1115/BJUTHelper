using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace BJUTDUHelper.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class WIFIHelperView : Page
    {
        private readonly string messageToken = "1";
        private readonly string bjutUri = "http://my.bjut.edu.cn";
        private readonly string lgnUri = "http://lgn.bjut.edu.cn/";
        Service.HttpBaseService _httpService;
        
        public ViewModel.WIFIHelperVM WIFIHelperVM { get; set; }
        public WIFIHelperView()
        {
            var locator = Application.Current.Resources["Locator"] as ViewModel.ViewModelLocator;
            WIFIHelperVM = locator.WIFIHelperVM;

            this.InitializeComponent();
            _httpService = new Service.HttpBaseService();
            
            this.Loaded += WIFIHelperView_Loaded;
        }

        private async void WIFIHelperView_Loaded(object sender, RoutedEventArgs e)
        {
            WIFIHelperVM.Loaded();
            try
            {

                //获取无线电设备
                var radios = await Service.WIFIService.GetWifiRadios();
                Windows.Devices.Radios.Radio defaultRadio = null;
                if (radios!=null&&radios.Count != 0)
                {
                    defaultRadio = radios[0];
                }

                //获取当前网络连接类型
                var types = Service.WIFIService.GetNetworkType();
                switch (types)
                {
                    case Service.NetworkType.None://没有任何网络连接，无线电设备可能被关闭了
                        if (defaultRadio == null)
                        {
                            GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("没有可以使用的网络", messageToken);
                        }
                        else
                        {
                            Service.WIFIService.SetWifiRadiosState(defaultRadio, true);//打开wifi设备
                            
                        }
                        this.Frame.Navigate(typeof(View.WIFIHelperAuthView), new WIFIAuthViewParam { WIFIHelperVM=WIFIHelperVM });

                        break;
                    case Service.NetworkType.Ethernet://以太网，直接导航到注册页面
                        var re = await GetAuthenStatus();
                        if (re)
                        {
                            this.Frame.Navigate(typeof(View.WIFIHelperRegView), new WIFIRegViewParam { WIFIHelperVM = WIFIHelperVM });
                        }
                        else
                        {
                            if (defaultRadio == null)
                            {
                                GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("连接不到校园网", messageToken);
                            }
                            else
                            {
                                Service.WIFIService.SetWifiRadiosState(defaultRadio, true);//打开wifi设备
                                
                            }
                            this.Frame.Navigate(typeof(View.WIFIHelperAuthView), new WIFIAuthViewParam { WIFIHelperVM = WIFIHelperVM });
                        }
                        break;
                    case Service.NetworkType.Wireless:
                        var status = await GetAuthenStatus();
                        if (status)
                        {
                            this.Frame.Navigate(typeof(View.WIFIHelperRegView), new WIFIRegViewParam { WIFIHelperVM = WIFIHelperVM });
                        }
                        else
                        {
                            this.Frame.Navigate(typeof(View.WIFIHelperAuthView), new WIFIAuthViewParam { WIFIHelperVM = WIFIHelperVM });
                        }

                        break;
                    case Service.NetworkType.Mobile:
                        if (defaultRadio == null)
                        {
                            GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("移动网络下不能使用WIFI助手", messageToken);
                        }
                        else
                        {
                            Service.WIFIService.SetWifiRadiosState(defaultRadio, true);//打开wifi设备
                             
                        }
                        this.Frame.Navigate(typeof(View.WIFIHelperAuthView), new WIFIAuthViewParam { WIFIHelperVM = WIFIHelperVM });
                        break;
                    case Service.NetworkType.Other:
                        GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("你的网络比较复杂！", messageToken);
                        this.Frame.Navigate(typeof(View.WIFIHelperAuthView), new WIFIAuthViewParam { WIFIHelperVM = WIFIHelperVM });
                        break;
                    default:
                        this.Frame.Navigate(typeof(View.WIFIHelperAuthView), new WIFIAuthViewParam { WIFIHelperVM = WIFIHelperVM });
                        break;
                }
            }
            catch(Exception ex)
            {
                await new MessageDialog(ex.Message).ShowAsync();
            }
        }

        public async Task<bool> GetAuthenStatus()
        {
            try
            {
                var code = await _httpService.GetResponseCode(lgnUri);
                if (code == System.Net.HttpStatusCode.OK)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }

        }

        
    }

    public class WIFIAuthViewParam
    {
        public bool IsAuth { get; set; }
        public ViewModel.WIFIHelperVM WIFIHelperVM { get; set; }
    }
    public class WIFIRegViewParam
    {
        public bool? IsInternet { get; set; }
        public ViewModel.WIFIHelperVM WIFIHelperVM { get; set; }
    }
}
