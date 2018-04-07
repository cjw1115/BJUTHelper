using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class BJUTCampusCardView : Page
    {
        public ViewModel.BJUTCampusCardVM BJUTCampusCardVM { get; set; }
        public BJUTCampusCardView()
        {
            this.InitializeComponent();
            this.Loaded += BJUTCampusCardView_Loaded;
            var locator = Application.Current.Resources["Locator"] as ViewModel.ViewModelLocator;
            BJUTCampusCardVM = locator.BJUTCampusCardVM;
        }

        private void BJUTCampusCardView_Loaded(object sender, RoutedEventArgs e)
        {
            BJUTCampusCardVM.Loaded();
        }
    }
    public class CampusCardStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var str = (string)value;
            if (string.IsNullOrEmpty(str))
            {
                return Visibility.Visible;
            }
            if (str.Contains("在用"))
                return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
   
    
}
