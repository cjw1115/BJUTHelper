using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using Xamarin.Forms;
using BJUTDUHelperXamarin.iOS.NativeControlRenderers;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Xamarin.Forms.Picker), typeof(PickerExpandRenderer))]
namespace BJUTDUHelperXamarin.iOS.NativeControlRenderers
{
    public class PickerExpandRenderer:PickerRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Picker> e)
        {
            base.OnElementChanged(e);

            this.Control.SetValueForKeyPath(UIColor.FromRGB(0xc4, 0xc4, 0xc4),new NSString("_placeholderLabel.textColor"));
        }
    }
}