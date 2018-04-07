using System.Windows.Input;
using Xamarin.Forms;
using static Xamarin.Forms.BindableProperty;

namespace BJUTDUHelperXamarin.Views
{
    public partial class CaptchaPage : ContentPage
    {
        public CaptchaPage()
        {
            InitializeComponent();
        }

       
        public static readonly BindableProperty TapCommandProperty = BindableProperty.CreateAttached("TapCommand", typeof(ICommand), typeof(Image), null, propertyChanged: (o, oldValue,newValue)=> 
        {
            var img=o as Image;
            img.GestureRecognizers.Clear();
            img.GestureRecognizers.Add(new TapGestureRecognizer() { Command = GetTapCommand(img) });

        });
        public static ICommand GetTapCommand(BindableObject bindObj)
        {
            return (ICommand)bindObj.GetValue(TapCommandProperty);
        }
        public static void SetTapCommand(BindableObject bindObj,Command value)
        {
            bindObj.SetValue(TapCommandProperty, value);
        }
    }
}
