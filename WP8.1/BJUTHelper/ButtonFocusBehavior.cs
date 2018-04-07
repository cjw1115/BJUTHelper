using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace BJUTHelper
{
    public class ButtonFocusBehavior : DependencyObject, IBehavior
    {
        private FrameworkElement frameworkElement;
        private Storyboard _storyboard = new Storyboard();
        private DoubleAnimation animationX = new DoubleAnimation();
        private DoubleAnimation animationY = new DoubleAnimation();
        public DependencyObject AssociatedObject
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void Attach(DependencyObject associatedObject)
        {
            frameworkElement = associatedObject as FrameworkElement;
            frameworkElement.PointerEntered += FrameworkElement_PointerEntered;
            frameworkElement.PointerExited += FrameworkElement_PointerExited;
            CompositeTransform trans = new CompositeTransform();
            
            frameworkElement.RenderTransform = trans;
            
            Storyboard.SetTarget(animationX, frameworkElement.RenderTransform);
            Storyboard.SetTargetProperty(animationX, nameof(CompositeTransform.ScaleX));

            _storyboard.Children.Add(animationX);
            Storyboard.SetTarget(animationY, frameworkElement.RenderTransform);
            Storyboard.SetTargetProperty(animationY, nameof(CompositeTransform.ScaleY));

            _storyboard.Children.Add(animationY);
        }

        private void FrameworkElement_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            SetAnimation(animationX, 1.5, 1);
            SetAnimation(animationY, 1.5, 1);
            _storyboard.Begin();
        }

        private void FrameworkElement_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            SetAnimation(animationX, 1, 1.5);
            SetAnimation(animationY, 1, 1.5);
            _storyboard.Begin();
        }
        private void SetAnimation(DoubleAnimation animation, double from, double to)
        {
            animation.From = from;
            animation.To = to;
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            animation.EasingFunction = new SineEase() { EasingMode = EasingMode.EaseInOut };
        }
        public void Detach()
        {
            frameworkElement.PointerExited -= FrameworkElement_PointerExited;
            frameworkElement.PointerEntered -= FrameworkElement_PointerEntered;
        }
    }
}
