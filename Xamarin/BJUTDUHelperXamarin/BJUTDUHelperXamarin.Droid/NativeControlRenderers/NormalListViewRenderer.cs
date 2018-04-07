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
using Xamarin.Forms.Platform.Android;
using System.ComponentModel;
using Android.Support.V7.Widget;
using System.Collections;
using BJUTDUHelperXamarin.Droid.NativeControls;
using BJUTDUHelperXamarin.Views.MyBJUT;

[assembly: ExportRenderer(typeof(Xamarin.Forms.ListView), typeof(NormalListViewRenderer))]
namespace BJUTDUHelperXamarin.Droid.NativeControls
{
    public class NormalListViewRenderer : ListViewRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.ListView> e)
        {
            base.OnElementChanged(e);
            var lv = (Android.Widget.ListView)this.Control;
            lv.SetSelector(Resource.Drawable.listSelector);
            lv.CacheColorHint = Color.FromRgba(0, 0, 0, 0).ToAndroid();
        }
    }
    
}