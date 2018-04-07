using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BJUTDUHelperXamarin.ValueConverters
{
    public class EmptyTipConverter : IValueConverter
    {
        public string PlaceHolderValue { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var tempValue=(string)value;
            if (string.IsNullOrWhiteSpace(tempValue))
            {
                return PlaceHolderValue;
            }
            else
            {
                return tempValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var str = value as string;
            if(str==null||str.Trim()== PlaceHolderValue)
            {
                return string.Empty;
            }
            else
            {
                return str;
            }
        }
    }
}
