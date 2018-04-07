using System.Windows.Input;
using Xamarin.Forms;
namespace BJUTDUHelperXamarin.Views
{
    public partial class EduPage : ContentPage
    {
        public EduPage()
        {
            InitializeComponent();
            this.Appearing += EduPage_Appearing;
        }
        
        private void EduPage_Appearing(object sender, System.EventArgs e)
        {
            LoadedCommand?.Execute(null);

            listView.SelectedItem = null;
        }
        public static readonly BindableProperty LoadedCommandProperty = BindableProperty.Create("LoadedCommand", typeof(ICommand), typeof(EduPage), defaultBindingMode: BindingMode.OneWay);
        public ICommand LoadedCommand
        {
            get { return (ICommand)GetValue(LoadedCommandProperty); }
            set { SetValue(LoadedCommandProperty, value); }
        }
    }

}
