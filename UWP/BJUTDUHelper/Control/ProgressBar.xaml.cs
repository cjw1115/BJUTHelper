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
    public sealed partial class ProgressBar : UserControl
    {
        public ProgressBar()
        {
            this.InitializeComponent();
        }
        public event EventHandler Acticed;
        public event EventHandler Closed;

        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof(string), typeof(ProgressBar), new PropertyMetadata(null));
        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public static readonly DependencyProperty ActiveProperty = DependencyProperty.Register("Message", typeof(string), typeof(ProgressBar), new PropertyMetadata(false,
            (o,e)=>
            {
                var bar = o as ProgressBar;
                var status = (bool)e.NewValue;
                if (status)
                {
                    bar.Visibility = Visibility.Visible;
                    bar.Acticed?.Invoke(bar,null);
                    
                }
                else
                {
                    bar.Visibility = Visibility.Collapsed;
                    bar.Message = string.Empty;
                    bar.Closed?.Invoke(bar, null);
                }
            }));

        public bool Active
        {
            get { return (bool)GetValue(ActiveProperty); }
            set { SetValue(ActiveProperty, value); }
        }
    }

    public class BoolVisibiltyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool v = (bool)value;
            if (v)
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
            throw new NotImplementedException();
        }
    }
}
