using System;
using System.Collections;
using Xamarin.Forms;

namespace BJUTDUHelperXamarin.Controls
{
    public class BindablePicker : Picker
    {
        public BindablePicker()
        {
            this.SelectedIndexChanged += BindablePicker_SelectedIndexChanged;
            
        }

        public static BindableProperty ItemsSourceProperty =
            BindableProperty.Create("ItemsSource", typeof(IList), typeof(BindablePicker), default(IList), propertyChanged: OnItemsSourceChanged);

        public static BindableProperty SelectedItemProperty =
            BindableProperty.Create("SelectedItem",typeof(object), typeof(BindablePicker), default(object), propertyChanged: OnSelectedItemChanged);

        public IList ItemsSource
        {
            get { return (IList)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        private static void OnItemsSourceChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var picker = bindable as BindablePicker;
            picker.Items.Clear();
            if (newvalue != null)
            {
                //now it works like "subscribe once" but you can improve
                foreach (var item in (IEnumerable)newvalue)
                {
                    picker.Items.Add(item.ToString());
                }
            }
        }

        private void BindablePicker_SelectedIndexChanged(object sender, EventArgs eventArgs)
        {
            if (SelectedIndex < 0 || SelectedIndex > Items.Count - 1)
            {
                SelectedItem = null;
            }
            else
            {
                SelectedItem = Items[SelectedIndex];
            }
        }

        private static void OnSelectedItemChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var picker = bindable as BindablePicker;
            if (newvalue != null)
            {
                picker.SelectedIndex = picker.Items.IndexOf(newvalue.ToString());
            }
        }
    }
}