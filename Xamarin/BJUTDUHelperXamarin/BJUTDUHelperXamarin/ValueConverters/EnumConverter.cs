using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace BJUTDUHelperXamarin.ValueConverters
{
    public class EnumConverter<T> : IValueConverter
    {
        //private Type _enumType;
        //public EnumConverter(Type enumType)
        //{
        //    _enumType = enumType;
        //}
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }
            var re= (T)value;
            return re.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            return Enum.Parse(typeof(T),value as string);
        }
    }
}
