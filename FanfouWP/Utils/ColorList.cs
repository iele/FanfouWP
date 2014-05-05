using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FanfouWP.Utils
{
    public class Color
    {
        public string color { get; set; }
        public Color(string color)
        {
            this.color = color;
        }
    }
    public class ColorList
    {
        public static List<Color> Colors
        {
            get
            {
                var l = new List<Color>();
                l.Add(new Color("#72CBE1"));
                var currentAccentColorHex = (System.Windows.Media.Color)Application.Current.Resources["PhoneAccentColor"];
                l.Add(new Color(currentAccentColorHex.ToString().Remove(1, 2)));
                l.Add(new Color("#66C5DC"));
                l.Add(new Color("#ffffe5"));
                l.Add(new Color("#fffcd9"));
                l.Add(new Color("#3a9dcf"));
                l.Add(new Color("#0094de"));
                l.Add(new Color("#004263"));
                l.Add(new Color("#5F7821"));
                l.Add(new Color("#942970"));
                return l;
            }
            private set { }
        }
    }
}
