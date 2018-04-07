using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
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
using System.Xml.Serialization;
using System.Xml;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media.Animation;
using System.Runtime.Serialization;

namespace BJUTHelper
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        ObservableCollection<InfoEntity> infoList;
        
        public  MainPage()
        {
           
            NavigationCacheMode = NavigationCacheMode.Required;
            this.InitializeComponent();
            
            Task.Run(async () =>
            {
                infoList = await ReadWebInfo<InfoEntity>();
                if (infoList == null)
                {
                    infoList = new ObservableCollection<InfoEntity>();
                }

                while (true)
                {
                    await GetNetInfo();
                    await Task.Delay(60000);
                }
            });
        }
        #region 功能类按钮
        private void btnNicRegist_Click(object sender, RoutedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(Login), null);
        }
       
        private void btnNetAuthentication_Click(object sender, RoutedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(Authentication), null);
        }

        private void btnJWGL_Click(object sender, RoutedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(JWGL), null);


        }

        private void btnIDCard_Click(object sender, RoutedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(IDCardCenter), null);

        }

        private void btnMore_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(About.About));
        }
        #endregion


        WebInfoContent WebTipsContent;
        private void btnTips_Click(object sender, RoutedEventArgs e)
        {
            if (((SolidColorBrush)btnTips.Foreground).Color == Colors.LightYellow)
            {
                btnTips.Foreground = new SolidColorBrush(Colors.Black);
                if (WebTipsContent != null)
                {
                    WebTipsContent.Tapped -= (s1, e1) => btnTips_Click(null, null);
                    mainGrid.Children.Remove(WebTipsContent);
                }

            }
            else
            {
                btnTips.Foreground = new SolidColorBrush(Colors.LightYellow);

                if (WebTipsContent == null)
                    WebTipsContent = new WebInfoContent();//创建web提示窗口

                WebTipsContent.DataContext = infoList;//绑定get到的信息

                WebTipsContent.Tapped += (s1, e1) => btnTips_Click(null, null);//注册单击事件，单击可关闭

                mainGrid.Children.Add(WebTipsContent);

            }
        }

        public async Task GetNetInfo()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {

                    string uriStr = "http://www.cjw1115.com/Info?id=" + DateTime.Now;
                    using (HttpResponseMessage respnese = await client.GetAsync(new Uri(uriStr)))
                    {
                        await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                        {
                            if (infoList == null)
                            { infoList = new  ObservableCollection<InfoEntity>(); }
                            
                            //获取json信息
                            string json = await respnese.Content.ReadAsStringAsync();
                            var list=WebInfoContent.PraseJson(json);
                            //添加前清空数据
                            infoList.Clear();
                            foreach (var item in list)
                            {
                                infoList.Add(item);
                            }
                            WriteWebInfoToLocal(infoList);
                        });

                    }
                }
            }
            catch (Exception ex) { }
        }
        

        private async Task<ObservableCollection<T>> ReadWebInfo<T>() where T : class, new()
        {
            ObservableCollection<T> list = null;
            Windows.Storage.StorageFile file = null;
            try
            {
                file = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync("webinfo");
                if (file != null)
                {
                    using (Stream stream = await file.OpenStreamForReadAsync())
                    {
                        DataContractSerializer ser = new DataContractSerializer(typeof(ObservableCollection<T>));
                        list = ser.ReadObject(stream) as ObservableCollection<T>;
                    }
                }
            }
            catch { }

            
            return list;
        }
        private async void WriteWebInfoToLocal<T>(ObservableCollection<T> list) where T : class, new()
        {
            Windows.Storage.StorageFile file = null;
            try
            {
                file = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync("webinfo", Windows.Storage.CreationCollisionOption.ReplaceExisting);
                if (file != null)
                {
                    using (Stream stream = await file.OpenStreamForWriteAsync())
                    {
                        DataContractSerializer ser = new DataContractSerializer(typeof(ObservableCollection<T>));
                        ser.WriteObject(stream, list);
                    }
                }
            }
            catch { }
        }
    }
    
    
}
