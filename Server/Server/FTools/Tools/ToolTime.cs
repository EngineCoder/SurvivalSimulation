using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTools.Tools
{
    public static partial class Tool
    {
        /// <summary>
        /// Convert seconds to hours, minutes and seconds
        /// </summary>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static string SecondsToHMS(long duration)
        {
            TimeSpan ts = new TimeSpan(0, 0, Convert.ToInt32(duration));
            string str = "";
            if (ts.Hours > 0 && ts.Hours < 10)
            {
                str = String.Format("{0:0}", ts.Hours) + ":" + String.Format("{0:00}", ts.Minutes) + ":" + String.Format("{0:00}", ts.Seconds);
            }
            else if (ts.Hours >= 10)
            {
                str = String.Format("{0:00}", ts.Hours) + ":" + String.Format("{0:00}", ts.Minutes) + ":" + String.Format("{0:00}", ts.Seconds);
            }
            else if (ts.Minutes > 0 && ts.Minutes < 10)
            {
                str = String.Format("{0:0}", ts.Minutes) + ":" + String.Format("{0:00}", ts.Seconds);
            }
            else if (ts.Minutes >= 10)
            {
                str = String.Format("{0:00}", ts.Minutes) + ":" + String.Format("{0:00}", ts.Seconds);
            }
            else
            {
                str = "0:" + String.Format("{0:00}", ts.Seconds);
            }
            return str;
        }
    }
}
