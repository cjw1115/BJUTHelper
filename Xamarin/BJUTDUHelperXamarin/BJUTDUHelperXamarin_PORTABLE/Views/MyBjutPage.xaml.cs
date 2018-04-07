using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace BJUTDUHelperXamarin.Views
{
    public partial class MyBjutPage : ContentPage
    {
        
        public MyBjutPage()
        {
            InitializeComponent();
            this.Appearing += EduPage_Appearing;
            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += TapGestureRecognizer_Tapped;
            this.cvHeader.GestureRecognizers.Add(tapGesture);
        }
        

        private void EduPage_Appearing(object sender, System.EventArgs e)
        {
            LoadedCommand?.Execute(null);
        }
        public static readonly BindableProperty LoadedCommandProperty = BindableProperty.Create("LoadedCommand", typeof(ICommand), typeof(MyBjutPage), defaultBindingMode: BindingMode.OneWay);
        public ICommand LoadedCommand
        {
            get { return (ICommand)GetValue(LoadedCommandProperty); }
            set { SetValue(LoadedCommandProperty, value); }
        }

        #region CarouselView item 点击事件
        public static readonly BindableProperty TappedCommandProperty = BindableProperty.CreateAttached("TappedCommand", typeof(ICommand), typeof(PivotPagePortable.CarouselView), null, BindingMode.OneWay);
        public static ICommand GetTappedCommand(BindableObject o)
        {
            return (ICommand)o.GetValue(TappedCommandProperty);
        }
        public static void SetTappedCommand(BindableObject o,ICommand value)
        {
            o.SetValue(TappedCommandProperty, value);
        }
        private void TapGestureRecognizer_Tapped(object sender, System.EventArgs e)
        {
            var tapCommand = (ICommand)cvHeader.GetValue(TappedCommandProperty);
            tapCommand?.Execute(cvHeader.Item);
        }

        #endregion


    }

}
