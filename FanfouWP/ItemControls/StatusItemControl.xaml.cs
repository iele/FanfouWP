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
using FanfouWP.API;
using FanfouWP.API.Items;

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
      
        private void MenuItem1_Click(object sender, RoutedEventArgs e)
        {
            if (PhoneApplicationService.Current.State.ContainsKey("Reply"))
            {
                PhoneApplicationService.Current.State.Remove("Reply");
            }
            PhoneApplicationService.Current.State.Add("Reply", this.DataContext);
            App.RootFrame.Navigate(new Uri("/SendPage.xaml", UriKind.Relative));
        }

        private void MenuItem2_Click(object sender, RoutedEventArgs e)
        {
            if (PhoneApplicationService.Current.State.ContainsKey("ReSend"))
            {
                PhoneApplicationService.Current.State.Remove("ReSend");
            }
            PhoneApplicationService.Current.State.Add("ReSend", this.DataContext);
            App.RootFrame.Navigate(new Uri("/SendPage.xaml", UriKind.Relative));
        }

        private void Rectangle_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (PhoneApplicationService.Current.State.ContainsKey("UserPage"))
            {
                PhoneApplicationService.Current.State.Remove("UserPage");
            }
            PhoneApplicationService.Current.State.Add("UserPage", (this.DataContext as Status).user);
            App.RootFrame.Navigate(new Uri("/UserPage.xaml", UriKind.Relative));
        }

        private void RectangleImage_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (PhoneApplicationService.Current.State.ContainsKey("ImagePage"))
            {
                PhoneApplicationService.Current.State.Remove("ImagePage");
            }
            PhoneApplicationService.Current.State.Add("ImagePage", this.DataContext);
            App.RootFrame.Navigate(new Uri("/ImagePage.xaml", UriKind.Relative));
        }
    }
}
