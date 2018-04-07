using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
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
    public sealed partial class UserManagerView : Page
    {
        public ViewModel.UserManagerVM UserManagerVM { get; set; }
        public UserManagerView()
        {
            var locator = Application.Current.Resources["Locator"] as ViewModel.ViewModelLocator;
            UserManagerVM = locator.UserManagerVM;

            this.InitializeComponent();
            this.Loaded += UserManagerView_Loaded;

        }
        

        private void UserManagerView_Loaded(object sender, RoutedEventArgs e)
        {
            UserManagerVM.Loaded();
        }

        private void EditInfoClick(object sender, RoutedEventArgs e)
        {
            FrameworkElement grid= (FrameworkElement)VisualTreeHelper.GetParent((DependencyObject)sender);
            var user = grid.DataContext as Model.UserBase;
            if(user!=null)
                UserManagerVM.EditInfoClick(user.Username);
        }

        private void EditEduClick(object sender, RoutedEventArgs e)
        {
            FrameworkElement grid = (FrameworkElement)VisualTreeHelper.GetParent((DependencyObject)sender);
            var user = grid.DataContext as Model.UserBase;
            if (user != null)
                UserManagerVM.EditEduClick(user.Username);
        }

        private void EditLibClick(object sender, RoutedEventArgs e)
        {
            FrameworkElement grid = (FrameworkElement)VisualTreeHelper.GetParent((DependencyObject)sender);
            var user = grid.DataContext as Model.UserBase;
            if (user != null)
                UserManagerVM.EditLibClick(user.Username);
        }

        private void DeleteLibClick(object sender, RoutedEventArgs e)
        {
            FrameworkElement grid = (FrameworkElement)VisualTreeHelper.GetParent((DependencyObject)sender);
            var user = grid.DataContext as Model.UserBase;
            if (user != null)
                UserManagerVM.DeleteLibClick(user.Username);
        }
        private void DeleteEduClick(object sender, RoutedEventArgs e)
        {
            FrameworkElement grid = (FrameworkElement)VisualTreeHelper.GetParent((DependencyObject)sender);
            var user = grid.DataContext as Model.UserBase;
            if (user != null)
                UserManagerVM.DeleteEduClick(user.Username);
        }
        private void DeleteInfoClick(object sender, RoutedEventArgs e)
        {
            FrameworkElement grid = (FrameworkElement)VisualTreeHelper.GetParent((DependencyObject)sender);
            var user = grid.DataContext as Model.UserBase;
            if (user != null)
                UserManagerVM.DeleteInfoClick(user.Username);
        }


    }
    public class PlaceHolderTextConvereter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var str = value as string;
            if (!string.IsNullOrEmpty(str))
            {
                return $"{str}账号";
            }
            else
            {
                return "请输入账号";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class ColorBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Color color = (Color)value;
            return new SolidColorBrush(color);

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
