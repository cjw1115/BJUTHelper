using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
using BJUTHelper.Model;
using Windows.Phone.UI.Input;
using BJUTHelper.NetworkManager;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace BJUTHelper
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    /// 
    public sealed partial class Login : Page
    {
       
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
           
            if (Frame.CanGoBack)
                Frame.GoBack();
        }
        NetAccountInfo netAccountInfo;
        User<UserLoginEntity> user;
        public Login()
        {
            //if ("Windows.Mobile" == Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily)
            //    Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            //设置当前页面可被缓存
            NavigationCacheMode = NavigationCacheMode.Enabled;
            this.InitializeComponent();


            user = new User<UserLoginEntity>();
            if (user.UserInfo != null)
            {
                grid.DataContext = user.UserInfo;
            }
            
            netAccountInfo = new NetAccountInfo(this.Dispatcher);
            
            gridMain.DataContext = netAccountInfo;
            panelInfo.DataContext = netAccountInfo;
        }

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
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
                user.UserInfo = new UserLoginEntity();
            user.UserInfo.UserName = tbUserName.Text.Trim();
            user.UserInfo.Password = tbPassword.Password.Trim();
            if (radiobtnIpv4.IsChecked == true)
            {
                user.UserInfo.IPMode = IPModeEnum.IPV4;
            }
            else
            {
                user.UserInfo.IPMode = IPModeEnum.IPV6V4;
            }

            //显示等待环
            //progressRing.IsActive = true;
            //SetControlStatus(grid, false);
            ProgressView.ProgressIndicator progress = new ProgressView.ProgressIndicator();
            
            progress.Show();
            ConnectMode connectMode;
            if (user.UserInfo.IPMode== IPModeEnum.IPV4)
            {
                connectMode = ConnectMode.Ipv4;
            }
            else
            {
                connectMode = ConnectMode.IPv4v6;
            }

            var re=await BjutInternet.Login(user.UserInfo.UserName, user.UserInfo.Password, connectMode);
            await new MessageDialog(re.Msg).ShowAsync();
            if(re.CodeResult== CodeResult.Success)
            {
                user.SaveUserInfo();
                netAccountInfo.GetAccountBasicInfo(user.UserInfo.UserName, user.UserInfo.Password);
            }
            progress.Hide();

        }
     
       
        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            OnNavigatedFrom(null);
        }

        private async void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            ProgressView.ProgressIndicator progress = new ProgressView.ProgressIndicator();

            progress.Show();
            var re=await BjutInternet.Logout();
            await new MessageDialog(re.Msg).ShowAsync();
            progress.Hide();
            if (re.CodeResult== CodeResult.Fail)
            {
                //添加其他逻辑，例如重新注销或检测网络状况   
            }
            else
            {
                //添加其他逻辑
            }
        }

        private void SetControlStatus(Panel panel, bool status)
        {
            foreach (var item in panel.Children)
            {
                if (item is Panel)
                {
                    SetControlStatus((Panel)item, status);
                }
                else
                {
                    if (item is Control)
                        ((Control)item).IsEnabled = status;
                }
            }
        }
    }
}
