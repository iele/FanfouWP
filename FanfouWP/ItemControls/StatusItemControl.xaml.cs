using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Input;

namespace FanfouWP.ItemControls
{
    public partial class StatusItemControl : UserControl
    {
        public StatusItemControl()
        {
            InitializeComponent();

            this.Loaded += StatusItemControl_Loaded;
        }

        void StatusItemControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Tap_LeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            StackPanel rect = sender as StackPanel;
            // Change the size of the Rectangle.
            if (null != rect)
            {
                rect.Opacity = 0.5;


            }
        }
        private void Tap_LeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            StackPanel rect = sender as StackPanel;
            // Reset the dimensions on the Rectangle.
            if (null != rect)
            {
                rect.Opacity = 1.0;

            }
        }
        private void Tap_MouseLeave(object sender, MouseEventArgs e)
        {
            StackPanel rect = sender as StackPanel;
            // Finger moved out of Rectangle before the mouse up event.
            // Reset the dimensions on the Rectangle.


            if (null != rect)
            {
                rect.Opacity = 1.0;

            }
        }

    }
}
