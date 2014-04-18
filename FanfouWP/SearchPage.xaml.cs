﻿using System;
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

namespace FanfouWP
{
    public partial class SearchPage : PhoneApplicationPage
    {
        private string keyword, keyword_user;

        public SearchPage()
        {
            InitializeComponent();

            if (PhoneApplicationService.Current.State.ContainsKey("SearchPage"))
            {
                keyword = (PhoneApplicationService.Current.State["SearchPage"] as FanfouWP.API.Items.Trends).query;
                PhoneApplicationService.Current.State.Remove("SearchPage");
            }

            if (PhoneApplicationService.Current.State.ContainsKey("SearchPage_User"))
            {
                keyword_user = (PhoneApplicationService.Current.State["SearchPage_User"] as FanfouWP.API.Items.Trends).query;
                PhoneApplicationService.Current.State.Remove("SearchPage_User");
            }

            this.Loaded += SearchPage_Loaded;
        }

        void SearchPage_Loaded(object sender, RoutedEventArgs e)
        {
            FanfouWP.API.FanfouAPI.Instance.SearchTimelineSuccess += Instance_SearchTimelineSuccess;
            FanfouWP.API.FanfouAPI.Instance.SearchTimelineFailed += Instance_SearchTimelineFailed;
            FanfouWP.API.FanfouAPI.Instance.SearchUserSuccess += Instance_SearchUserSuccess;
            FanfouWP.API.FanfouAPI.Instance.SearchUserFailed += Instance_SearchUserFailed;

            Dispatcher.BeginInvoke(() =>
            {
                if (keyword != null)
                {
                    this.SearchText.Text = keyword;
                    this.loading.Visibility = System.Windows.Visibility.Visible;
                    (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = false;
                    FanfouWP.API.FanfouAPI.Instance.SearchTimeline(this.SearchText.Text);
                    this.Pivot.SelectedIndex = 0;
                }

                if (keyword_user != null)
                {
                    this.UserSearchText.Text = keyword_user;
                    this.loading.Visibility = System.Windows.Visibility.Visible;
                    (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = false;
                    FanfouWP.API.FanfouAPI.Instance.SearchUser(this.UserSearchText.Text);
                    this.Pivot.SelectedIndex = 1;
                }
            });

        }

        void Instance_SearchUserFailed(object sender, FailedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
                (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = true;
                this.toast.NewToast("用户搜索失败:( " + e.error.error);
            });
        }

        void Instance_SearchUserSuccess(object sender, UserTimelineEventArgs<User> e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.UserStatusListBox.ItemsSource = e.UserStatus;
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
                (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = true;
            });
        }

        void Instance_SearchTimelineFailed(object sender, FailedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
                (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = true;
                this.toast.NewToast("时间线搜索失败:( " + e.error.error);
            });
        }

        void Instance_SearchTimelineSuccess(object sender, UserTimelineEventArgs<Status> e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.SearchStatusListBox.ItemsSource = e.UserStatus;
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
                (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = true;
            });
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            if ((SearchText.Text.Count() != 0 && this.Pivot.SelectedIndex == 0) || (UserSearchText.Text.Count() != 0 && this.Pivot.SelectedIndex == 1))
            {
                Dispatcher.BeginInvoke(() =>
                {
                    this.loading.Visibility = System.Windows.Visibility.Visible;
                    (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = false;
                });
                if (this.Pivot.SelectedIndex == 0)
                    FanfouWP.API.FanfouAPI.Instance.SearchTimeline(this.SearchText.Text);
                else if (this.Pivot.SelectedIndex == 1)
                    FanfouWP.API.FanfouAPI.Instance.SearchUser(this.UserSearchText.Text);
            }
        }

        private void SearchStatusListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.SearchStatusListBox.SelectedItem != null)
            {
                var item = this.SearchStatusListBox.SelectedItem;
                this.SearchStatusListBox.SelectedIndex = -1;

                if (PhoneApplicationService.Current.State.ContainsKey("StatusPage"))
                {
                    PhoneApplicationService.Current.State.Remove("StatusPage");
                }
                PhoneApplicationService.Current.State.Add("StatusPage", item);
                NavigationService.Navigate(new Uri("/StatusPage.xaml", UriKind.Relative));
            }
        }

        private void UserStatusListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.UserStatusListBox.SelectedItem != null)
            {
                var item = this.UserStatusListBox.SelectedItem;
                this.UserStatusListBox.SelectedIndex = -1;

                if (PhoneApplicationService.Current.State.ContainsKey("UserPage"))
                {
                    PhoneApplicationService.Current.State.Remove("UserPage");
                }
                PhoneApplicationService.Current.State.Add("UserPage", item);
                NavigationService.Navigate(new Uri("/UserPage.xaml", UriKind.Relative));
            }
        }
    }
}