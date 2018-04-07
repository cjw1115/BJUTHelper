using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace BJUTDUHelperXamarin.Views
{
    public partial class MainPage : ContentPage
    {
        private readonly string appview = "https://cjw1115.com/bjutduhelper/appview/";
        public MainPage()
        {
            InitializeComponent();
            this.Appearing += MainPage_Appearing;
            webView.Navigating += WebView_Navigating; ;
            webView.Navigated += WebView_Navigated;
            HelloView();
            
        }

        private void MainPage_Appearing(object sender, EventArgs e)
        {
            webView.Source = new UrlWebViewSource() { Url = appview };
        }

        private void WebView_Navigated(object sender, WebNavigatedEventArgs e)
        {
            if (!e.Url.Contains("cjw1115.com"))
            {
                HelloView();
            }
            else
            {
                if(e.Result!= WebNavigationResult.Success)
                {
                    var htmlSource = new HtmlWebViewSource();
                    string htmStr = "<html><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" /></head><body><div><br /><div style=\"margin-left:auto;margin-right:auto;text-align:center\"><label style=\"color:cornflowerblue\">网络走丢了O(∩_∩)O</label></div><hr /><br /><div style=\"margin-left:auto;margin-right:auto;text-align:center\"><label style=\"color:cornflowerblue\">&copy; 2017 - 工大助手</label></div></div></body></html>";
                    htmlSource.Html = htmStr;
                    webView.Source = htmlSource;
                }
                
            }
        }

        private void WebView_Navigating(object sender, WebNavigatingEventArgs e)
        {
            
            if (e.Url == appview)
            {
                return;
            }
            if (e.Url.Contains("cjw1115.com"))
            {
                Device.OpenUri(new Uri(e.Url));
                e.Cancel = true;
            }
            else
            {
                e.Cancel = true;
            }
        }

        public void HelloView()
        {
            var htmlSource = new HtmlWebViewSource();
            string htmStr = "<html><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" /></head><body><div><br /><div style=\"margin-left:auto;margin-right:auto;text-align:center\"><label style=\"color:cornflowerblue\">点击左上汉堡菜单，开启精彩工大生活O(∩_∩)O</label></div><hr /><br /><div style=\"margin-left:auto;margin-right:auto;text-align:center\"><label style=\"color:cornflowerblue\">&copy; 2017 - 工大助手</label></div></div></body></html>";
            htmlSource.Html = htmStr;
            webView.Source = htmlSource;
        }
    }
}
