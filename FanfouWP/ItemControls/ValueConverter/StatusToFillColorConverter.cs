using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace FanfouWP.ItemControls.ValueConverter
{
    public sealed class StatusToFillColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo language)
        {
            return new SolidColorBrush(Parse(value as string));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo language)
        {
            return null;
        }

        private static Color Parse(string color)
        {
            try
            {

                var offset = color.StartsWith("#") ? 1 : 0;

                byte a = 0xff;
                var r = Byte.Parse(color.Substring(0 + offset, 2), NumberStyles.HexNumber);
                var g = Byte.Parse(color.Substring(2 + offset, 2), NumberStyles.HexNumber);
                var b = Byte.Parse(color.Substring(4 + offset, 2), NumberStyles.HexNumber);
                return Color.FromArgb(a, r, g, b);
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine(exception.Message);
                return Color.FromArgb(0xff, 0x88, 0x88, 0x88);
            }
        }
    }
}
