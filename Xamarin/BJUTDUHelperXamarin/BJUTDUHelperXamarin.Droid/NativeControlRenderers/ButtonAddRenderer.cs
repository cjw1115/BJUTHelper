using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Graphics.Drawables;


[assembly: ExportRenderer(typeof(BJUTDUHelperXamarin.Controls.ButtonAdd), typeof(BJUTDUHelperXamarin.Droid.NativeControls.ButtonAddRenderer))]
[assembly: ExportRenderer(typeof(BJUTDUHelperXamarin.Controls.ButtonDel), typeof(BJUTDUHelperXamarin.Droid.NativeControls.ButtonDelRenderer))]
namespace BJUTDUHelperXamarin.Droid.NativeControls
{
    public class ButtonAddRenderer:ButtonRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Button> e)
        {
            base.OnElementChanged(e);
            if (this.Control != null)
            {
                this.Control.Background = this.Resources.GetDrawable("imgadd.png"); 
            }
        }
    }
    public class ButtonDelRenderer : ButtonRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Button> e)
        {
            base.OnElementChanged(e);
            if (this.Control != null)
            {
                this.Control.Background = this.Resources.GetDrawable("imgdel.png");
            }
        }
    }
}