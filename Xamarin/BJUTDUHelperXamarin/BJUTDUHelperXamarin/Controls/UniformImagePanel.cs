using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BJUTDUHelperXamarin.Controls
{
    public class UniformImagePanel : Layout<View>
    {
        TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer();
        public UniformImagePanel()
        {
            tapGestureRecognizer.Tapped += TapGesture_Tapped;
        }
        public double DividerWidth { get; set; } = 4;
        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {
            if (double.IsInfinity(widthConstraint))
                return new SizeRequest(new Size(0, 0));
            if (this.Children == null || this.Children.Count == 0)
            {
                return new SizeRequest(new Size(0, 0));
            }
            var perWidth = (widthConstraint - DividerWidth * 2) / 3;
            var t1 = this.Children.Count / 3;
            var t2 = this.Children.Count % 3;
            if (t2 == 0)
            {
                var height = t1 * perWidth;
                if (t1 > 1)
                {
                    height += (t1 - 1) * DividerWidth;
                }
                return new SizeRequest(new Size(widthConstraint, height));
            }
            else
            {
                var height = (t1 + 1) * perWidth;
                if (t1 + 1 > 1)
                {
                    height += (t1) * DividerWidth;
                }
                return new SizeRequest(new Size(widthConstraint, height));
            }
        }
        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            if (this.Children == null || this.Children.Count == 0)
            {
                return;
            }
            var perWidth = (width - DividerWidth * 2) / 3 ;
            for (int i = 0; i < this.Children.Count; i++)
            {
                var col=i % 3;
                var row = i / 3;
                var rect = new Rectangle(col*(perWidth+ DividerWidth),row* (perWidth + DividerWidth), perWidth,perWidth);
                this.Children[i].Layout(rect);
            }
        }
        //public List<View> Children { get; set; }
        public IList<string> ItemsSource
        {
            get { return (IList<string>)this.GetValue(ItemsSourceProperty); }
            set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }
        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create("ItemsSource", typeof(IList<string>), typeof(UniformImagePanel), defaultValue: null, propertyChanged: ItemsSourcePropertyChanged);
        public static void ItemsSourcePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var panel = bindable as UniformImagePanel;
            var items = newValue as IList<string>;
            if (items == null || items.Count == 0)
            {
                return;
            }

            panel.Children.Clear();
            foreach (var item in items)
            {
                Image img = new Image();
                img.Aspect = Aspect.AspectFill;
                img.Source = UriImageSource.FromUri(new Uri(item+ "-small_x150"));
                panel.Children.Add(img);
                img.GestureRecognizers.Add(panel.tapGestureRecognizer);
            }
            panel.InvalidateLayout();

        }

        private void TapGesture_Tapped(object sender, EventArgs e)
        {
            var index = this.Children.IndexOf(sender as View);
            
            ItemClicked?.Invoke(this, new UniformImagePaneItemClickedEventArgs() { Index = index });
        }

        public event EventHandler<UniformImagePaneItemClickedEventArgs> ItemClicked;
    }
    public class UniformImagePaneItemClickedEventArgs: EventArgs
    {
        public int Index { get; set; }
    }
}
