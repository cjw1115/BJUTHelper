using System.Windows.Input;
using Xamarin.Forms;


namespace BJUTDUHelperXamarin.Views
{
    public partial class CampusCardPage : ContentPage
    {
        public CampusCardPage()
        {
            InitializeComponent();
            this.Appearing += CampusCardPage_Appearing;
        }

        private void CampusCardPage_Appearing(object sender, System.EventArgs e)
        {
            LoadedCommand?.Execute(null);
        }

        public static readonly BindableProperty LoadedCommandProperty = BindableProperty.Create("LoadedCommand", typeof(ICommand), typeof(CampusCardPage), defaultBindingMode: BindingMode.OneWay);
        public ICommand LoadedCommand
        {
            get { return (ICommand)GetValue(LoadedCommandProperty); }
            set { SetValue(LoadedCommandProperty, value); }
        }
    }
}
