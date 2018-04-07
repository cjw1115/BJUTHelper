using Android.Views;
using Android.Widget;
using Xamarin.Forms.Platform.Android;
using BJUTDUHelperXamarin.Controls;
using Xamarin.Forms;
using Android.Support.Design.Widget;
using System.ComponentModel;
using BJUTDUHelperXamarin.Droid.NativeControls;

[assembly:Xamarin.Forms.ExportRenderer(typeof(TextBox),typeof(TextBoxRenderer))]
namespace BJUTDUHelperXamarin.Droid.NativeControls
{
    public class TextBoxRenderer:ViewRenderer
    {
        EditText _editText;
        TextBox _textBox;
        TextInputLayout _nativTextBox;
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.View> e)
        {
            base.OnElementChanged(e);
            _textBox = this.Element as TextBox;
            if (this.Control == null)
            {
                _nativTextBox = LayoutInflater.From(this.Context).Inflate(Resource.Layout.textbox, null) as TextInputLayout;
                this.SetNativeControl(_nativTextBox);

                _editText = this.FindViewById<EditText>(Resource.Id.editText);

                if (!_textBox.IsPassword)
                {
                    _editText.InputType = Android.Text.InputTypes.ClassText;
                }
                else
                {
                    _editText.InputType = Android.Text.InputTypes.ClassText | Android.Text.InputTypes.TextVariationPassword;
                }
                _editText.TextChanged += _editText_TextChanged;
                _nativTextBox.Hint= _textBox.Placeholder;
            }
            
        }

        private void _editText_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            
           if (_textBox.Text != _editText.Text)
            {
                _textBox.Text = _editText.Text;
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
#if DEBUG
            System.Diagnostics.Debug.WriteLine(e.PropertyName);
#endif
            if (e.PropertyName == TextBox.TextProperty.PropertyName)
            {
                if (_editText.Text != _textBox.Text)
                {
                    _editText.Text = _textBox.Text;
                }
                return;
            }
            if (e.PropertyName == TextBox.PlaceholderProperty.PropertyName)
            {
                if (_nativTextBox.Hint != _textBox.Placeholder)
                {
                    _nativTextBox.Hint = _textBox.Placeholder;
                }
                return;
            }
        }
    }
}