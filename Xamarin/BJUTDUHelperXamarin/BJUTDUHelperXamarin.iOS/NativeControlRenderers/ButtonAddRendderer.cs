using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Foundation;
using Xamarin.Forms;
using UIKit;
using Xamarin.Forms.Platform.iOS;
using BJUTDUHelperXamarin.iOS.NativeControlRenderers;

[assembly: ExportRenderer(typeof(BJUTDUHelperXamarin.Controls.ButtonAdd), typeof(ButtonAddRenderer))]
[assembly: ExportRenderer(typeof(BJUTDUHelperXamarin.Controls.ButtonDel), typeof(ButtonDelRenderer))]
namespace BJUTDUHelperXamarin.iOS.NativeControlRenderers
{
    public class ButtonAddRenderer : ButtonRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Button> e)
        {
            base.OnElementChanged(e);
            if (this.Control != null)
            {
                this.Control.SetBackgroundImage(new UIImage("imgadd.png"), UIControlState.Normal);
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
                this.Control.SetBackgroundImage(new UIImage("imgdel.png"), UIControlState.Normal);
            }
        }
    }
}
