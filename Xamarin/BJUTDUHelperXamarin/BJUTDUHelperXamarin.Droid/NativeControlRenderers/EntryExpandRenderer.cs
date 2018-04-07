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
using Xamarin.Forms.Platform.Android;
using Android.Util;
using System.ComponentModel;

[assembly: ExportRenderer(typeof(EntryExpand), typeof(BJUTDUHelperXamarin.Droid.NativeControls.EntryExpandRenderer))]
namespace BJUTDUHelperXamarin.Droid.NativeControls
{

    public class EntryExpandRenderer : EditorRenderer
    {
        private EditText _nativeControl;
        private EntryExpand _formsControl;
        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);

            _formsControl = this.Element as EntryExpand;
            _nativeControl = this.Control;

            this.Control.SetMinLines(1);
            //this.Control.Hint = "年轻人，说出你的想法";
            this.Control.TextChanged += Control_TextChanged;
            //this.Control.InputType = Android.Text.InputTypes.TextFlagMultiLine | Android.Text.InputTypes.ClassText;
            //this.Control.LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent);
            //this.Control.Gravity = GravityFlags.Left | GravityFlags.Bottom;
            this.Control.Hint = _formsControl.Placeholder;
            this.Control.Text = _formsControl.Text;
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == EntryExpand.PlaceholderProperty.PropertyName)
            {
                _nativeControl.Hint = this._formsControl.Placeholder;
            }
        }
        private bool isFirst = true;
        private void Control_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            if (isFirst)
            {
                isFirst = !isFirst;
                return;
            }
            var height = 0d;
            if (this.Control.LineCount <= 1)
            {
                height = this.Control.LineCount * this.Control.LineHeight
                + this.Control.PaddingBottom + this.Control.PaddingTop;
            }
            else
            {
                height = this.Control.LineCount * this.Control.LineHeight
                + this.Control.LineSpacingExtra * this.Control.LineSpacingMultiplier * (this.Control.LineCount - 1)
                + this.Control.PaddingBottom + this.Control.PaddingTop;
            }


            height = height / this.Control.Resources.DisplayMetrics.Density;

            if (height > 200)
            {
                this.Element.HeightRequest = 200;
            }
            else
            {
                this.Element.HeightRequest = height;

            }
        }
        private int dip2px(Context context, float dipValue)
        {
            var r = this.Control.Resources;
            return (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, dipValue, r.DisplayMetrics);
        }
    }
}