using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace VinaInvoice.Common
{
    public static class InvoiceInvoice_Variable
    {
        #region Search Sort Method

        public static int sort_method = 1;
        public static int type_sort = 5;

        public static DateTime start_date = DateTime.Now.AddMonths(-1);
        public static DateTime stop_date = DateTime.Now;
        public static DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);
        public static string start_date_timestamp
        {
            get
            {
				// return (start_date.ToUniversalTime() - epoch).TotalSeconds.ToString();
				double hour = -(double)start_date.Hour;				
				return DateTimeConvert.GetTimeStamp(start_date.AddHours(hour));

			}
        }
        public static string stop_date_timestamp
        {
            get
            {
				//return (stop_date.ToUniversalTime() - epoch).TotalSeconds.ToString();
				double hour = -(double)stop_date.Hour + 24;
				return DateTimeConvert.GetTimeStamp(stop_date.AddHours(hour));
			}
        }

        public static string companyforsearching = "";
        public static string taxcodeforsearching = "";
        public static string invoicenumberforsearching = "";
        public static string invoicerefforsearching = "";
        public static string serialnameforsearching = "";

        #endregion
    }
}
