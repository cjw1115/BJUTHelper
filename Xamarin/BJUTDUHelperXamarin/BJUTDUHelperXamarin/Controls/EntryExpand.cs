using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace BJUTDUHelperXamarin.Controls
{
    public class EntryExpand:Editor
    {
        public static BindableProperty PlaceholderProperty =
           BindableProperty.Create("Placeholder", typeof(string), typeof(EntryExpand), string.Empty);

        public string Placeholder
        {
            get { return (string)GetValue(PlaceholderProperty); }
            set { SetValue(PlaceholderProperty, value); }
        }
    }
}
