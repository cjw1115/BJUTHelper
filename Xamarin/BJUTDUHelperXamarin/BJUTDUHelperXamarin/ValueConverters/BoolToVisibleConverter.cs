using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace BJUTDUHelperXamarin.ValueConverters
{
    public class BoolToVisibleConverter : IValueConverter
    {
        public bool TrueToDisplay { get; set; } = true;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (TrueToDisplay)
            {
                return (bool)value;
            }
            else
            {
                return !(bool)value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
