using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using FanfouWP.API.Items;
using FanfouWP.API.Event;
using System.Collections.ObjectModel;
using FanfouWP.Utils;

namespace FanfouWP
{
    public partial class FriendsPages : PhoneApplicationPage
    {
        private API.Items.User user;
        private dynamic list;
        private int currentPage = 1;

        private ToastUtil toast = new ToastUtil();

        public FriendsPages()
        {
            InitializeComponent();

            if (PhoneApplicationService.Current.State.ContainsKey("FriendsPage"))
            {
                user = PhoneApplicationService.Current.State["FriendsPage"] as FanfouWP.API.Items.User;
            }
            this.Loaded += FriendsPage_Loaded;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            State["FriendsPages_currentPage"] = this.currentPage;
            State["FriendsPages_user"] = this.user;
            State["FriendsPages_list"] = list;
            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (State.ContainsKey("FriendsPages_currentPage"))
            {
                this.currentPage = (int)State["FriendsPages_currentPage"];
                this.page.Text = "第" + currentPage.ToString() + "页";
            }
            if (State.ContainsKey("FriendsPages_currentPage"))
            {
                this.list = State["FriendsPages_currentPage"] as ObservableCollection<Status>;
                Dispatcher.BeginInvoke(() =>
                {
                    this.loading.Visibility = System.Windows.Visibility.Collapsed;
                });
            }
            if (State.ContainsKey("FriendsPages_user"))
                this.user = State["FriendsPages_user"] as API.Items.User;
            base.OnNavigatedTo(e);
        }

        private void changeMenu(bool is_end, bool is_disabled = false)
        {
            ApplicationBarIconButton ForeButton = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
            if (currentPage <= 1 || is_disabled)
                ForeButton.IsEnabled = false;
            else
                ForeButton.IsEnabled = true;

            ApplicationBarIconButton BackButton = (ApplicationBarIconButton)ApplicationBar.Buttons[1];
            if (is_end || is_disabled)
                BackButton.IsEnabled = false;
            else
                BackButton.IsEnabled = true;

        }

        void FriendsPage_Loaded(object sender, RoutedEventArgs e)
        {
            FanfouWP.API.FanfouAPI.Instance.UsersFriendsSuccess += Instance_UsersFriendsSuccess;
            FanfouWP.API.FanfouAPI.Instance.UsersFriendsFailed += Instance_UsersFriendsFailed;

            if (list != null)
            {
                this.FriendsStatusListBox.ItemsSource = list;
                return;
            }
            FanfouWP.API.FanfouAPI.Instance.UsersFriends(user.id, 60, currentPage);
        }


        void Instance_UsersFriendsFailed(object sender, API.Event.FailedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
                this.toast.NewToast("朋友列表获取失败:( " + e.error.error);
            });
        }

        void Instance_UsersFriendsSuccess(object sender, UserTimelineEventArgs<FanfouWP.API.Items.User> e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
                if (e.UserStatus.Count != 0)
                {
                    list = (e as UserTimelineEventArgs<User>).UserStatus;
                    this.FriendsStatusListBox.ItemsSource = list;
                    changeMenu(false);
                    this.page.Text = "第" + currentPage.ToString() + "页";
                }
                else
                {
                    changeMenu(true);
                }
            });
        }

        private void FriendsStatusListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.FriendsStatusListBox.SelectedItem != null)
            {
                var item = this.FriendsStatusListBox.SelectedItem;
                this.FriendsStatusListBox.SelectedIndex = -1;

                if (PhoneApplicationService.Current.State.ContainsKey("SendPage_Friend"))
                {
                    PhoneApplicationService.Current.State.Remove("SendPage_Friend");
                }
                PhoneApplicationService.Current.State.Add("SendPage_Friend", item);
                NavigationService.GoBack();
            }
        }

        private void ForeButton_Click(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.loading.Visibility = System.Windows.Visibility.Visible;
            });
            if (currentPage >= 1)
            {
                currentPage--;
                FanfouWP.API.FanfouAPI.Instance.UsersFriends(user.id, 60, currentPage);
                changeMenu(false, true);
            }
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.loading.Visibility = System.Windows.Visibility.Visible;
            });
            currentPage++;
            FanfouWP.API.FanfouAPI.Instance.UsersFriends(user.id, 60, currentPage);
            changeMenu(false, true);
        }


    }
}