using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
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
using SystemNet = System.Net;
using BJUTHelper.Model;
using Windows.Security.Cryptography.Certificates;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Phone.UI.Input;
// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace BJUTHelper
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class JWGL : Page
    {
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.SourcePageType.ToString() == "Grade")
            { }
            else
            {
                imgCheckcode_PointerPressed(null, null);
            }
        }
        //private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
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

        User<UserJWGLEntity> user;
        GDJWGL gd;
        //bool IsWorking { get; set; }

        public JWGL()
        {
            //if ("Windows.Mobile" == Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily)
            //    Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
            this.InitializeComponent();
            user = new User<UserJWGLEntity>();
            if (user.UserInfo != null)
            {
                gridLogin.DataContext = user.UserInfo;
            }
            btnLogoff.IsEnabled = false;
            
        }

        //查看成绩页面
        private async void btnGrade_Click(object sender, RoutedEventArgs e)
        {
           
            ProgressView.ProgressIndicator progress = new ProgressView.ProgressIndicator();
            progress.Text = "请等待";
            progress.Show();

            if (gd == null)
            {
                await new MessageDialog("登录有误，请重新登录！").ShowAsync();
                //ShowWaitingRing(false);
                progress.Hide();
                return;
            }
            HttpRequestMessage request;
            HttpResponseMessage response;
            //    byte[] bytedata;
            try
            {
                HttpBaseProtocolFilter filter = gd.loginInfo.ob as HttpBaseProtocolFilter;
                if (filter == null)
                {
                    //ShowWaitingRing(false);
                    //IsWorking = false;
                    progress.Hide();
                    return;
                }

                HttpClient client = new HttpClient(filter);
                request = new HttpRequestMessage(HttpMethod.Get,
                    new Uri("http://gdjwgl.bjut.edu.cn/xscjcx.aspx?xh=" + gd.StudentID + "&xm=" + gd.StudentName + "&gnmkdm=N121605"));

                request.Headers["Referer"] = "http://gdjwgl.bjut.edu.cn/xs_main.aspx?xh=" + gd.StudentID;

                response = await client.SendRequestAsync(request, HttpCompletionOption.ResponseHeadersRead);


                string str = await response.Content.ReadAsStringAsync();
                //request.Dispose();
                //response.Dispose();

                string __VIEWSTATEString;
                __VIEWSTATEString = gd.GetVIEWSTATE(str);

                IDictionary<string, string> parameters = new Dictionary<string, string>();
                parameters.Add("__EVENTTARGET", "");
                parameters.Add("__EVENTARGUMENT", "");
                parameters.Add("__VIEWSTATE", SystemNet.WebUtility.UrlEncode(__VIEWSTATEString));
                parameters.Add("hidLanguage", "");
                parameters.Add("ddlXN", "");
                parameters.Add("ddlXQ", "");
                parameters.Add("ddl_kcxz", "");
                parameters.Add("btn_zcj", SystemNet.WebUtility.UrlEncode("历年成绩"));
                string buffer = "";

                int i = 0;
                foreach (string key in parameters.Keys)
                {
                    if (i > 0)
                    {
                        buffer += "&" + key + "=" + parameters[key];
                    }
                    else
                    {
                        buffer = key + "=" + parameters[key];
                    }
                    i++;
                }


                byte[] bytedata = Encoding.UTF8.GetBytes(buffer);

                SystemNet.HttpWebRequest netrequest = (SystemNet.HttpWebRequest)SystemNet.WebRequest.Create(new Uri("http://gdjwgl.bjut.edu.cn/xscjcx.aspx?xh=" + gd.StudentID + "&xm=" + "" + "&gnmkdm=N121605"));
                netrequest.Headers["Referer"] = "http://gdjwgl.bjut.edu.cn/xscjcx.aspx?xh=" + gd.StudentID + "&xm=" + "" + "&gnmkdm=N121605";
                netrequest.Method = "POST";

                netrequest.ContentType = "application/x-www-form-urlencoded";


                HttpCookie cookie = filter.CookieManager.GetCookies(new Uri("http://gdjwgl.bjut.edu.cn/xscjcx.aspx?xh=" + gd.StudentID + "&xm=" + gd.StudentName + "&gnmkdm=N121605"))[0];


                netrequest.Headers["Cookie"] = cookie.Name + "=" + cookie.Value;

                using (Stream streamRequest = await netrequest.GetRequestStreamAsync())//从在异步调用问题，
                {
                    streamRequest.Write(bytedata, 0, bytedata.Length);
                }

                SystemNet.HttpWebResponse netrespones = (SystemNet.HttpWebResponse)(await netrequest.GetResponseAsync());

                Stream stream = netrespones.GetResponseStream();
                using (StreamReader sr = new StreamReader(stream, DBCSCodePage.DBCSEncoding.GetDBCSEncoding("gb2312")))
                {

                    string s = sr.ReadToEnd();
                    gd.gc.GetGradeChart(s);
                }

                Frame.Navigate(typeof(Grade), gd);
                //IsWorking = false;
            }
            catch (Exception ex)
            {
                await new MessageDialog(ex.Message).ShowAsync();
                //IsWorking = false;
            }
            finally
            {
                //ShowWaitingRing(false);
                progress.Hide();
            }
        }


        //等待环页面
        //public void ShowWaitingRing(bool status)
        //{
        //    progressRing.IsActive = status;

        //}

        HttpBaseProtocolFilter filter;

        HttpClient client;
        private async void imgCheckcode_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if(filter==null)
                filter = new HttpBaseProtocolFilter();

            filter.CacheControl.WriteBehavior = HttpCacheWriteBehavior.NoCache;
            filter.IgnorableServerCertificateErrors.Add(ChainValidationResult.Untrusted);
            filter.IgnorableServerCertificateErrors.Add(ChainValidationResult.InvalidName);
            filter.IgnorableServerCertificateErrors.Add(ChainValidationResult.RevocationFailure);
            
            if(client==null)
                client = new HttpClient(filter);
            try
            {
                HttpResponseMessage response = await client.GetAsync(new Uri("http://gdjwgl.bjut.edu.cn/CheckCode.aspx"));
                var buffer = await response.Content.ReadAsBufferAsync();
                byte[] byteBuffer = new byte[buffer.Length];
                buffer.CopyTo(byteBuffer);
                using (MemoryStream stream = new MemoryStream(byteBuffer))
                {
                    BitmapImage img = new BitmapImage();
                    await img.SetSourceAsync(stream.AsRandomAccessStream());
                    imgCheckcode.Source = img;
                }
            }
            catch (Exception ex)
            {
                await new MessageDialog(ex.Message).ShowAsync();
            }
        }
        string __VIEWSTATEString = "";
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
            if (string.IsNullOrEmpty(tbCheckcode.Text.Trim()))
            {
                tbCheckcode.Focus(FocusState.Keyboard);
                return;
            }

            if (user.UserInfo == null)
                user.UserInfo = new UserJWGLEntity();
            user.UserInfo.UserName = tbUserName.Text.Trim();
            user.UserInfo.Password = tbPassword.Password.Trim();
            string checkCode = tbCheckcode.Text.Trim();

            try
            {
                tbTips.Text = "登录中，请稍后···";

                //简单单例模式，献丑了
                gd = GDJWGL.GDJWGLInit();
                if(client==null)
                {
                    
                    await new MessageDialog("重新输入验证码").ShowAsync();
                    imgCheckcode_PointerPressed(null, null);
                    tbCheckcode.Text = "";
                    return;
                }
                HttpResponseMessage response = await client.GetAsync(new Uri("http://gdjwgl.bjut.edu.cn/default2.aspx"));
                //HttpCookieCollection cookieCollection = filter.CookieManager.GetCookies(new Uri("http://gdjwgl.bjut.edu.cn/default2.aspx"));


                string responseHtml = await response.Content.ReadAsStringAsync();
                __VIEWSTATEString = gd.GetVIEWSTATE(responseHtml);
                

                if (__VIEWSTATEString == "")
                {
                    tbTips.Text = "登录失败！";
                    tbPassword.Password = "";
                    imgCheckcode_PointerPressed(null, null);
                    tbCheckcode.Text = "";
                    return;
                }
                

                IDictionary<string, string> parameters = new Dictionary<string, string>();
                parameters.Add("__VIEWSTATE", __VIEWSTATEString);
             
                parameters.Add("txtUserName", user.UserInfo.UserName);
                parameters.Add("TextBox2", user.UserInfo.Password);
                parameters.Add("txtSecretCode", checkCode);
                parameters.Add("RadioButtonList1", "学生");
                parameters.Add("Button1", "");
                parameters.Add("lbLanguage", "");
                parameters.Add("hidPdrs", "");
                parameters.Add("hidsc", "");

                HttpFormUrlEncodedContent postData = new HttpFormUrlEncodedContent(parameters);

                //HttpStringContent content = new HttpStringContent(postData);
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, new Uri("http://gdjwgl.bjut.edu.cn/default2.aspx"));

                //request.Headers["Referer"] = "http://gdjwgl.bjut.edu.cn/default2.aspx";
                request.Headers["ContentType"] = "application/x-www-form-urlencoded";

                request.Content = postData;

                HttpResponseMessage respones = await client.SendRequestAsync(request);
                string stringRespones = await respones.Content.ReadAsStringAsync();
                //Windows.Storage.Streams.IInputStream streamRespones = await respones.Content.ReadAsInputStreamAsync()

                gd.StudentName = gd.GetName(stringRespones);
                gd.StudentID = user.UserInfo.UserName;
                gd.loginInfo.ergs = null;
                gd.loginInfo.ob = filter;

                if (gd.StudentName == "")
                {
                    tbTips.Text = "登录失败！检查用户名或密码！";
                    tbPassword.Password = "";
                    imgCheckcode_PointerPressed(null, null);
                    tbCheckcode.Text = "";
                    return;
                }
                await new MessageDialog(String.Format("欢迎你，{0}同学！", gd.StudentName)).ShowAsync();
                btnLogoff.IsEnabled = true;
                btnLogin.IsEnabled = false;
                if (cbSave.IsChecked == true)
                {
                    user.SaveUserInfo();
                }
                //登录成功，隐藏登录界面，显示教务管理主页
                gridContent.Visibility = Visibility.Visible;
                gridLogin.Visibility = Visibility.Collapsed;

                
            }
            catch (Exception ex)
            {
                tbTips.Text = "登录失败！检查你的网络连接！";
                imgCheckcode_PointerPressed(null, null);
                tbCheckcode.Text = "";
            }
            
        }

        private void btnLogoff_Click(object sender, RoutedEventArgs e)
        {
            gridLogin.Visibility = Visibility.Visible ;
            gridContent.Visibility = Visibility.Collapsed;

            tbTips.Text = "";
            tbCheckcode.Text = "";
            imgCheckcode_PointerPressed(null, null);

            btnLogoff.IsEnabled = false;
            btnLogin.IsEnabled = true;
        }
    }
}
