using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TruMobility.Reporting.CDR
{
    public class TimeKeeper
    {
        // a day time span
        private static TimeSpan ts = new TimeSpan(1, 0, 0, 0);

        private static int m_reportDayTime  = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ReportDayTime"]);

        // a day interval forces to run from yesterday
        private static DateTime rDailyTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, m_reportDayTime, 0, 0).Subtract(TimeKeeper.ts);

        /// <summary>
        /// public method that indicates whether the desired interval has passed
        /// </summary>
        /// <param name="hourToRun"></param>
        /// <returns></returns>
        public bool DayPassed()
        {
            bool dayPassed = false;
            // check if it is time to run the report
            DateTime tNow = DateTime.Now;
            TimeSpan t = tNow.Subtract(rDailyTime);
            if (t.TotalHours > 24)
            {
                dayPassed = true;
                // setup to run at the same time tomorrow
                TimeKeeper.rDailyTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, m_reportDayTime, 0, 0);
            }
            else
            {
                dayPassed = false;
            } // not a 24 hour interval

            return dayPassed;
        }

    }
}
