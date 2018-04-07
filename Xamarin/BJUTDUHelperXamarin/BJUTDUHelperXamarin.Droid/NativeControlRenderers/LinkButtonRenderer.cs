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

[assembly: ExportRenderer(typeof(LinkButton), typeof(BJUTDUHelperXamarin.Droid.NativeControls.LinkButtonRenderer))]
namespace BJUTDUHelperXamarin.Droid.NativeControls
{
   
    public class LinkButtonRenderer : ButtonRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Button> e)
        {
            base.OnElementChanged(e);
            this.Control.SetPadding(0, 0, 0, 0);
            this.Control.SetTextColor(this.Resources.GetColorStateList(Resource.Drawable.linkbutton));
        }

        public Android.Content.Res.ColorStateList GetTextColors()
        {
            int[] colors = new int[] { 0xf38f5c, 0x553727 , 0x553727 };
            int[][] states = new int[3][];
            states[0] = new int[] { Android.Resource.Attribute.StateEnabled};
            states[1] = new int[] { Android.Resource.Attribute.StateEnabled,Android.Resource.Attribute.StatePressed,Android.Resource.Attribute.StateFocused};
            states[2] = new int[] { Android.Resource.Attribute.StateEnabled, Android.Resource.Attribute.StatePressed};
            Android.Content.Res.ColorStateList colorList = new Android.Content.Res.ColorStateList(states, colors);
            return colorList;
        }
    }
}