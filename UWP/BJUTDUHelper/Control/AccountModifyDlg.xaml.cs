using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
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
    public sealed partial class AccountModifyDlg : UserControl
    {
        public AccountModifyDlg()
        {
            this.InitializeComponent();
            this.btnClose.Click += BtnClose_Click;
            this.btnSave.Click += BtnSave_Click;
            this.DataContext = this;
        }

        private void GetNavigationHandler()
        {
            Service.NavigationService.RegSingleHandler(View_BackRequested);
        }
        private void DisposeNavigationHandler()
        {
            Service.NavigationService.UnRegSingleHandler(View_BackRequested);
        }

        public void View_BackRequested(object sender, BackRequestedEventArgs e)
        {
            BtnClose_Click(null, null);
            e.Handled = true;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbUsername.Text))
                tbUsername.Focus(FocusState.Keyboard);
            if (string.IsNullOrWhiteSpace(tbPassword.Password))
                tbPassword.Focus(FocusState.Keyboard);
            this.CloseDlgStoryboard.Begin(); Open = false; Save?.Invoke();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.CloseDlgStoryboard.Begin(); Open = false;
        }

        public string Username
        {
            get { return (string)this.GetValue(UsernameProperty); }
            set { SetValue(UsernameProperty, value); }
        }
        public static readonly DependencyProperty UsernameProperty = DependencyProperty.Register("Username", typeof(string), typeof(AccountModifyDlg), new PropertyMetadata(null));
        public string Password
        {
            get { return (string)this.GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }
        public static readonly DependencyProperty PasswordProperty = DependencyProperty.Register("Password", typeof(string), typeof(AccountModifyDlg), new PropertyMetadata(null));

        

        public static readonly DependencyProperty OpenProperty = DependencyProperty.Register("Open", typeof(bool), typeof(AccountModifyDlg), new PropertyMetadata(false, (o, e) =>
        {
            var dlg = o as AccountModifyDlg;

            var isopen = dlg.Open;
            if (isopen)
            {
                dlg.OpenDlgStoryboard.Begin();
                dlg.GetNavigationHandler();
            }
            else if (isopen==false)
            {
                dlg.DisposeNavigationHandler();  
            }
        }));
        
        public bool Open
        {
            get { return (bool)GetValue(OpenProperty); }
            set { SetValue(OpenProperty, value); }
        }

        public event Action Save;
        
    }
}
