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

[assembly: ExportRenderer(typeof(CircleButton), typeof(BJUTDUHelperXamarin.Droid.NativeControls.CircleButtonRenderer))]
namespace BJUTDUHelperXamarin.Droid.NativeControls
{
   
    public class CircleButtonRenderer : ButtonRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Button> e)
        {
            base.OnElementChanged(e);
            this.Control.SetPadding(0, 0, 0, 0);
        }
    }
}