using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace BJUTDUHelperXamarin.Views
{
    public partial class UserPage : ContentPage
    {
        public UserPage()
        {
            InitializeComponent();
            this.Appearing += UserPage_Appearing;
            this.cellProxy.OnChanged += CellProxy_OnChanged;
            this.experimentalProxy.OnChanged += ExperimentalProxy_OnChanged;


        }

        

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }
        private void UserPage_Appearing(object sender, System.EventArgs e)
        {
            LoadedCommand?.Execute(null);
        }

        public static readonly BindableProperty LoadedCommandProperty = BindableProperty.Create("LoadedCommand", typeof(ICommand), typeof(UserPage), defaultBindingMode: BindingMode.OneWay);
        public ICommand LoadedCommand
        {
            get { return (ICommand)GetValue(LoadedCommandProperty); }
            set { SetValue(LoadedCommandProperty, value); }
        }

        

        public static readonly BindableProperty TappedCommandProperty =
   BindableProperty.CreateAttached("TappedCommand", typeof(ICommand), typeof(UserPage), null, defaultBindingMode: BindingMode.OneWay);

        public static void SetTappedCommand(BindableObject o,object value)
        {
            o.SetValue(TappedCommandProperty, value); 
        }
        public static ICommand GetTappedCommand(BindableObject o)
        {
            return (ICommand)o.GetValue(TappedCommandProperty);
        }
        private void CellProxy_OnChanged(object sender, ToggledEventArgs e)
        {
            GetTappedCommand(this)?.Execute(null);
        }

        public static readonly BindableProperty ExperimentalTappedCommandProperty =
   BindableProperty.CreateAttached("ExperimentalTappedCommand", typeof(ICommand), typeof(UserPage), null, defaultBindingMode: BindingMode.OneWay);

        public static void SetExperimentalTappedCommand(BindableObject o, object value)
        {
            o.SetValue(ExperimentalTappedCommandProperty, value);
        }
        public static ICommand GetExperimentalTappedCommand(BindableObject o)
        {
            return (ICommand)o.GetValue(ExperimentalTappedCommandProperty);
        }
        private void ExperimentalProxy_OnChanged(object sender, ToggledEventArgs e)
        {
            GetExperimentalTappedCommand(this)?.Execute(null);
        }
    }
    
}
