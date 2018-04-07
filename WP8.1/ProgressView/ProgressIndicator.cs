using System.Collections;
using System.Collections.Generic;
using Windows.Phone.UI.Input;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace ProgressView
{
    public class ProgressIndicator : ContentControl
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
        private Page RootPage
        {
            get
            {
                return Window.Current == null ? null : GetRootPage(Window.Current.Content);
            }
        }
        private Page GetRootPage(DependencyObject parent)
        {
            Queue<DependencyObject> queue = new Queue<DependencyObject>();
            queue.Enqueue(parent);
            while (queue.Count > 0)
            {
                var temp = queue.Dequeue();
                int count = VisualTreeHelper.GetChildrenCount(temp);
                for (int i = 0; i < count; i++)
                {
                    var child = VisualTreeHelper.GetChild(temp, i);
                    if (child is Page)
                    {
                        return child as Page;
                    }
                    else
                    {
                        queue.Enqueue(child);
                    }
                }
            }
            return null;
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

            if(Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily=="Windows.Mobile")
                HardwareButtons.BackPressed += HardwareButtons_BackPressed;

            if(RootPage!=null)
                RootPage.SizeChanged += RootPage_SizeChanged;

        }

        private void RootPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.Width = e.NewSize.Width;
            this.Height = e.NewSize.Height;
        }

        public void Hide()
        {
            if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
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
            if (RootPage != null)
            {
                RootPage.SizeChanged -= RootPage_SizeChanged;
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
            

            textBlockStatus.Text = Text == null ? "":Text ;

            progressBar.IsIndeterminate = true;
        }
    }
}
