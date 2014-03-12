using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace FanfouWP
{
    public partial class StatusPage : PhoneApplicationPage
    {
        private FanfouWP.API.Items.Status status;
        public StatusPage()
        {
            InitializeComponent();

            if (PhoneApplicationService.Current.State.ContainsKey("StatusPage"))
            {
                status = PhoneApplicationService.Current.State["StatusPage"] as FanfouWP.API.Items.Status;
                PhoneApplicationService.Current.State.Remove("StatusPage");
            }

            Loaded += StatusPage_Loaded;
        }

        private void StatusPage_Loaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.DataContext = status;
            });
        }

        private void UserButton_Click(object sender, EventArgs e)
        {
            if (PhoneApplicationService.Current.State.ContainsKey("User"))
            {
                PhoneApplicationService.Current.State.Remove("User");
            }
            PhoneApplicationService.Current.State.Add("User", status.user);
            NavigationService.Navigate(new Uri("/UserPage.xaml", UriKind.Relative));
        }

        private void ResendButton_Click(object sender, EventArgs e)
        {
            if (PhoneApplicationService.Current.State.ContainsKey("ReSend"))
            {
                PhoneApplicationService.Current.State.Remove("ReSend");
            }
            PhoneApplicationService.Current.State.Add("ReSend", status);
            NavigationService.Navigate(new Uri("/SendPage.xaml", UriKind.Relative));
        }

        private void FavButton_Click(object sender, EventArgs e)
        {

        }

        private void ReplyButton_Click(object sender, EventArgs e)
        {
            if (PhoneApplicationService.Current.State.ContainsKey("Reply"))
            {
                PhoneApplicationService.Current.State.Remove("Reply");
            }
            PhoneApplicationService.Current.State.Add("Reply", status);
            NavigationService.Navigate(new Uri("/SendPage.xaml", UriKind.Relative));
        }
    }
}