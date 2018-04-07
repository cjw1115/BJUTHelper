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
using Xamarin.Forms.Platform.Android;
using BJUTDUHelperXamarin.Controls;
using BJUTDUHelperXamarin.Droid.NativeControls;
using System.ComponentModel;
using Xamarin.Forms;
using BJUTDUHelperXamarin.Droid.NativeControlRenderers;

[assembly:ExportRenderer(typeof(AvartarClipper),typeof(AvartarClipperRenderer))]
namespace BJUTDUHelperXamarin.Droid.NativeControlRenderers
{
    public class AvartarClipperRenderer:ViewRenderer<AvartarClipper,ClipImageView>
    {
        ClipImageView _nativeControl;
        AvartarClipper _formsControl;
        protected override void OnElementChanged(ElementChangedEventArgs<AvartarClipper> e)
        {
            base.OnElementChanged(e);
            _nativeControl = new ClipImageView(this.Context,null);
            _formsControl = this.Element;
            this.SetNativeControl(_nativeControl);

            _formsControl.Clipper = _nativeControl;
        }
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if(e.PropertyName== AvartarClipper.PathProperty.PropertyName)
            {
                _nativeControl.SetBitmap(_formsControl.Path);
            }
        }
    }
}