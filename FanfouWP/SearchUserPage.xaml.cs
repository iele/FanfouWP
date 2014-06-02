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
using System.Collections.ObjectModel;

namespace FanfouWP
{
    public partial class SearchUserPage : PhoneApplicationPage
    {
        private string keyword;
        private FanfouWP.API.Items.User user;
        private ObservableCollection<Status> keyword_list;
        private bool SearchUserTimelineEnd = false;

        private Toast toast = new Toast();

        public SearchUserPage()
        {
            InitializeComponent();

            if (PhoneApplicationService.Current.State.ContainsKey("SearchUserPage"))
            {
                user = PhoneApplicationService.Current.State["SearchUserPage"] as FanfouWP.API.Items.User;
            }

            FanfouWP.API.FanfouAPI.Instance.SearchUserTimelineSuccess += Instance_SearchUserTimelineSuccess;
            FanfouWP.API.FanfouAPI.Instance.SearchUserTimelineFailed += Instance_SearchUserTimelineFailed;

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
                State["SearchUserPage_SearchUserTimelineEnd"] = this.SearchUserTimelineEnd;
            }

            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (State.ContainsKey("SearchUserPage_keyword"))
                this.keyword = State["SearchUserPage_keyword"] as string;
            if (State.ContainsKey("SearchUserPage_keyword_list"))
                this.keyword_list = State["SearchUserPage_keyword_list"] as ObservableCollection<Status>;
            if (State.ContainsKey("SearchUserPage_user"))
                this.user = State["SearchUserPage_user"] as User;
            if (State.ContainsKey("SearchUserPage_SearchUserTimelineEnd"))
                this.SearchUserTimelineEnd = (bool)State["SearchUserPage_SearchUserTimelineEnd"];

            this.title.Text = "搜索" + this.user.screen_name + " 可用\"|\"分割多个关键字";

            base.OnNavigatedTo(e);
        }

        void SearchPage_Loaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (keyword != null && keyword != "")
                {
                    this.SearchText.Text = keyword;

                    if (keyword_list == null)
                    {
                        this.loading.Visibility = System.Windows.Visibility.Visible;
                        (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = false;
                        FanfouWP.API.FanfouAPI.Instance.SearchUserTimeline(this.SearchText.Text, this.user.id);
                    }
                    else {
                        this.SearchStatusListBox.ItemsSource = keyword_list;
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
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
                (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = true;

                if (e.UserStatus.Count == 0)
                {
                    SearchUserTimelineEnd = true;
                    return;
                }

                if (keyword_list == null)
                    this.keyword_list = e.UserStatus;
                else
                {
                    foreach (var item in e.UserStatus)
                        this.keyword_list.Add(item);
                }
                if (this.SearchStatusListBox.ItemsSource == null)
                    this.SearchStatusListBox.ItemsSource = keyword_list;
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

                keyword = this.SearchText.Text;
                keyword_list = null;
                this.SearchStatusListBox.ItemsSource = null;
                SearchUserTimelineEnd = false;
                FanfouWP.API.FanfouAPI.Instance.SearchUserTimeline(this.SearchText.Text, this.user.id);
            }
        }

        private void SearchStatusListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.SearchStatusListBox.SelectedItem != null)
            {
                var item = this.SearchStatusListBox.SelectedItem;
                this.SearchStatusListBox.SelectedItem = null;

                if (PhoneApplicationService.Current.State.ContainsKey("StatusPage"))
                {
                    PhoneApplicationService.Current.State.Remove("StatusPage");
                }
                PhoneApplicationService.Current.State.Add("StatusPage", item);
                NavigationService.Navigate(new Uri("/StatusPage.xaml", UriKind.Relative));
            }
        }

        private void SearchStatusListBox_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            if (e.Container.DataContext == this.SearchStatusListBox.ItemsSource[this.SearchStatusListBox.ItemsSource.Count - 1] && !SearchUserTimelineEnd)
            {
                this.loading.Visibility = System.Windows.Visibility.Visible;
                FanfouWP.API.FanfouAPI.Instance.SearchUserTimeline(keyword, user.id, 60, (e.Container.DataContext as Status).id);
            }
        }
    }
}