using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using VinaInvoice.Common;

namespace VinaInvoice.Common.Converter
{
	public class doubleConverterQuantity : IValueConverter
	{

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			double value_double = (double)value;
			return value_double.ToString("N" + ConverterVariable.NUMBER_BEHIND_DOT);

		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var str = (string)value;
			if (str == "")
			{
				return 0;
			}
			else
			{
				double converted_value = 0;
				try
				{
					converted_value = double.Parse(str);

				}
				catch
				{

				}
				return converted_value;
			}
		}
	}

    public class doubleConverter1 : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double value_double = (double)value;
            return value_double.ToString("N" + 0);

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var str = (string)value;
            if (str == "")
            {
                return 1;
            }
            else
            {
                double converted_value = 0;
                try
                {
                    converted_value = double.Parse(str);

                }
                catch
                {

                }
                return converted_value;
            }
        }
    }

    public class doubleConverter : IValueConverter
    {
	
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double value_double = (double)value;
			return  value_double.ToString("N" + ConverterVariable.NUMBER_BEHIND_DOT);
			
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
			var str = (string)value;
            if (str == "")
            {
                return 0;
            }
            else
            {
                double converted_value = 0;
                try
                {
                    converted_value = double.Parse(str);

                }
                catch
                {
                   
                }
                return converted_value;
            }				
		}
    }

    public class IntegerConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int value_convert = (int)value;
            return value_convert.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return int.Parse(value.ToString());
        }
    }

    public class PerCentFortmatConverterServiceCharge : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			double value_int = Math.Round((double)value,2);
			return value_int.ToString()+ "%";

		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			
			var str = (string)value;
			str = str.Replace("%", "");
			if (str == "")
			{
				return 0;
			}
            double converted_value = 0;
            try
            {
                converted_value = Double.Parse(str);

            }
            catch { }

		    return converted_value;

		}
	}

	public class PerCentFortmatConverterDiscount: IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
		    double value_int = Math.Round((double)value, 2);
            return value_int.ToString() + "%";

		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
           
			var str = (string)value;
            if (str == "" || str == "%")
            {
                return 0;
            }
            else
            {
                str = str.Replace("%", "");
                double converted_value = Double.Parse(str);
                return converted_value;
            }
		}
	}

    public class InvoiceNumberConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                int invoiceNum = Int32.Parse((string)value);
                return invoiceNum.ToString("D7");

            }
            else return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
