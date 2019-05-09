using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace VinaInvoice.Common.Converter
{

    public class TaxConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
        
            if (value == null)
            {
                return 10;
            
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var str = (string)value;
            if (str == "Không chịu thuế")
            {
                return -1;
            }
            else if(str == "0%")
            {
                return 0;
            }
            else if (str == "5%")
            {
                return 5;
            }
            else if (str == "10%")
            {
                return 10;
            }
            else
            {
                return 15;
            }

        }

    }
}
