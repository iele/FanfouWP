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
    public partial class FavStatusPage : PhoneApplicationPage
    {
        private API.Items.User user;

        private int currentPage = 1;
        public FavStatusPage()
        {
            InitializeComponent();

            if (PhoneApplicationService.Current.State.ContainsKey("FavStatusPage"))
            {
                user = PhoneApplicationService.Current.State["FavStatusPage"] as FanfouWP.API.Items.User;
                PhoneApplicationService.Current.State.Remove("FavStatusPage");
            }
            this.Loaded += FavStatusPage_Loaded;
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
            FanfouWP.API.FanfouAPI.Instance.FavoritesId(user.id, currentPage);
            FanfouWP.API.FanfouAPI.Instance.FavoritesSuccess += Instance_FavoritesSuccess;
            FanfouWP.API.FanfouAPI.Instance.FavoritesFailed += Instance_FavoritesFailed;
        }

        void Instance_FavoritesFailed(object sender, API.Event.FailedEventArgs e)
        {
        }

        void Instance_FavoritesSuccess(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if ((e as UserTimelineEventArgs<Status>).UserStatus.Count != 0)
                {
                    this.FavStatusListBox.ItemsSource = (e as UserTimelineEventArgs<Status>).UserStatus;
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
            if (currentPage >= 1)
            {
                currentPage--;
                FanfouWP.API.FanfouAPI.Instance.FavoritesId(user.id, currentPage);
                changeMenu(false, true);
            }
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            currentPage++;
            FanfouWP.API.FanfouAPI.Instance.FavoritesId(user.id, currentPage);
            changeMenu(false, true);
        }


    }
}