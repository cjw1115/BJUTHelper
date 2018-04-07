
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace BJUTHelper
{
   
    public sealed partial class WebInfoContent : UserControl
    {
        public WebInfoContent()
        {
            this.InitializeComponent();
            this.Width = Window.Current.Bounds.Width;
            this.Height = Window.Current.Bounds.Height / 2;
            this.HorizontalAlignment = HorizontalAlignment.Stretch;
            this.VerticalAlignment = VerticalAlignment.Top;

            TranslateTransform trans = new TranslateTransform();
            this.RenderTransform = trans;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (storyBoard != null) 
                storyBoard.Begin();
        }

        public  static IEnumerable<InfoEntity> PraseJson(string jsonData)
        {
            JsonObject jsonObject = null;
            try
            {
                jsonObject = JsonObject.Parse(jsonData);
            }
            catch { }
            if (jsonObject == null)
                yield break;

            string status = string.Empty;
            try
            {
                status = jsonObject["status"].GetString();
            }
            catch { }
            
         
            JsonArray msgs=null;
            try
            { msgs = jsonObject["msgs"].GetArray(); }
            catch { }
            if (msgs == null)
                yield break;
            
            foreach (var msg in msgs)
            {
                InfoEntity info = new InfoEntity();
                try
                {
                    info.Level = Convert.ToInt32(msg.GetObject()["level"].GetNumber());
                    info.Info = msg.GetObject()["msg"].GetString();
                }
                catch(Exception ex) { yield break; }
                
                yield return info;
            }
        }
    }

    public class Converter : Windows.UI.Xaml.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is int?)
            {
                int level = (value as int?) ?? 1;
                if (level == 1)
                {
                    return new SolidColorBrush(Colors.Green);
                }
                else if (level == 2)
                {
                    return new SolidColorBrush( Colors.OrangeRed);
                }
                else if (level == 3)
                {
                    return new SolidColorBrush( Colors.Red);
                }
            }
            return new SolidColorBrush(Colors.Green);

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    //重要提示信息实体
    public class InfoEntity
    {
        public int Level { get; set; } = 1;
        public string Info { get; set; } = "";
    }

}
