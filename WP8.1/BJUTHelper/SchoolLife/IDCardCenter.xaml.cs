using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Cryptography.Certificates;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using BJUTHelper.Model;
using Windows.Phone.UI.Input;
using ProgressView;
// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace BJUTHelper
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class IDCardCenter : Page
    {
        IDCardInfoPrase info = new IDCardInfoPrase();
        SchoolLife.SchoolLifeCommon.LoginReault loginResult;
        public static bool isLogin { get; set; } = false;
        User<UserIDCardEntity> user;
        public IDCardCenter()
        {
            //if ("Windows.Mobile" == Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily)
            //    Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;

            NavigationCacheMode = NavigationCacheMode.Enabled;
            this.InitializeComponent();
            user = new  User<UserIDCardEntity>();
            if (user.UserInfo != null)
            {
                gridLogin.DataContext = user.UserInfo;
                
            }
            btnLogoff.IsEnabled = false;
            
        }
        //private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        //{
        //    e.Handled = true;

        //    if (Frame.CanGoBack)
        //        Frame.GoBack();
        //}
        

        HttpClient client = null;
        HttpBaseProtocolFilter filter = null;
        
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
                user.UserInfo = new UserIDCardEntity();
            user.UserInfo.UserName = tbUserName.Text.Trim();
            user.UserInfo.Password = tbPassword.Password.Trim();
            

            ProgressView.ProgressIndicator progress = new ProgressView.ProgressIndicator();
            progress.Show();
            try
            {
                tbTips.Text = "登录中，请稍后···";

                loginResult = await SchoolLife.SchoolLifeCommon.LoginSchoolLifeCenter(user.UserInfo.UserName, user.UserInfo.Password);

                if(loginResult.Client == null)
                {
                    await new MessageDialog("登录异常").ShowAsync();
                    progress.Hide();
                    tbTips.Text = "";
                    return;
                }
                if(loginResult.LoginResultEnum== SchoolLife.SchoolLifeCommon.LoginReault.LoginResultEnums.LoginFailure)
                {
                    tbTips.Text = "检查账户密码";
                    
                    tbPassword.Password = string.Empty;
                    progress.Hide();
                    tbTips.Text = "";
                    return;
                }
                
                client = await SchoolLife.SchoolLifeCommon.GetCardCenterClient(loginResult.Client);
                if (client == null)
                {
                    await new MessageDialog("登录一卡通自助服务中心异常").ShowAsync();
                    progress.Hide();
                    tbTips.Text = "";
                    return;
                }
                if (cbSave.IsChecked == true)
                {
                    user.SaveUserInfo();
                }
                btnLogoff.IsEnabled = true;
                btnLogin.IsEnabled = false;
                //登录成功，隐藏登录界面.
                isLogin = true;
                gridLogin.Visibility = Visibility.Collapsed;
                


                gridIDCardInfo.Visibility = Visibility;//显示一卡通页面
                info.GetTransactionInfo(client);
                info.GetIDCardInfo(client);

                //绑定数据
                listView.ItemsSource = info.TransactionList;
                gridIDCardInfo.DataContext = info.idCardInfo;

                splitView.Visibility = Visibility.Visible;
                gridLogin.Visibility = Visibility.Collapsed;


            }
            catch (Exception ex)
            {
                tbTips.Text = "登录失败！检查你的网络连接！";
            }
            finally
            {
                progress.Hide();
            }

        }
        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.Frame.CanGoBack == true)
                Frame.GoBack();
        }

        private void btnMenu_Click(object sender, RoutedEventArgs e)
        {
            splitView.IsPaneOpen = !splitView.IsPaneOpen;
        }

        private void Panel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ListViewItem selitem = listView.ContainerFromItem((sender as Panel).DataContext) as ListViewItem;
            if (selitem.ContentTemplate == DataTemplateNoSelected)
            {
                selitem.ContentTemplate = DataTemplateSelected;
            }
            else
            {
                selitem.ContentTemplate = DataTemplateNoSelected;
            }
        }

        private void btnIDCardInfo_Click(object sender, RoutedEventArgs e)
        {
            if (isLogin == true)
            {
                gridIDCardInfo.Visibility = Visibility.Visible;
                listView.Visibility = Visibility.Collapsed;
            }
            
        }

        private void btnIDCardUsedInfo_Click(object sender, RoutedEventArgs e)
        {
            if (isLogin == true)
            {
                gridIDCardInfo.Visibility = Visibility.Collapsed;
                listView.Visibility = Visibility.Visible;
            }
            
        }

        private void btnLogoff_Click(object sender, RoutedEventArgs e)
        {
            isLogin = false;
            gridLogin.Visibility = Visibility.Visible;
            listView.Visibility = Visibility.Collapsed;
            gridIDCardInfo.Visibility = Visibility.Collapsed;


            info = new IDCardInfoPrase();
            //清空登录页面
            tbTips.Text = "";

            btnLogoff.IsEnabled = false ;
            btnLogin.IsEnabled = true;
        }

        private async void btnControlStatus_Click(object sender, RoutedEventArgs e)
        {
            if (client == null)
            {
                await new MessageDialog("登录可能已失效，请重新登录后尝试！").ShowAsync();
                return;
            }
            
            HttpRequestMessage request = null;
            HttpResponseMessage response = null;
            Dictionary<string, string> parameters = null;
            ProgressIndicator progerss = new ProgressIndicator();
            progerss.Text = "操作较耗时，请耐心等待！";
            progerss.Show();
            switch (btnControlStatus.Content as string)
            {
                case "挂失":
                    
                    request = new HttpRequestMessage(HttpMethod.Post, new Uri("https://cwss.bjut.edu.cn:9090/smart_web/ajax/card/modifyCardStatus.json?str_smtEndcode=2"));
                    parameters = new Dictionary<string, string>();
                    parameters.Add("lg_smtCardid",info.idCardInfo.smtCardid);
                    request.Content = new HttpFormUrlEncodedContent(parameters);
                    request.Headers["ContentType"] = "application/x-www-form-urlencoded";
                    response =await client.SendRequestAsync(request);
                    await info.GetIDCardInfo(client);
                    if (info.idCardInfo.smtEndcodeTxt == "挂失未补")
                    {
                        await new MessageDialog("挂失成功！请尽快补办！(补办地点：信西楼一楼)。\n若已找到，解挂即可！").ShowAsync();
                    }
                    request.Dispose();
                    response.Dispose();
                    break;
                case "解挂":
                    request = new HttpRequestMessage(HttpMethod.Post, new Uri("https://cwss.bjut.edu.cn:9090/smart_web/ajax/card/modifyCardStatus.json?str_smtEndcode=0"));
                    parameters = new Dictionary<string, string>();
                    parameters.Add("lg_smtCardid", info.idCardInfo.smtCardid);
                    request.Content = new HttpFormUrlEncodedContent(parameters);
                    request.Headers["ContentType"] = "application/x-www-form-urlencoded";
                    response = await client.SendRequestAsync(request);
                    await info.GetIDCardInfo(client);
                    if (info.idCardInfo.smtEndcodeTxt == "在用")
                    {
                        await new MessageDialog("解挂成功，以后注意保管好一卡通！").ShowAsync();
                    }
                    break;
                default:
                    break;
            }
            progerss.Hide();
        }
    }

    //卡状态文本转换挂失按钮文本
    public class StatusConver : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string goal = string.Empty;
            var str = value as string;
            if (str == null)
                return "挂失";
            switch (str)
            {
                case "在用":
                    goal = "挂失";
                    break;
                case "挂失未补":
                    goal = "解挂";
                    break;
                default:
                    break;
            }
            return goal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
