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
    public partial class SearchUserPage : PhoneApplicationPage
    {
        private string keyword;
        private FanfouWP.API.Items.User user;
        private dynamic keyword_list;

        private ToastUtil toast = new ToastUtil();

        public SearchUserPage()
        {
            InitializeComponent();

            if (PhoneApplicationService.Current.State.ContainsKey("SearchUserPage"))
            {
                user = PhoneApplicationService.Current.State["SearchUserPage"] as FanfouWP.API.Items.User;
            }

            this.Loaded += SearchPage_Loaded;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (e.NavigationMode != NavigationMode.Back)
            {
                this.keyword = this.SearchText.Text;

                State["SearchUserPage_keyword"] = this.keyword;
                State["SearchUserPage_user"] = this.user;
                State["SearchUserPage_keyword_list"] = this.keyword_list;
            }

            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (State.ContainsKey("SearchUserPage_keyword"))
                this.keyword = State["SearchUserPage_keyword"] as string;
            if (State.ContainsKey("SearchUserPage_user"))
                this.user = State["SearchUserPage_user"] as User;
            if (State.ContainsKey("SearchUserPage_keyword_list"))
                this.keyword_list = State["SearchUserPage_keyword_list"];
    
            this.title.Text = "搜索" + this.user.screen_name + " 可用\"|\"分割多个关键字";

            base.OnNavigatedTo(e);
        }

        void SearchPage_Loaded(object sender, RoutedEventArgs e)
        {
            FanfouWP.API.FanfouAPI.Instance.SearchUserTimelineSuccess += Instance_SearchUserTimelineSuccess;
            FanfouWP.API.FanfouAPI.Instance.SearchUserTimelineFailed += Instance_SearchUserTimelineFailed;

            Dispatcher.BeginInvoke(() =>
            {
               
                if (keyword != null && keyword != "")
                {
                    this.SearchText.Text = keyword;

                    if (keyword_list != null)
                    {
                        this.SearchStatusListBox.ItemsSource = this.keyword_list;
                    }
                    else
                    {
                        this.loading.Visibility = System.Windows.Visibility.Visible;
                        (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = false;
                        FanfouWP.API.FanfouAPI.Instance.SearchUserTimeline(this.SearchText.Text, this.user.id);
                    }
                }
            });

        }

        void Instance_SearchUserTimelineFailed(object sender, FailedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
                (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = true;
                this.toast.NewToast("时间线搜索失败:( " + e.error.error);
            });
        }

        void Instance_SearchUserTimelineSuccess(object sender, UserTimelineEventArgs<Status> e)
        {

            Dispatcher.BeginInvoke(() =>
            {
                this.keyword_list = e.UserStatus;
                this.SearchStatusListBox.ItemsSource = keyword_list;
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
                (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = true;
            });
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            if ((SearchText.Text.Count() != 0))
            {
                Dispatcher.BeginInvoke(() =>
                {
                    this.loading.Visibility = System.Windows.Visibility.Visible;
                    (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = false;
                    this.Focus();
                });
                FanfouWP.API.FanfouAPI.Instance.SearchUserTimeline(this.SearchText.Text, this.user.id);
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
    }
}