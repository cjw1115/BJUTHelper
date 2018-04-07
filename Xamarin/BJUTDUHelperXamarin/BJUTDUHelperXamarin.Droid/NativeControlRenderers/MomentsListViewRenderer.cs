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
using Android.Graphics.Drawables;

[assembly: ExportRenderer(typeof(BJUTDUHelperXamarin.Views.MyBJUT.MomentsListView), typeof(MomentsListViewRenderer))]
namespace BJUTDUHelperXamarin.Droid.NativeControls
{
    public class MomentsListViewRenderer : ListViewRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.ListView> e)
        {
            base.OnElementChanged(e);
            //this.Control.Divider = new Android.Graphics.Drawables.ColorDrawable(Color.FromRgb(0xef,0xef,0xef).ToAndroid());
            this.Control.Divider = new Android.Graphics.Drawables.ColorDrawable(Color.Transparent.ToAndroid());
            this.Control.DividerHeight = 12;
            this.Control.Selector = new ColorDrawable(Color.Transparent.ToAndroid());
        }
    }
    
}