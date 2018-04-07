using System.Windows.Input;
using Xamarin.Forms;

namespace BJUTDUHelperXamarin.Views
{
    public partial class WifiHelperPage : ContentPage
    {
        public WifiHelperPage()
        {
            InitializeComponent();
            this.Appearing += WifiHelperPage_Appearing;
            
        }

        private void WifiHelperPage_Appearing(object sender, System.EventArgs e)
        {
            LoadedCommand?.Execute(null);
        }

        public static readonly BindableProperty LoadedCommandProperty = BindableProperty.Create("LoadedCommand", typeof(ICommand), typeof(EduPage), defaultBindingMode: BindingMode.OneWay);
        public ICommand LoadedCommand
        {
            get { return (ICommand)GetValue(LoadedCommandProperty); }
            set { SetValue(LoadedCommandProperty, value); }
        }
    }
}
