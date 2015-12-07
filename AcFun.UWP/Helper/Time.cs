using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcFun.UWP.Helper
{
    public class Time
    {
        public static int getTimeSpan()
        {
            var startTime = new DateTime(1970, 1, 1);
            var time = (int)(DateTime.Now - startTime).TotalSeconds;
            return time;
        }

        public static int getTimeSpan(DateTime dateTime)
        {
            var startTime = new DateTime(1970, 1, 1);
            var time = (int)(dateTime - startTime).TotalSeconds;
            return time;
        }

        public static DateTime getTime(long timeStamp)
        {
            var dtStart = new DateTime(1970, 1, 1).ToLocalTime();
            var lTime = long.Parse(timeStamp.ToString() + "0000");
            var toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }
    }
}
