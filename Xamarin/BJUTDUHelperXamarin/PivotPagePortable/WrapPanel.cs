using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PivotPagePortable
{
    [ContentProperty("Children")]
    public class WrapPanel : Layout<View>
    {
        public WrapPanel()
        {

        }
        public static readonly BindableProperty RowSpacingProerpty = BindableProperty.Create("RowSpacing", typeof(double), typeof(WrapPanel), 0.0);
        public double RowSpacing
        {
            get { return (double)this.GetValue(RowSpacingProerpty); }
            set { SetValue(RowSpacingProerpty, value); }
        }
        public static readonly BindableProperty ColumnSpacingProerpty = BindableProperty.Create("ColumnSpacing", typeof(double), typeof(WrapPanel), 0.0);
        public double ColumnSpacing
        {
            get { return (double)this.GetValue(ColumnSpacingProerpty); }
            set { SetValue(ColumnSpacingProerpty, value); }
        }
        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            //var i = 0;
            //foreach (var item in this.Children)
            //{
            //    if (item.IsVisible == false)
            //    {
            //        continue;
            //    }
            //    if (i == itemsRect.Count)
            //    {
            //        break;
            //    }
            //    item.Layout(itemsRect[i++]);
            //}
            double[] colHeights = new double[this.Children.Count];

            var posX = 0d;
            var allHeight = 0d;
            var maxHeight = 0d;
            foreach (var item in this.Children)
            {
                if (item.IsVisible == false)
                {
                    continue;
                }
                var measuredSize = item.Measure(width,height, MeasureFlags.IncludeMargins);
                if (posX + measuredSize.Request.Width + ColumnSpacing > width)
                {
                    allHeight += maxHeight;

                    item.Layout(new Rectangle(0, allHeight, measuredSize.Request.Width, measuredSize.Request.Height));

                    maxHeight = measuredSize.Request.Height;
                    posX = measuredSize.Request.Width + ColumnSpacing;
                }
                else
                {

                    item.Layout(new Rectangle(posX, allHeight, measuredSize.Request.Width, measuredSize.Request.Height));

                    posX += measuredSize.Request.Width + ColumnSpacing;

                    if (measuredSize.Request.Height > maxHeight)
                    {
                        maxHeight = measuredSize.Request.Height;
                    }
                }
            }
        }
        private List<Rectangle> itemsRect { get; set; } = new List<Rectangle>();
        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {
            itemsRect.Clear();

            double[] colHeights = new double[this.Children.Count];

            var x = 0d;
            var allHeight = 0d;
            var maxHeight = 0d;
            foreach (var item in this.Children)
            {
                if (item.IsVisible == false)
                {
                    continue;
                }
                var measuredSize = item.Measure(widthConstraint, heightConstraint, MeasureFlags.IncludeMargins);
                if (x + measuredSize.Request.Width + ColumnSpacing > widthConstraint)
                {
                    x = 0;
                    allHeight += maxHeight;

                    itemsRect.Add(new Rectangle(x, allHeight, measuredSize.Request.Width, measuredSize.Request.Height));
                    maxHeight = measuredSize.Request.Height;
                }
                else
                {


                    itemsRect.Add(new Rectangle(x, allHeight, measuredSize.Request.Width, measuredSize.Request.Height));

                    x += measuredSize.Request.Width + ColumnSpacing;

                    if (measuredSize.Request.Height > maxHeight)
                    {
                        maxHeight = measuredSize.Request.Height;
                    }
                }
            }
            allHeight += maxHeight;
            return new SizeRequest(new Size(widthConstraint, allHeight));
        }


        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create("ItemsSource", typeof(IList), typeof(WrapPanel), null, propertyChanged: ItemsSource_PropertyChanged);
        public IList ItemsSource
        {
            get { return (IList)this.GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        private static void ItemsSource_PropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var flowLayout = (WrapPanel)bindable;
            var newItems = newValue as IList;
            var oldItems = oldValue as IList;
            var oldCollection = oldValue as INotifyCollectionChanged;
            if (oldCollection != null)
            {
                oldCollection.CollectionChanged -= flowLayout.OnCollectionChanged;
            }

            if (newValue == null)
            {
                return;
            }

            if (newItems == null)
                return;
            if (oldItems == null || newItems.Count != oldItems.Count)
            {
                flowLayout.Children.Clear();
                for (int i = 0; i < newItems.Count; i++)
                {

                    flowLayout.Children.Add((View)newItems[i]);
                }

            }

            var newCollection = newValue as INotifyCollectionChanged;
            if (newCollection != null)
            {
                newCollection.CollectionChanged += flowLayout.OnCollectionChanged;
            }
            flowLayout.ForceLayout();
        }


        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

            if (e.NewItems == null)
            {
                return;
            }
            for (int i = 0; i < e.NewItems.Count; i++)
            {
                this.Children.Add((View)e.NewItems[i]);
            }

            ForceLayout();
        }

    }
}
