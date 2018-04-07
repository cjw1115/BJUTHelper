using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class WIFIHelperRegView : Page
    {
        public ViewModel.WIFIHelperRegVM WIFIHelperRegVM { get; set; }
        public object navigationParam;
        public WIFIHelperRegView()
        {
            var locator = Application.Current.Resources["Locator"] as ViewModel.ViewModelLocator;
            WIFIHelperRegVM = locator.WIFIHelperRegVM;
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
            this.InitializeComponent();
            this.Loaded += WIFIHelperRegView_Loaded;

            progressBar.Closed += ProgressBar_Closed;
            progressBar.Acticed += ProgressBar_Acticed;
           

            imgRotation.Begin();

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
            WIFIHelperRegVM.cancellationTokenSource.Cancel();
        }

        private void WIFIHelperRegView_Loaded(object sender, RoutedEventArgs e)
        {
            WIFIHelperRegVM.Loaded(navigationParam);
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationParam = e.Parameter;
        }
        
    }
    public class VisibiltyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Visibility v = (Visibility)value;
            if(v== Visibility.Collapsed)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            Visibility v = (Visibility)value;
            if (v == Visibility.Collapsed)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }
    }
    public class ReBoolVisibiltyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            
            var v = (bool)value;
            if (v==false)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
           
            throw new Exception();
        }
    }
    public class BoolVisibiltyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {

            var v = (bool)value;
            if (v == true)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {

            throw new Exception();
        }
    }
}
