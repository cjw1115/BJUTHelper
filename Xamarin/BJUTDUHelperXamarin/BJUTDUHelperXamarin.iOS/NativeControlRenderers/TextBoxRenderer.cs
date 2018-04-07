
//using BJUTDUHelperXamarin.Controls;
//using BJUTDUHelperXamarin.iOS.NativeControls;
//using System.ComponentModel;
//using Xamarin.Forms.Platform.iOS;
//using Xamarin.Forms;
//using TextBox;
//using UIKit;

//[assembly:ExportRenderer(typeof(BJUTDUHelperXamarin.Controls.TextBox),typeof(TextBoxRenderer))]
//namespace BJUTDUHelperXamarin.iOS.NativeControls
//{
//    public class TextBoxRenderer:ViewRenderer<BJUTDUHelperXamarin.Controls.TextBox, XLTextFiledAnimation>
//    {
//        XLTextFiledAnimation _navtiveControl;
//        BJUTDUHelperXamarin.Controls.TextBox _formsControl;
//        protected override void OnElementChanged(ElementChangedEventArgs<BJUTDUHelperXamarin.Controls.TextBox> e)
//        {
//            base.OnElementChanged(e);

//            _navtiveControl = new TextBox.XLTextFiledAnimation();
//            _navtiveControl.Frame = new CoreGraphics.CGRect(0, 0, Element.WidthRequest, Element.HeightRequest);
//            this.SetNativeControl(_navtiveControl);

//            _navtiveControl.ValueChanged += _navtiveControl_ValueChanged;
//            _navtiveControl.AnimationLine = true;
//            _navtiveControl.PlaceHolderLabel.TextColor = UIColor.FromRGB(0x09, 0x97, 0xF7);
//            //_navtiveControl.PlaceHolderLabel.Frame = new CoreGraphics.CGRect(0, Element.WidthRequest / 2, Element.WidthRequest, Element.WidthRequest / 2);

//            _formsControl = this.Element;

//            if (_formsControl.IsPassword)
//            {
//                _navtiveControl.SecureTextEntry = true;
//            }
//            Update_Placeholder();
//        }
//        public void Update_Placeholder()
//        {
//            if (_navtiveControl.Placeholder != _formsControl.Placeholder)
//            {
//                _navtiveControl.PlaceHolderLabel.Text = _formsControl.Placeholder;
//            }
//        }

//        private void _navtiveControl_ValueChanged(object sender, System.EventArgs e)
//        {
//            if (_formsControl.Text != _navtiveControl.Text)
//            {
//                _formsControl.Text = _navtiveControl.Text;
//            }
//        }

//        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
//        {
//            base.OnElementPropertyChanged(sender, e);
//#if DEBUG
//            System.Diagnostics.Debug.WriteLine(e.PropertyName);
//#endif
//            if (e.PropertyName == BJUTDUHelperXamarin.Controls.TextBox.TextProperty.PropertyName)
//            {
//                if (_navtiveControl.Text != _formsControl.Text)
//                {
//                    _navtiveControl.Text = _formsControl.Text;
//                }
//                return;
//            }
//            if (e.PropertyName == BJUTDUHelperXamarin.Controls.TextBox.PlaceholderProperty.PropertyName)
//            {
//                Update_Placeholder();
                
//                return;
//            }
//        }
//    }
//}