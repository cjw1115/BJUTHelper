using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Xamarin.Forms.Platform.Android;
using Android.Widget;
using Xamarin.Forms;
using BJUTDUHelperXamarin.Controls;

[assembly: ExportRenderer(typeof(BJUTDUHelperXamarin.Controls.ProgressRing), typeof(BJUTDUHelperXamarin.Droid.NativeControls.ProgressRingRenderer))]
namespace BJUTDUHelperXamarin.Droid.NativeControls
{
    public class ProgressRingRenderer: ActivityIndicatorRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<ActivityIndicator> e)
        {
            base.OnElementChanged(e);
            this.Control.IndeterminateDrawable = this.Resources.GetDrawable("progressBar");
        }
    }
}