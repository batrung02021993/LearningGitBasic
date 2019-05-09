using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VinaInvoice.Data.DataContext;
using VinaInvoice.ViewModel;

namespace VinaInvoice.Common
{
    public static class DateTimeConvert
    {
        /// <summary>
        /// The function convert timestamp to datetime
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
		
		public static DuringTimePeriod GetQuarterTimeStamp(int Year, int currQuarter)
		{
			try
			{
				DateTime dtFirstDay = new DateTime(Year, 3 * currQuarter - 2, 1);
				DateTime dtLastDay;
				if (currQuarter == 4)
				{
					dtLastDay = new DateTime(Year + 1, 1, 1).AddDays(-1);
				}
				else
				{

					dtLastDay = new DateTime(Year, 3 * currQuarter + 1, 1).AddDays(-1);
				}


				double hour = -(double)dtFirstDay.Hour;
				dtFirstDay = dtFirstDay.AddHours(hour);
				 hour = -(double)dtLastDay.Hour + 24;
				dtLastDay = dtLastDay.AddHours(hour);

				DuringTimePeriod period = new DuringTimePeriod
				{
				StartTime = GetTimeStamp(dtFirstDay),
					EndTime = GetTimeStamp(dtLastDay),
					EndTimeDT = dtLastDay,
					StartTimeDT = dtFirstDay
				};

				return period;
			}
			catch
			{
				return null;
			}
			
		}

		public static DuringTimePeriod GetMonthTimeStamp(int Year, int Month)
		{
			DateTime dtFirstDay = new DateTime(Year, Month, 1);
            DateTime dtLastDay;
            if (Month == 12)
            {
                 dtLastDay = new DateTime(Year + 1, 1, 1).AddDays(-1);
            }
            else
            {
                 dtLastDay = new DateTime(Year, Month + 1, 1).AddDays(-1);
            }

			double hour = -(double)dtFirstDay.Hour;
			dtFirstDay = dtFirstDay.AddHours(hour);
			hour = -(double)dtLastDay.Hour + 24;
			dtLastDay = dtLastDay.AddHours(hour);

			DuringTimePeriod period = new DuringTimePeriod
			{
				StartTime = GetTimeStamp(dtFirstDay),
				EndTime = GetTimeStamp(dtLastDay),
				EndTimeDT = dtLastDay,
				StartTimeDT = dtFirstDay
			};

			return period;
		}

		public static DateTime GetdatetimeFromStamp(double timestamp)
        {
            System.DateTime dateTime = new System.DateTime(1970, 1, 1, 7, 0, 0, 0, DateTimeKind.Local);
            dateTime = dateTime.AddSeconds(timestamp);
            return dateTime;
        }


        /// <summary>
        /// The function return list date time beetween start day and end day
        /// </summary>
        /// <param name="startingDate"></param>
        /// <param name="endingDate"></param>
        /// <returns></returns>
        public static ObservableCollection<SignDay> GetListSignDay(DateTime startingDate, DateTime endingDate)
        {
            List<DateTime> allDates = new List<DateTime>();
            ObservableCollection<SignDay> days = new ObservableCollection<SignDay>();

            for (DateTime date = endingDate; date.Date >= startingDate.Date; date = date.AddDays(-1))
			{
				if (date == endingDate) date.AddSeconds(-1);
				if (date.Date == startingDate.Date) date = startingDate.AddSeconds(1);
				allDates.Add(date);
			}
               
            foreach (var d in allDates)
            {
                var sday = new SignDay()
                {
                    dayview = d.ToString("dd/MM/yyyy"),
                    daytimeStamp = GetTimeStamp(d)

                };
                days.Add(sday);
            }
            return days;
        }
		

        /// <summary>
        /// Convert datetime to timstamp in string 
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        public static string GetTimeStamp(DateTime day)
        {
            TimeSpan span = day.Subtract(new DateTime(1970, 1, 1, 7, 0, 0, DateTimeKind.Local));
            int re = (int)span.TotalSeconds;
            return re.ToString();
        }

    }

	public class DuringTimePeriod
	{
		public string StartTime { get; set; }
		public string EndTime { get; set; }
		public DateTime StartTimeDT { get; set; }
		public DateTime EndTimeDT { get; set; }
	}
}
