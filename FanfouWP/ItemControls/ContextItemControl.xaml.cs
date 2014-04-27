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

namespace FanfouWP.ItemControls
{
    public partial class ContextItemControl : UserControl
    {
        public ContextItemControl()
        {
            InitializeComponent();

            this.Loaded += StatusItemControl_Loaded;
        }

        void StatusItemControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem1_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (PhoneApplicationService.Current.State.ContainsKey("Reply"))
            {
                PhoneApplicationService.Current.State.Remove("Reply");
            }
            PhoneApplicationService.Current.State.Add("Reply", this.DataContext);
            App.RootFrame.Navigate(new Uri("/SendPage.xaml", UriKind.Relative));
        }

        private void MenuItem2_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (PhoneApplicationService.Current.State.ContainsKey("ReSend"))
            {
                PhoneApplicationService.Current.State.Remove("ReSend");
            }
            PhoneApplicationService.Current.State.Add("ReSend", this.DataContext);
            App.RootFrame.Navigate(new Uri("/SendPage.xaml", UriKind.Relative));
        }
    }
}
