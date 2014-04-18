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

namespace FanfouWP
{
    public partial class RequestPage : PhoneApplicationPage
    {
        private int currentPage = 1;
        public RequestPage()
        {
            InitializeComponent();


            this.Loaded += RequestPage_Loaded;
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

        void RequestPage_Loaded(object sender, RoutedEventArgs e)
        {
            FanfouWP.API.FanfouAPI.Instance.FriendshipsRequestsSuccess += Instance_FriendshipsRequestsSuccess;
            FanfouWP.API.FanfouAPI.Instance.FriendshipsRequestsFailed += Instance_FriendshipsRequestsFailed;
            FanfouWP.API.FanfouAPI.Instance.FriendshipRequests(currentPage);
        }


        void Instance_FriendshipsRequestsFailed(object sender, API.Event.FailedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
                this.toast.NewToast("请求列表获取失败:( " + e.error.error);
            });
        }

        void Instance_FriendshipsRequestsSuccess(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
                if ((e as UserTimelineEventArgs<User>).UserStatus.Count != 0)
                {
                    this.RequestStatusListBox.ItemsSource = (e as UserTimelineEventArgs<User>).UserStatus;
                    changeMenu(false);
                    this.page.Text = "第" + currentPage.ToString() + "页";
                }
                else
                {
                    changeMenu(true);
                }
            });
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
                FanfouWP.API.FanfouAPI.Instance.FriendshipRequests(currentPage);
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
            FanfouWP.API.FanfouAPI.Instance.FriendshipRequests(currentPage);
            changeMenu(false, true);
        }

        private void RequestStatusListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.RequestStatusListBox.SelectedItem != null)
            {
                var item = this.RequestStatusListBox.SelectedItem;
                this.RequestStatusListBox.SelectedIndex = -1;

                if (PhoneApplicationService.Current.State.ContainsKey("FriendshipPage"))
                {
                    PhoneApplicationService.Current.State.Remove("FriendshipPage");
                }
                PhoneApplicationService.Current.State.Add("FriendshipPage", item);
                App.RootFrame.Navigate(new Uri("/FriendshipPage.xaml", UriKind.Relative));
            }
        }


    }
}