using System;
using System.Windows.Data;

namespace ACFUN
{
    public class TimeConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var str = value.ToString();
            var timespan = Int64.Parse(str);
            return TimeFuc.getTime(timespan).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
