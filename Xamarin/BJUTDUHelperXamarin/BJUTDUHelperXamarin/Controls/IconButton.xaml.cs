using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace BJUTDUHelperXamarin.Controls
{
    public partial class IconButton : ContentView
    {
        public IconButton()
        {
            InitializeComponent();
            
            this.boxView.BindingContext = this;
            TransformCommand = new Command(Transform);
        }
        public ICommand TransformCommand { get; set; }
        public void Transform()
        {
            Task.Run(async () =>
            {
                int count = 1;
                while (count <= 5)
                {
                    this.boxView.Opacity = 0.01 * 3 * count++;
                    await Task.Delay(20);
                }
                await Task.Delay(100);
                while (count >= 0)
                {
                    this.boxView.Opacity = 0.01 * 3 * count--;
                    await Task.Delay(20);
                }
            });

            ClickCommand?.Execute(null);
        }
        
       
        public string Text
        {
            get { return text.Text; }
            set { text.Text = value; }
        }
        public string Source
        {
            get { return img.Text; }
            set { img.Text = value; }
        }

        public ICommand ClickCommand
        {
            get { return (ICommand)GetValue(ClickCommandProperty); }
            set
            {
                SetValue(ClickCommandProperty, value);
            }
        }
        public static readonly BindableProperty ClickCommandProperty = BindableProperty.Create("ClickCommand", typeof(ICommand), typeof(IconButton));

        private void boxViewTapped(object sender, EventArgs e)
        {
            Transform();
        }
    }
}
