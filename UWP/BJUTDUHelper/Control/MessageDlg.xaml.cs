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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace BJUTDUHelper.Control
{
    public sealed partial class MessageDlg : UserControl
    {
        public MessageDlg()
        {
            this.InitializeComponent();
            //this.Storyboard.Completed += (o, e) => { this.IsShow = false; };
        }
        public void Show(string message)
        {
            Message = message;
            this.Storyboard.Begin();
        }
        protected string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }
        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof(string), typeof(MessageDlg), new PropertyMetadata(null));

        //public bool IsShow
        //{
        //    get { return (bool)GetValue(IsShowProperty); }
        //    set { SetValue(IsShowProperty, value); }
        //}
        //public static readonly DependencyProperty IsShowProperty = DependencyProperty.Register("IsShow", typeof(bool), typeof(MessageDlg), new PropertyMetadata(false,
        //    (o, e) =>
        //    {
        //        var dlg = o as MessageDlg;
        //        bool value = (bool)e.NewValue;
        //        if (value)
        //        {
        //            dlg.Storyboard.Begin();
                    
                    
        //        }
        //    }));
    }
}
