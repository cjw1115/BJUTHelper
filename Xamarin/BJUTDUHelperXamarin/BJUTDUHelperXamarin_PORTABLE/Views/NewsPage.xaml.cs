using Xamarin.Forms;

namespace BJUTDUHelperXamarin.Views
{
    public partial class NewsPage : ContentPage
    {
        public NewsPage()
        {
            InitializeComponent();
            webView.Navigating += WebView_Navigating;
        }

        private void WebView_Navigating(object sender, WebNavigatingEventArgs e)
        {

            if (!e.Url.StartsWith("file")) 
            {
                if (!e.Url.Contains("cjw1115.com/news"))
                {
                    Device.OpenUri(new System.Uri(e.Url));
                    e.Cancel = true;
                }
            }
            
        }

        public static readonly BindableProperty HtmlSourceProperty = BindableProperty.CreateAttached("HtmlSource", typeof(string), typeof(WebView), null,propertyChanged:(o,oldValue,newValue)=> 
        {
            if (newValue != null)
            {
                WebView wv = o as WebView;
                HtmlWebViewSource htmlSource = new HtmlWebViewSource();
                htmlSource.Html = newValue as string;
                wv.Source = htmlSource;
            }
            

        });
        public static string GetHtmlSource(BindableObject bindbleObject)
        {
            return (string)bindbleObject.GetValue(HtmlSourceProperty);
        }

        public static void SetHtmlSource(BindableObject bindableObject,string value)
        {
            bindableObject.SetValue(HtmlSourceProperty, value);
        }
    }
}
