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
using Android.Graphics;
using Android.Animation;
using Android.Util;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using BJUTDUHelperXamarin.Controls;
using Android.Graphics.Drawables;

[assembly: ExportRenderer(typeof(CornerView), typeof(BJUTDUHelperXamarin.Droid.NativeControls.CornerViewRenderer))]
namespace BJUTDUHelperXamarin.Droid.NativeControls
{
   
    public class CornerViewRenderer : ViewRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.View> e)
        {
            base.OnElementChanged(e);
            
            GradientDrawable drawable = new GradientDrawable();//创建drawable
            drawable.SetColor(this.Element.BackgroundColor.ToAndroid());
            drawable.SetCornerRadius(20);
            //this.Background = drawable;
            this.Background = this.Resources.GetDrawable("momentselector");
            this.Clickable = true;
        }
    }
}