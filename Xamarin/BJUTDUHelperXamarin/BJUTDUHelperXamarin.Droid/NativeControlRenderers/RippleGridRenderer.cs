using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using BJUTDUHelperXamarin.Controls;
using BJUTDUHelperXamarin.Droid.NativeControlRenderers;
using Xamarin.Forms.Platform.Android;
using Android.Graphics.Drawables;
using System.ComponentModel;

[assembly:ExportRenderer(typeof(RippleView),typeof(RippleViewRenderer))]
namespace BJUTDUHelperXamarin.Droid.NativeControlRenderers
{
    public class RippleViewRenderer: ButtonRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Button> e)
        {
            base.OnElementChanged(e);
            this.Control.SetPadding(0, 0, 0, 0);
            this.Control.Background = this.Resources.GetDrawable("ripple_bg");
            this.Control.Clickable = true;
            
        }
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Grid.BackgroundColorProperty.PropertyName)
            {
                return;
            }
            base.OnElementPropertyChanged(sender, e);
            
        }
    }
}