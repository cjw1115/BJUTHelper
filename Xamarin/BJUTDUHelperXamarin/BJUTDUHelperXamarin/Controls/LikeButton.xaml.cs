using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
namespace BJUTDUHelperXamarin.Controls
{
    
    public partial class LikeButton : ContentView
    {
        Color normalHeartColor;
        Color selectedHeartColor;
        public LikeButton()
        {
            
            InitializeComponent();
            normalHeartColor = (Color)this.Resources["NormalHeartColor"];
            selectedHeartColor = (Color)this.Resources["SelectedHeartColor"];

            var tap = new TapGestureRecognizer();
            tap.Tapped += OnTapped;
            this.GestureRecognizers.Add(tap);
        }

        private void OnTapped(object sender, EventArgs e)
        {
            Clicked?.Invoke(this, this.BindingContext);
            Command?.Execute(CommandParameter);
        }
        public event EventHandler<object> Clicked;
        public string Text
        {
            get => (string)this.GetValue(TextProperty);
            set => this.SetValue(TextProperty, value);
        }
        public static readonly BindableProperty TextProperty = BindableProperty.Create("Text", typeof(string), typeof(LikeButton),defaultValue:string.Empty,propertyChanged: TextPropertyChanged);
        public static void TextPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (oldValue != newValue)
            {
                ((LikeButton)bindable).OnTextPropertyChnaged(newValue as string);
            }
        }
        public void OnTextPropertyChnaged(string newValue)
        {
            lbText.Text = newValue;
        }


        public bool IsLike
        {
            get => (bool)this.GetValue(IsLikeProperty);
            set => this.SetValue(IsLikeProperty, value);
        }
        public static readonly BindableProperty IsLikeProperty = BindableProperty.Create("IsLike", typeof(bool), typeof(LikeButton), defaultValue: false, propertyChanged: IsLikePropertyChanged);
        public static void IsLikePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if ((bool)oldValue != (bool)newValue)
            {
                ((LikeButton)bindable).OnIsLikePropertyChanged((bool)newValue);
            }
        }
        public void OnIsLikePropertyChanged(bool newValue)
        {
            if (newValue == true)
            {
                lbHeart.TextColor = selectedHeartColor;
                lbText.TextColor = selectedHeartColor;
            }
            else
            {
                lbHeart.TextColor = normalHeartColor;
                lbText.TextColor = normalHeartColor;
            }
        }
        public Command Command
        {
            get => (Command)this.GetValue(CommandProperty);
            set => this.SetValue(CommandProperty, value);
        }
        public static readonly BindableProperty CommandProperty = BindableProperty.Create("Command", typeof(Command), typeof(LikeButton), defaultValue: null);
        public Object CommandParameter
        {
            get => (object)this.GetValue(CommandParameterProperty);
            set => this.SetValue(CommandParameterProperty, value);
        }
        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create("CommandParameter", typeof(object), typeof(LikeButton), defaultValue: null);
    }
}