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
            if (PhoneApplicationService.Current.State.ContainsKey("UserPage"))
            {
                user = PhoneApplicationService.Current.State["UserPage"] as FanfouWP.API.Items.User;
                PhoneApplicationService.Current.State.Remove("UserPage");
            }
            this.Loaded += UserPage_Loaded;
            FanfouWP.API.FanfouAPI.Instance.UserTimelineSuccess += Instance_UserTimelineSuccess;
            FanfouWP.API.FanfouAPI.Instance.UserTimelineFailed += Instance_UserTimelineFailed;
            FanfouWP.API.FanfouAPI.Instance.FavoritesSuccess += Instance_FavoritesSuccess;
            FanfouWP.API.FanfouAPI.Instance.FavoritesFailed += Instance_FavoritesFailed;
            FanfouWP.API.FanfouAPI.Instance.UsersFriendsSuccess += Instance_UsersFriendsSuccess;
            FanfouWP.API.FanfouAPI.Instance.UsersFriendsFailed += Instance_UsersFriendsFailed;
            FanfouWP.API.FanfouAPI.Instance.UsersFollowersSuccess += Instance_UsersFollowersSuccess;
            FanfouWP.API.FanfouAPI.Instance.UsersFollowersFailed += Instance_UsersFollowersFailed;

            FanfouWP.API.FanfouAPI.Instance.FriendshipsCreateSuccess += Instance_FriendshipsCreateSuccess;
            FanfouWP.API.FanfouAPI.Instance.FriendshipsCreateFailed += Instance_FriendshipsCreateFailed;
            FanfouWP.API.FanfouAPI.Instance.FriendshipsDestroySuccess += Instance_FriendshipsDestroySuccess;
            FanfouWP.API.FanfouAPI.Instance.FriendshipsDestroyFailed += Instance_FriendshipsDestroyFailed;
        }

        void Instance_FriendshipsDestroyFailed(object sender, API.Event.FailedEventArgs e)
        {
        }

        void Instance_FriendshipsDestroySuccess(object sender, EventArgs e)
        {
            user = sender as FanfouWP.API.Items.User;
            checkMenu();
        }

        void Instance_FriendshipsCreateFailed(object sender, API.Event.FailedEventArgs e)
        {
      }

        void Instance_FriendshipsCreateSuccess(object sender, EventArgs e)
        {
            user = sender as FanfouWP.API.Items.User;
            checkMenu();
        }

        void Instance_UsersFollowersFailed(object sender, API.Event.FailedEventArgs e)
        {
        }

        void Instance_UsersFollowersSuccess(object sender, API.Event.UserTimelineEventArgs<API.Items.User> e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.FollowersListBox.ItemsSource = (e as API.Event.UserTimelineEventArgs<FanfouWP.API.Items.User>).UserStatus;
            });
        }

        void Instance_UsersFriendsFailed(object sender, API.Event.FailedEventArgs e)
        {
        }

        void Instance_UsersFriendsSuccess(object sender, API.Event.UserTimelineEventArgs<API.Items.User> e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.FriendsListBox.ItemsSource = (e as API.Event.UserTimelineEventArgs<FanfouWP.API.Items.User>).UserStatus;
            });
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
            FanfouWP.API.FanfouAPI.Instance.UsersFriends(this.user.id);
            FanfouWP.API.FanfouAPI.Instance.UsersFollowers(this.user.id);
            FanfouWP.API.FanfouAPI.Instance.FavoritesId(this.user.id);

            checkMenu();
        }

        protected void checkMenu()
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (user.following)
                    (this.ApplicationBar.MenuItems[0] as ApplicationBarMenuItem).Text = "解除好友";
                else
                    (this.ApplicationBar.MenuItems[0] as ApplicationBarMenuItem).Text = "添加好友";
            });
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
            this.pivot.SelectedIndex = 2;
        }

        private void StackPanel_Tap_3(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.pivot.SelectedIndex = 3;
        }
        private void StackPanel_Tap_4(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.pivot.SelectedIndex = 4;
        }

        private void FavListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.FavListBox.SelectedItem != null)
            {
                var item = this.FavListBox.SelectedItem;

                if (PhoneApplicationService.Current.State.ContainsKey("StatusPage"))
                {
                    PhoneApplicationService.Current.State.Remove("StatusPage");
                }
                PhoneApplicationService.Current.State.Add("StatusPage", item);
                NavigationService.Navigate(new Uri("/StatusPage.xaml", UriKind.Relative));
            }
        }

        private void FollowersListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.FollowersListBox.SelectedItem != null)
            {
                var item = this.FollowersListBox.SelectedItem;

                if (PhoneApplicationService.Current.State.ContainsKey("UserPage"))
                {
                    PhoneApplicationService.Current.State.Remove("UserPage");
                }
                PhoneApplicationService.Current.State.Add("UserPage", item);
                App.RootFrame.Navigate(new Uri("/UserPage.xaml", UriKind.Relative));
            }
        }

        private void FriendsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.FriendsListBox.SelectedItem != null)
            {
                var item = this.FriendsListBox.SelectedItem;

                if (PhoneApplicationService.Current.State.ContainsKey("UserPage"))
                {
                    PhoneApplicationService.Current.State.Remove("UserPage");
                }
                PhoneApplicationService.Current.State.Add("UserPage", item);
                App.RootFrame.Navigate(new Uri("/UserPage.xaml", UriKind.Relative));
            }
        }

        private void FriendMenu_Click(object sender, EventArgs e)
        {
            if (user.following)
                FanfouWP.API.FanfouAPI.Instance.FriendshipDestroy(user.id);
            else
                FanfouWP.API.FanfouAPI.Instance.FriendshipCreate(user.id);
        }

    }
}