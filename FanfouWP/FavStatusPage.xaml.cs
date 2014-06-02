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
using FanfouWP.Utils;

namespace FanfouWP
{
    public partial class FavStatusPage : PhoneApplicationPage
    {
        private API.Items.User user;

        private dynamic list;

        private int currentPage = 1;

        private Toast toast = new Toast();

        public FavStatusPage()
        {
            InitializeComponent();

            if (PhoneApplicationService.Current.State.ContainsKey("FavStatusPage"))
            {
                user = PhoneApplicationService.Current.State["FavStatusPage"] as FanfouWP.API.Items.User;
            }
            this.Loaded += FavStatusPage_Loaded;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (e.NavigationMode != NavigationMode.Back)
            {
                State["FavStatusPage_currentPage"] = this.currentPage;
                State["FavStatusPage_list"] = this.list;
            }

            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (State.ContainsKey("FavStatusPage_currentPage"))
            {
                this.currentPage = (int)State["FavStatusPage_currentPage"];
                this.page.Text = "第" + currentPage.ToString() + "页";
            }
            if (State.ContainsKey("FavStatusPage_list"))
            {
                this.list = State["FavStatusPage_list"];
                Dispatcher.BeginInvoke(() =>
                {
                    this.loading.Visibility = System.Windows.Visibility.Collapsed;
                });
            }


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

        void FavStatusPage_Loaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.title.Text = this.user.screen_name + "的收藏";

            });
            FanfouWP.API.FanfouAPI.Instance.FavoritesSuccess += Instance_FavoritesSuccess;
            FanfouWP.API.FanfouAPI.Instance.FavoritesFailed += Instance_FavoritesFailed;


            if (this.list != null)
            {
                this.FavStatusListBox.ItemsSource = list;
                return;
            }

            FanfouWP.API.FanfouAPI.Instance.FavoritesId(user.id, currentPage);
        }

        void Instance_FavoritesFailed(object sender, API.Event.FailedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
                this.toast.NewToast("收藏列表获取失败:( " + e.error.error);
            });
        }

        void Instance_FavoritesSuccess(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
                if ((e as UserTimelineEventArgs<Status>).UserStatus.Count != 0)
                {
                    list = (e as UserTimelineEventArgs<Status>).UserStatus;
                    this.FavStatusListBox.ItemsSource = list;
                    changeMenu(false);
                    this.page.Text = "第" + currentPage.ToString() + "页";
                }
                else
                {
                    changeMenu(true);
                }
            });
        }

        private void FavStatusListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.FavStatusListBox.SelectedItem != null)
            {
                var item = this.FavStatusListBox.SelectedItem;
                this.FavStatusListBox.SelectedItem = null;

                if (PhoneApplicationService.Current.State.ContainsKey("StatusPage"))
                {
                    PhoneApplicationService.Current.State.Remove("StatusPage");
                }
                PhoneApplicationService.Current.State.Add("StatusPage", item);
                NavigationService.Navigate(new Uri("/StatusPage.xaml", UriKind.Relative));
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
                FanfouWP.API.FanfouAPI.Instance.FavoritesId(user.id, currentPage);
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
            FanfouWP.API.FanfouAPI.Instance.FavoritesId(user.id, currentPage);
            changeMenu(false, true);
        }


    }
}