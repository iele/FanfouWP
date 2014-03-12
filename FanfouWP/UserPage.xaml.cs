using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Collections.ObjectModel;

namespace FanfouWP
{
    public partial class UserPage : PhoneApplicationPage
    {
        private API.Items.User user;
        private ObservableCollection<FanfouWP.API.Items.Status> status = new ObservableCollection<API.Items.Status>();
        public UserPage()
        {
            InitializeComponent();
            if (PhoneApplicationService.Current.State.ContainsKey("User"))
            {
                user = PhoneApplicationService.Current.State["User"] as FanfouWP.API.Items.User;
                PhoneApplicationService.Current.State.Remove("User");
            }
            this.Loaded += UserPage_Loaded;
            FanfouWP.API.FanfouAPI.Instance.UserTimelineSuccess += Instance_UserTimelineSuccess;
            FanfouWP.API.FanfouAPI.Instance.UserTimelineFailed += Instance_UserTimelineFailed;
        }

        private void Instance_UserTimelineSuccess(object sender, API.Event.UserTimelineEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                status = e.UserStatus; 
                this.TimeLineListBox.ItemsSource = status;
                this.FirstStatusText.Text = status.First().text;
            });
        }

        void Instance_UserTimelineFailed(object sender, API.Event.FailedEventArgs e)
        {
        }

        void UserPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = user;
            FanfouWP.API.FanfouAPI.Instance.StatusUserTimeline(this.user.id);
        }

        private void TimeLineListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.TimeLineListBox.SelectedItem != null)
            {
                var item = this.TimeLineListBox.SelectedItem;

                if (PhoneApplicationService.Current.State.ContainsKey("StatusPage"))
                {
                    PhoneApplicationService.Current.State.Remove("StatusPage");
                }
                PhoneApplicationService.Current.State.Add("StatusPage", item);
                NavigationService.Navigate(new Uri("/StatusPage.xaml", UriKind.Relative));
            }
        }

    }
}