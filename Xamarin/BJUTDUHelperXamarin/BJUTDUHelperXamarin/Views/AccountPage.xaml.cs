using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace BJUTDUHelperXamarin.Views
{
    public partial class AccountPage : ContentPage
    {
        public AccountPage()
        {
            InitializeComponent();
            this.Appearing += UserPage_Appearing;
        }

        

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }
        private void UserPage_Appearing(object sender, System.EventArgs e)
        {
            LoadedCommand?.Execute(null);
        }

        public static readonly BindableProperty LoadedCommandProperty = BindableProperty.Create("LoadedCommand", typeof(ICommand), typeof(AccountPage), defaultBindingMode: BindingMode.OneWay);
        public ICommand LoadedCommand
        {
            get { return (ICommand)GetValue(LoadedCommandProperty); }
            set { SetValue(LoadedCommandProperty, value); }
        }
    }
    
}
