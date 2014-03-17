using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

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
            FanfouWP.API.FanfouAPI.Instance.FavoritesSuccess += Instance_FavoritesSuccess;
            FanfouWP.API.FanfouAPI.Instance.FavoritesFailed += Instance_FavoritesFailed;
        }

        private void Instance_FavoritesFailed(object sender, API.Event.FailedEventArgs e)
        {
        }

        private void Instance_FavoritesSuccess(object sender, EventArgs e)
        {
             Dispatcher.BeginInvoke(() =>
             {
                 this.FavListBox.ItemsSource = (e as API.Event.UserTimelineEventArgs<FanfouWP.API.Items.Status>).UserStatus;
             });
        }

        private void Instance_UserTimelineSuccess(object sender, API.Event.UserTimelineEventArgs<FanfouWP.API.Items.Status> e)
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
            FanfouWP.API.FanfouAPI.Instance.FavoritesId(this.user.id);
        }

        private void TimeLineListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
        }

        private void FavButton_Click(object sender, EventArgs e)
        {

            //if (PhoneApplicationService.Current.State.ContainsKey("FavStatusPage"))
            //{
            //    PhoneApplicationService.Current.State.Remove("FavStatusPage");
            //}
            //PhoneApplicationService.Current.State.Add("FavStatusPage", user);
            //NavigationService.Navigate(new Uri("/FavStatusPage.xaml", UriKind.Relative));
        }


        private void StackPanel_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.pivot.SelectedIndex = 1;
        }

        private void StackPanel_Tap_2(object sender, System.Windows.Input.GestureEventArgs e)
        {

        }

        private void StackPanel_Tap_3(object sender, System.Windows.Input.GestureEventArgs e)
        {

        }
        private void StackPanel_Tap_4(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.pivot.SelectedIndex = 2;
        }

        private void FavListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

    }
}