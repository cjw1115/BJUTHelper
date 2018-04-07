using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
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
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using BJUTHelper.Model;
using Windows.Phone.UI.Input;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace BJUTHelper
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    /// 
    public sealed partial class Authentication : Page
    {
        

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            wifi.ScanWifiList();
        }

        //针对手机后退键
        //private void  HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        //{
        //    e.Handled = true;

        //    if (Frame.CanGoBack)
        //        Frame.GoBack();
        //}
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (Frame.CanGoBack)
                Frame.GoBack();
        }
        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            OnNavigatedFrom(null);
        }
        User<UserAuthEntity> user;

        Wifi wifi;

        public Authentication()
        {
            //if ("Windows.Mobile" == Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily)
            //{
            //    Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            //}
            
            //设置当前页面可被缓存
            NavigationCacheMode = NavigationCacheMode.Enabled;
            this.InitializeComponent();

            wifi = new Wifi();

            user = new User<UserAuthEntity>();
            if (user.UserInfo != null)
            {
                grid.DataContext = user.UserInfo;
            }
            //绑定wifi列表
            cbNetList.ItemsSource = wifi.WifiList;
            //绑定用户名密码
            lbInfo.DataContext = wifi;

        }
       
        
        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (cbNetList.SelectedIndex==-1)
            {
                await new MessageDialog("请选择一个WIFI信号").ShowAsync();
                return;
            }
            var selWifi = cbNetList.SelectedItem as Windows.Devices.WiFi.WiFiAvailableNetwork;
            if (wifi.BjutWifiList.Contains(selWifi.Ssid) == false)
            {
                await new MessageDialog("请选择一个工大官方wifi信号").ShowAsync();
                return;
            }

            if (string.IsNullOrEmpty(tbUserName.Text))
            {
                tbUserName.Focus(FocusState.Keyboard);
                return;
            }
            if (string.IsNullOrEmpty(tbPassword.Password))
            {
                tbPassword.Focus(FocusState.Keyboard);
                return;
            }

            if (user.UserInfo == null)
                user.UserInfo = new UserAuthEntity();
            user.UserInfo.UserName = tbUserName.Text.Trim();
            user.UserInfo.Password = tbPassword.Password.Trim();

            //显示进度环
            ProgressView.ProgressIndicator progress = new ProgressView.ProgressIndicator();

            progress.Show();
         

            try
            {
                //if (cbSave.IsChecked == true)
                //{
                //    user.SaveUserInfo();
                //}
                user.SaveUserInfo();//保存密码

                var connProfile = Windows.Networking.Connectivity.NetworkInformation.GetInternetConnectionProfile();

                bool re = true;
                if (connProfile == null || connProfile.ProfileName != selWifi.Ssid)
                {
                    re = await Task.Run<bool>(() => wifi.ConnectWifi(selWifi));
                }

                if (re == true)
                {
                    wifi.ConStatus = "已连接到：" + selWifi.Ssid;

                    await Authenticate();
                }
                else
                {
                    wifi.ConStatus = "WIFI连接异常，请重试";
                }
               

            }
            catch (Exception ex)
            {
                await new MessageDialog(ex.Message).ShowAsync();
            }
            finally
            {
                //隐藏进度环
                //progressRing.IsActive = false;
                //SetControlStatus(grid, true);
                progress.Hide();
                //btnAuthentication.Focus(FocusState.Keyboard);
            }

        }
        private void SetControlStatus(Panel panel,bool status)
        {
            foreach (var item in panel.Children)
            {
                if (item is Panel)
                {
                    SetControlStatus((Panel)item, status);
                }
                else
                {
                    if(item is Control)
                        ((Control)item).IsEnabled = status;
                }
            }
        }

        public async Task Authenticate()
        {
            var selWifi = cbNetList.SelectedItem as Windows.Devices.WiFi.WiFiAvailableNetwork;
            if (selWifi == null)
            {
                await new MessageDialog("选择的wifi型号参数不正确").ShowAsync();
                return;
            }
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            switch (selWifi.Ssid)
            {  
                case "bjut_wifi":
                    {
                        parameters.Clear();
                        parameters.Add("0MKKey", "123");
                        parameters.Add("DDDDD", user.UserInfo.UserName);
                        parameters.Add("upass", user.UserInfo.Password);

                        HttpClient httpClient = null;
                        HttpRequestMessage request = null;
                        HttpFormUrlEncodedContent postData = null;
                        HttpResponseMessage response = null;
                        try
                        {
                            httpClient = new HttpClient();
                            response = await httpClient.GetAsync(new Uri("http://wlgn.bjut.edu.cn/"));
                            string responseString = await response.Content.ReadAsStringAsync();
                            if(response.StatusCode== HttpStatusCode.Ok)
                            {
                                if (responseString.Contains("url4"))
                                {
                                    await new MessageDialog("已经认证成功").ShowAsync();
                                    Frame.Navigate(typeof(MainPage));
                                    Frame.Navigate(typeof(Login));
                                }
                                else
                                {
                                    string posturi = "http://wlgn.bjut.edu.cn/";
                                    request = new HttpRequestMessage(HttpMethod.Post, new Uri(posturi));
                                    postData = new HttpFormUrlEncodedContent(parameters);
                                    request.Content = postData;
                                    response = await httpClient.SendRequestAsync(request);
                                    responseString = await response.Content.ReadAsStringAsync();
                                    if (response.StatusCode == HttpStatusCode.Ok)
                                    {
                                        #region 针对其他认证方式的结果判断逻辑
                                        if (responseString.Contains("url4")|| responseString.Contains("注销"))
                                        {
                                            await new MessageDialog("认证成功").ShowAsync();
                                            Frame.Navigate(typeof(MainPage));
                                            Frame.Navigate(typeof(Login));
                                        }
                                        else
                                        {
                                            int statusCode;
                                            string flagString = "Msg=";
                                            string status = string.Empty;
                                            int index = responseString.IndexOf(flagString);
                                            for (int i = index + flagString.Length + 1; i < 10; i++)
                                            {
                                                var t = responseString[i];
                                                if (t != ';')
                                                    status += t;
                                                else
                                                    break;
                                            }
                                            statusCode = int.Parse(status);
                                            if (statusCode == 1)
                                            {
                                                await new MessageDialog("账号或密码不对，请重新输入").ShowAsync();
                                            }
                                            else if (statusCode == 2)
                                            {
                                                await new MessageDialog("该账号正在使用中，请您与网管联系").ShowAsync();
                                            }
                                            else if (statusCode == 3)
                                            {
                                                await new MessageDialog("用户名无效，可能已从本系统注销").ShowAsync();
                                            }
                                            else if (statusCode == 4)
                                            {
                                                await new MessageDialog("本账号费用超支或时长流量超过限制").ShowAsync();
                                            }
                                            else if (statusCode == 5)
                                            {
                                                await new MessageDialog("本账号暂停使用").ShowAsync();
                                            }
                                            else if (statusCode == 15)
                                            {
                                                await new MessageDialog("登录成功").ShowAsync();
                                            }

                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        await new MessageDialog(response.StatusCode.ToString()).ShowAsync();
                                    }
                                }
                            }
                            else
                            {
                                await new MessageDialog("网络异常").ShowAsync();
                            }
                            
                            
                        }
                        catch (Exception ex)
                        {
                            await new MessageDialog(ex.Message).ShowAsync();
                        }
                        finally
                        {
                            if (httpClient != null) httpClient.Dispose() ;
                            if (request != null) request.Dispose(); ;
                            if(postData != null)postData.Dispose();
                            if(response!= null)response.Dispose();
                        }
                    }
                    break;
                case "Tushuguan":
                    {
                        HttpStringContent httpStringContent = new HttpStringContent(
                            "PtUser=" + user.UserInfo.UserName + "&PtPwd=" + user.UserInfo.Password 
                            + "&PtButton=%B5%C7%C2%BC");
                        HttpBaseProtocolFilter filter = null;
                        HttpClient httpClient = null;
                        HttpRequestMessage request = null;
                        HttpFormUrlEncodedContent postData = null;
                        HttpResponseMessage response = null;
                        try
                        {
                            httpClient = new HttpClient();
                            
                            string posturi = "http://172.24.39.253/portal/logon.cgi";
                            request = new HttpRequestMessage(HttpMethod.Post, new Uri(posturi));

                            request.Content = httpStringContent;
                            response = await httpClient.SendRequestAsync(request);

                            string responseString = await response.Content.ReadAsStringAsync();
                            //获取cookie
                            filter = new HttpBaseProtocolFilter();

                            HttpCookieCollection cookieCollection = filter.CookieManager.GetCookies(new Uri(posturi));

                            //设置cookie
                            foreach (HttpCookie item in cookieCollection)
                            {
                                filter.CookieManager.SetCookie(item, false);
                            }
                            request.Dispose();
                            request = new HttpRequestMessage(HttpMethod.Get, new Uri("http://172.24.39.253/portal/logon.htm"));
                            response = await httpClient.SendRequestAsync(request);

                            if (response.StatusCode == HttpStatusCode.Ok)
                            {
                                if (responseString.Contains("value='Logoff'"))
                                {
                                    await new MessageDialog("认证成功").ShowAsync();
                                    Frame.Navigate(typeof(MainPage));
                                    Frame.Navigate(typeof(Login));
                                }
                                else if (responseString.Contains("pt_unload()"))
                                    await new MessageDialog("已登录 ").ShowAsync();
                                else
                                    await new MessageDialog("认证失败，请检查用户名或密码").ShowAsync();
                            }
                            else
                            {
                                await new MessageDialog(response.StatusCode.ToString()).ShowAsync();
                            }
                        }
                        
                        catch (Exception ex)
                        {
                            await new MessageDialog("wifi连接可能未就绪，请稍后重试").ShowAsync();
                           
                        }
                        finally
                        {
                            if (httpClient != null) httpClient.Dispose();
                            if (request != null) request.Dispose(); ;
                            if (postData != null) postData.Dispose();
                            if (response != null) response.Dispose();
                            if (filter != null) filter.Dispose();
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
