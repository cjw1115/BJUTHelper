using BJUTDUHelperXamarin.Controls;
using BJUTDUHelperXamarin.iOS.NativeControlRenderers;
using CoreGraphics;
using Foundation;
using System;
using System.ComponentModel;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using RectangleF = CoreGraphics.CGRect;

[assembly: ExportRenderer(typeof(BJUTDUHelperXamarin.Controls.EntryExpand), typeof(EntryExpandRenderer))]

namespace BJUTDUHelperXamarin.iOS.NativeControlRenderers
{
    public class EntryExpandRenderer : ViewRenderer<EntryExpand, TextViewExpand.XXTextView>
    {
        UIToolbar _accessoryView;

        IElementController ElementController => Element as IElementController;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Control.Changed -= HandleChanged;
                Control.Started -= OnStarted;
                Control.Ended -= OnEnded;
            }

            base.Dispose(disposing);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<EntryExpand> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                SetNativeControl(new TextViewExpand.XXTextView());

                if (Device.Idiom == TargetIdiom.Phone)
                {
                    // iPhone does not have a dismiss keyboard button
                    var keyboardWidth = UIScreen.MainScreen.Bounds.Width;
                    _accessoryView = new UIToolbar(new RectangleF(0, 0, keyboardWidth, 44)) { BarStyle = UIBarStyle.Default, Translucent = true };

                    var spacer = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace);
                    var doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, (o, a) =>
                    {
                        Control.ResignFirstResponder();
                        //Element.SendCompleted();
                    });
                    _accessoryView.SetItems(new[] { spacer, doneButton }, false);
                    Control.InputAccessoryView = _accessoryView;
                }

                NSNotificationCenter.DefaultCenter.AddObserver(new NSString(UIKeyboard.WillShowNotification), (o) =>
                {
                    var info = o.UserInfo;
                    var size = (NSValue)info.ObjectForKey(new NSString(UIKeyboard.FrameEndUserInfoKey));
                    
                    ((Grid)this.Element.Parent).TranslateTo(0, -size.CGRectValue.Height);
                });
                NSNotificationCenter.DefaultCenter.AddObserver(new NSString(UIKeyboard.WillHideNotification), (o) =>
                {
                    ((Grid)this.Element.Parent).TranslateTo(0, 0);
                });

                Control.Changed += HandleChanged;
                Control.Started += OnStarted;
                Control.Ended += OnEnded;

                this.Element.TextChanged += Element_TextChanged;
            }

            if (e.NewElement != null)
            {
                UpdateText();
                UpdateFont();
                UpdateTextColor();
                UpdateKeyboard();
                UpdateEditable();
                UpdatePlaceholder();
                UpdatePlaceholderColor();
            }
        }

        private void Element_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.Element.Text))
            {
                this.Element.HeightRequest = 36;
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == EntryExpand.TextProperty.PropertyName)
                UpdateText();
            else if (e.PropertyName == Xamarin.Forms.InputView.KeyboardProperty.PropertyName)
                UpdateKeyboard();
            else if (e.PropertyName == VisualElement.IsEnabledProperty.PropertyName)
                UpdateEditable();
            else if (e.PropertyName == EntryExpand.TextColorProperty.PropertyName)
                UpdateTextColor();
            else if (e.PropertyName == EntryExpand.FontAttributesProperty.PropertyName)
                UpdateFont();
            else if (e.PropertyName == EntryExpand.FontFamilyProperty.PropertyName)
                UpdateFont();
            else if (e.PropertyName == EntryExpand.FontSizeProperty.PropertyName)
                UpdateFont();
            else if (e.PropertyName == EntryExpand.PlaceholderProperty.PropertyName)
                UpdatePlaceholder();
        }

        void HandleChanged(object sender, EventArgs e)
        {
            ElementController.SetValueFromRenderer(EntryExpand.TextProperty, Control.Text);


            if (this.Control.Frame.Height != this.Control.ContentSize.Height)
            {
                if (this.Control.ContentSize.Height > 200)
                {
                    this.Element.HeightRequest = 200;
                }
                else
                {
                    this.Element.HeightRequest = this.Control.ContentSize.Height;
                }

            }

        }

        void OnEnded(object sender, EventArgs eventArgs)
        {
            if (Control.Text != Element.Text)
                ElementController.SetValueFromRenderer(EntryExpand.TextProperty, Control.Text);
            Element.Unfocus();
            //Element.SetValue(VisualElement.IsFocusedProperty, false);
            //Element.SendCompleted();
        }

        void OnStarted(object sender, EventArgs eventArgs)
        {
            Element.Focus();
            //ElementController.SetValueFromRenderer(VisualElement.IsFocusedProperty, true);
        }

        void UpdateEditable()
        {
            Control.Editable = Element.IsEnabled;
            Control.UserInteractionEnabled = Element.IsEnabled;

            if (Control.InputAccessoryView != null)
                Control.InputAccessoryView.Hidden = !Element.IsEnabled;
        }

        void UpdateFont()
        {
            //Control.Font = UIFont.FromName(this.Element.FontFamily,Control.font)
        }

        void UpdateKeyboard()
        {
            Control.ApplyKeyboard(Element.Keyboard);
            Control.ReloadInputViews();
        }

        void UpdateText()
        {
            // ReSharper disable once RedundantCheckBeforeAssignment
            if (Control.Text != Element.Text)
                Control.Text = Element.Text;
        }

        void UpdateTextColor()
        {
            //var textColor = Element.TextColor;
            //Control.TextColor = textColor.ToUIColor();
        }

        public void UpdatePlaceholder()
        {
            Control.Xx_placeholder = this.Element.Placeholder;
        }
        public void UpdatePlaceholderColor()
        {
            Control.Xx_placeholderColor = UIColor.LightGray;
        }
    }
}
