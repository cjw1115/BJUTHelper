using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BJUTDUHelperXamarin.Controls
{
    public class ProgressTipBar:Grid
    {
        
        public ProgressTipBar()
        {
            BoxView boxview = new BoxView();
            boxview.VerticalOptions = LayoutOptions.Fill;
            boxview.HorizontalOptions = LayoutOptions.Fill;
            boxview.Color = Color.Gray;
            boxview.Opacity = 0.4;

            this.Children.Add(boxview);

            ActivityIndicator indicatior = new ActivityIndicator();
            indicatior.VerticalOptions = LayoutOptions.Center;
            indicatior.HorizontalOptions = LayoutOptions.Center;
            indicatior.IsRunning = true;
            indicatior.Color=(Color)App.Current.Resources["BJUTDUHelperMainBackground"];
            

            this.Children.Add(indicatior);

        }
        public static readonly BindableProperty TipProperty = BindableProperty.Create("Tip", typeof(string), typeof(ProgressTipBar), "······操作中······");
        public string Tip
        {
            get { return (string)GetValue(TipProperty); }
            set { SetValue(TipProperty, value); }
        }
    }
}
