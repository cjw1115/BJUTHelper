using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using Xamarin.Forms;

namespace BJUTDUHelperXamarin.Controls
{
    
    [ContentProperty("Children")]
    public class ItemsControl : Layout<View>
    {
        public ItemsControl()
        {

        }
        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            if (double.IsInfinity(width))
                return;
            
            var allHeight = 0d;
            foreach (var item in this.Children)
            {
                if (item.IsVisible == false)
                {
                    continue;
                }
                var measuredSize = item.Measure(width, height, MeasureFlags.IncludeMargins);
                item.Layout(new Rectangle(0, allHeight, width, measuredSize.Request.Height));
                allHeight += measuredSize.Request.Height;
            }
        }
        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {
            if (double.IsInfinity(widthConstraint))
                return new SizeRequest(new Size(0,0));
            
            double[] colHeights = new double[this.Children.Count];
            var allHeight = 0d;
            foreach (var item in this.Children)
            {
                if (item.IsVisible == false)
                {
                    continue;
                }
                var measuredSize = item.Measure(widthConstraint, heightConstraint, MeasureFlags.IncludeMargins);
                allHeight += measuredSize.Request.Height;
            }
            return new SizeRequest(new Size(widthConstraint, allHeight));
        }


        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create("ItemsSource", typeof(IList), typeof(ItemsControl), null, propertyChanged: ItemsSource_PropertyChanged);
        public IList ItemsSource
        {
            get { return (IList)this.GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        private static void ItemsSource_PropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var itemsControl = (ItemsControl)bindable;
            var items = newValue as IList;

            var oldCollection = oldValue as INotifyCollectionChanged;
            if (oldCollection != null)
            {
                oldCollection.CollectionChanged -= itemsControl.OnCollectionChanged;
            }

            if (items == null)
            {
                return;
            }

            itemsControl.Children.Clear();
            for (int i = 0; i < items.Count; i++)
            {
                if (itemsControl.ItemTemplate == null)
                {
                    throw new Exception("ItemTemplate can not be null");
                }
                var child = (View)itemsControl.ItemTemplate.CreateContent();
                child.BindingContext = items[i];

                itemsControl.Children.Add(child);
            }

            var newCollection = newValue as INotifyCollectionChanged;
            if (newCollection != null)
            {
                newCollection.CollectionChanged += itemsControl.OnCollectionChanged;
            }
            itemsControl.ForceLayout();
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
                if (ItemTemplate == null)
                {
                    throw new Exception("ItemTemplate can not be null");
                }
                var child = (View)ItemTemplate.CreateContent();
                child.BindingContext = e.NewItems[i];
                this.Children.Add(child);
            }
            ForceLayout();
        }

        public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create("ItemTemplate", typeof(DataTemplate), typeof(ItemsControl), defaultValue: default(DataTemplate));
        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)this.GetValue(ItemTemplateProperty); }
            set { this.SetValue(ItemTemplateProperty, value); }
        }

    }
}
