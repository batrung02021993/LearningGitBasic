using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace VinaInvoice.Common.Converter
{
    class DateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double a;
            try
            {
                a = Double.Parse((string)value);
            }
            catch
            {
                return "ERR("+ value+ ")";
            }
           
            
            return DateTimeConvert.GetdatetimeFromStamp(a).ToString("dd/MM/yyyy"); 
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "";
        }
    }
}
