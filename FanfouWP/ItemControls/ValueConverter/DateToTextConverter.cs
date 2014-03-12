using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;

namespace FanfouWP.ItemControls.ValueConverter
{
    public sealed class DateToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo language)
        {
            CultureInfo cultureInfo = new CultureInfo("en-US");
            string format = "ddd MMM d HH:mm:ss zz00 yyyy";
            string stringValue = DateTime.Now.ToString(format, cultureInfo);
            DateTime datetime = DateTime.ParseExact(value as string, format, cultureInfo);
            DateTime currenttime = DateTime.Now;

            string dateDiff = "";
            try
            {
                TimeSpan ts1 = new TimeSpan(currenttime.Ticks);
                TimeSpan ts2 = new TimeSpan(datetime.Ticks);
                TimeSpan ts = ts1.Subtract(ts2).Duration();
                if (ts.Days != 0)
                    return ts.Days.ToString() + "天";
                if (ts.Hours!= 0)
                    return ts.Hours.ToString() + "小时";
                if (ts.Minutes != 0)
                    return ts.Minutes.ToString() + "分钟";
                if (ts.Seconds != 0)
                    return ts.Seconds.ToString() + "秒";
            }
            catch
            {

            }
            return dateDiff;   
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo language)
        {
            return null;
        }
    }
}
