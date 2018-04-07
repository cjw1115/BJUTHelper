using Windows.Phone.UI.Input;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace ProgressView
{
    public sealed class ProgressIndicator : ContentControl
    {
        private Rectangle backgroundRect;

        private StackPanel stackPanel;
        private ProgressBar progressBar;
        private TextBlock textBlockStatus;

        private bool showLabel;
        public bool ShowLabel
        {
            get { return showLabel; }
            set { showLabel = value; }
        }
        private string labelText;
        public string Text
        {
            get { return labelText; }
            set { labelText = value; }
        }

        internal Popup ChildWindowPopup
        {
            get;
            private set;
        }

        public bool IsOpen
        {
            get { return ChildWindowPopup != null && ChildWindowPopup.IsOpen; }
        }

        public ProgressIndicator()
        {
            DefaultStyleKey = typeof(ProgressIndicator);


        }
        public void Show()
        {
            if (ChildWindowPopup == null)
            {
                ChildWindowPopup = new Popup();
                ChildWindowPopup.Height = Window.Current.Bounds.Height;
                ChildWindowPopup.Width = Window.Current.Bounds.Width;
                ChildWindowPopup.Child = this;
                this.Width = ChildWindowPopup.Width;
                this.Height = ChildWindowPopup.Height;

            }
            ChildWindowPopup.IsOpen = true;


            HardwareButtons.BackPressed += HardwareButtons_BackPressed;



        }
        public void Hide()
        {
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
            ChildWindowPopup.IsOpen = false;
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            if (IsOpen)
            {
                e.Handled = true;
                Hide();
            }
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            backgroundRect = GetTemplateChild("backgroundRect") as Rectangle;
            stackPanel = GetTemplateChild("stackPanel") as StackPanel;
            progressBar = GetTemplateChild("progressBar") as ProgressBar;
            textBlockStatus = GetTemplateChild("textBlockStatus") as TextBlock;

            //
            InitializeProgressType();
        }
        private void InitializeProgressType()
        {
            if (progressBar == null)
                return;



            backgroundRect.Visibility = Windows.UI.Xaml.Visibility.Visible;
            textBlockStatus.Visibility = Windows.UI.Xaml.Visibility.Visible;
            textBlockStatus.FontSize = 20;
            textBlockStatus.Opacity = 0.5;

            textBlockStatus.Text = Text;

            progressBar.IsIndeterminate = true;
        }
    }
}
