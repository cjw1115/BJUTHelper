using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
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
    public sealed partial class WIFIHelperAuthView : Page
    {
        public ViewModel.WIFIHelperAuthVM WIFIHelperAuthVM { get; set; }
        private object navigationParam;
        public WIFIHelperAuthView()
        {
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
            this.InitializeComponent();

            var locator = Application.Current.Resources["Locator"] as ViewModel.ViewModelLocator;
            WIFIHelperAuthVM = locator.WIFIHelperAuthVM;

            this.Loaded += WIFIHelperAuthView_Loaded;


            progressBar.Closed += ProgressBar_Closed;
            progressBar.Acticed += ProgressBar_Acticed;
            //GalaSoft.MvvmLight.Messaging.Messenger.Default.Register<string>(this,"WIFIHelperAuth",
            //    async (message) => { await new MessageDialog(message).ShowAsync(); });

            
        }
        private void ProgressBar_Closed(object sender, EventArgs e)
        {
            Service.NavigationService.UnRegSingleHandler(ProgressBar_BackRequested);
        }

        private void ProgressBar_Acticed(object sender, EventArgs e)
        {
            Service.NavigationService.RegSingleHandler(ProgressBar_BackRequested);
        }

        private void ProgressBar_BackRequested(object sender, BackRequestedEventArgs e)
        {
            e.Handled = true;
            progressBar.Active = false;
            WIFIHelperAuthVM.cancellationTokenSource.Cancel();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationParam = e.Parameter;
        }
        private void WIFIHelperAuthView_Loaded(object sender, RoutedEventArgs e)
        {

            WIFIHelperAuthVM.Loaded(navigationParam);
        }
    }

    public class ForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            byte sign = (byte)value;
            SolidColorBrush brush;
            Color color;
            switch (sign)
            {
                case 0: color= Colors.Black;break;
                case 1: color= Colors.Red; break;
                case 2: color= Colors.Orange; break;
                case 3: color= Colors.GreenYellow; break;
                case 4: color= Colors.Green; break;
                default: color=Colors.Black;break;
            }
            return brush = new SolidColorBrush(color);
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class TextIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            byte sign = (byte)value;
            //Encoding.UTF8.GetString()
            switch (sign)
            {
                case 0: return "\xE904";
                case 1: return "\xE905";
                case 2: return "\xE906";
                case 3: return "\xE907";
                case 4: return "\xE908";
                default:
                    return "\xE908";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class UserinfoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Model.BJUTInfoCenterUserinfo user = (Model.BJUTInfoCenterUserinfo)value;
            return user;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            Model.BJUTInfoCenterUserinfo user = (Model.BJUTInfoCenterUserinfo)value;
            return user;
        }
    }

}
