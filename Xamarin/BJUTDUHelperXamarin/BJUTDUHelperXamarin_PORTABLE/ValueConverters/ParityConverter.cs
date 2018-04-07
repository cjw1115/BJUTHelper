using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BJUTDUHelperXamarin.ValueConverters
{
    public class ParityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int? old = (int?)value;
            if (old == null)
            {
                return null;
            }
            else if (old == 0)
            {
                return "（双周）";
            }
            else
            {
                return "（单周）";
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
