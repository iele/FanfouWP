using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace FanfouWP.ItemControls.ValueConverter
{
    public sealed class SexToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo language)
        {
            if (value as string == "男")
            {
                return new BitmapImage(new Uri("/Assets/male.png", UriKind.Relative));
            }
            else if (value as string == "女")
            {
                return new BitmapImage(new Uri("/Assets/female.png", UriKind.Relative));
            }
            else
                return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo language)
        {
            return null;
        }
    }
}
