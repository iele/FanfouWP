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
    public partial class RequestPage : PhoneApplicationPage
    {
        private int currentPage = 1;

        private ToastUtil toast = new ToastUtil();

        private ObservableCollection<User> list;
        public RequestPage()
        {
            InitializeComponent();


            this.Loaded += RequestPage_Loaded;
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (e.NavigationMode != NavigationMode.Back)
            {
                State["RequestPage_currentPage"] = this.currentPage;
                State["RequestPage_list"] = this.list;
            }

            base.OnNavigatedFrom(e);
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
            if (list != null) {
                this.RequestStatusListBox.ItemsSource = list;
            }
            FanfouWP.API.FanfouAPI.Instance.FriendshipRequests(currentPage);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (State.ContainsKey("RequestPage_currentPage"))
            {
                this.currentPage = (int)State["RequestPage_currentPage"];
                this.page.Text = "第" + currentPage.ToString() + "页";
            }
            if (State.ContainsKey("RequestPage_list"))
            {
                this.list = State["RequestPage_list"] as ObservableCollection<User>;
                Dispatcher.BeginInvoke(() =>
                {
                    this.loading.Visibility = System.Windows.Visibility.Collapsed;
                });
            }
        
            if (PhoneApplicationService.Current.State.ContainsKey("RequestPage"))
            {
                var user = PhoneApplicationService.Current.State["RequestPage"] as User;

                try
                {
                    var item = from i in list where user.id == i.id select i;
                    list.Remove(item.First());

                    this.RequestStatusListBox.ItemsSource = list;
                }
                catch (Exception exception)
                {
                    System.Diagnostics.Debug.WriteLine(exception.Message);
                }

                PhoneApplicationService.Current.State.Remove("RequestPage");
            }
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
                    list = (e as UserTimelineEventArgs<User>).UserStatus;
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
                this.RequestStatusListBox.SelectedItem = null;

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