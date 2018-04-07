using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace BJUTDUHelper.Control
{
    public sealed partial class CheckCodeDlg : UserControl
    {
        public CheckCodeDlg()
        {
            this.InitializeComponent();
            this.Visibility = Visibility.Collapsed;
        }

        private void Backrequest_Handler(object o, BackRequestedEventArgs e)
        {
            e.Handled = true;
            Open = false;
        }
        public event EventHandler Saved;
        public event EventHandler Refresh;
        public static readonly DependencyProperty OpenProperty = DependencyProperty.Register("Open", typeof(bool), typeof(CheckCodeDlg), new PropertyMetadata(false, (o, e) =>
        {
            var dlg = o as CheckCodeDlg;

            var isopen = dlg.Open;
            if (isopen == true)
            {
                dlg.Visibility = Visibility.Visible;
                Service.NavigationService.RegSingleHandler(dlg.Backrequest_Handler);
            }
            else
            {
                dlg.Visibility = Visibility.Collapsed;
                Service.NavigationService.UnRegSingleHandler(dlg.Backrequest_Handler);
            }
        }));
        public bool Open
        {
            get { return (bool)GetValue(OpenProperty); }
            set { SetValue(OpenProperty, value); }
        }


        public static readonly DependencyProperty CheckCodeProperty = DependencyProperty.Register("CheckCode", typeof(string), typeof(CheckCodeDlg), new PropertyMetadata(null));
        public string CheckCode
        {
            get { return (string)GetValue(CheckCodeProperty); }
            set { SetValue(CheckCodeProperty, value); }
        }

        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(CheckCodeDlg), new PropertyMetadata(null));
        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }
        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            Open = false;
            
            Saved?.Invoke(this,null);
        }

        private void Image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Refresh?.Invoke(this,null);
        }
    }
}
