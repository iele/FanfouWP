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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using FanfouWP.Storage;
using System.Collections.ObjectModel;

namespace FanfouWP
{
    public partial class SearchPage : PhoneApplicationPage
    {
        private string keyword, keyword_user;
        private ObservableCollection<Status> keyword_list;
        private ObservableCollection<User> keyword_user_list;
        private int currentIndex = -1;
        private bool SearchTimelineEnd = false;
        private bool SearchUserEnd = false;
        private int SearchUserCount = 1;

        private ToastUtil toast = new ToastUtil();

        public SearchPage()
        {
            InitializeComponent();

            if (PhoneApplicationService.Current.State.ContainsKey("SearchPage"))
            {
                keyword = (PhoneApplicationService.Current.State["SearchPage"] as FanfouWP.API.Items.Trends).query;
            }

            if (PhoneApplicationService.Current.State.ContainsKey("SearchPage_User"))
            {
                keyword_user = (PhoneApplicationService.Current.State["SearchPage_User"] as FanfouWP.API.Items.Trends).query;
            }

            FanfouWP.API.FanfouAPI.Instance.SearchTimelineSuccess += Instance_SearchTimelineSuccess;
            FanfouWP.API.FanfouAPI.Instance.SearchTimelineFailed += Instance_SearchTimelineFailed;
            FanfouWP.API.FanfouAPI.Instance.SearchUserSuccess += Instance_SearchUserSuccess;
            FanfouWP.API.FanfouAPI.Instance.SearchUserFailed += Instance_SearchUserFailed;
            FanfouWP.API.FanfouAPI.Instance.SavedSearchListSuccess += Instance_SavedSearchListSuccess;
            FanfouWP.API.FanfouAPI.Instance.SavedSearchListFailed += Instance_SavedSearchListFailed;

            this.Loaded += SearchPage_Loaded;
        }

        void Instance_SavedSearchListFailed(object sender, FailedEventArgs e)
        {
        }

        void Instance_SavedSearchListSuccess(object sender, ListEventArgs<FanfouWP.API.Items.Search> e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.SearchText.FilterMode = AutoCompleteFilterMode.None;
                this.SearchText.MinimumPrefixLength = 0;
                this.SearchText.ItemsSource = from s in FanfouWP.API.FanfouAPI.Instance.SavedSearch as List<FanfouWP.API.Items.Search> select s.query;
            });
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (e.NavigationMode != NavigationMode.Back)
            {
                this.keyword = this.SearchText.Text;
                this.keyword_user = this.UserSearchText.Text;

                State["SearchPage_keyword"] = this.keyword;
                State["SearchPage_keyword_user"] = this.keyword_user;
                State["SearchPage_currentIndex"] = this.Pivot.SelectedIndex;
                State["SearchUserPage_keyword_list"] = this.keyword_list;
                State["SearchUserPage_keyword_user_list"] = this.keyword_user_list;
                State["SearchPage_SearchTimelineEnd"] = this.SearchTimelineEnd;
                State["SearchPage_SearchUserEnd"] = this.SearchUserEnd;
                State["SearchPage_SearchUserCount"] = this.SearchUserCount;
            }

            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (State.ContainsKey("SearchPage_keyword"))
                this.keyword = State["SearchPage_keyword"] as string;
            if (State.ContainsKey("SearchPage_keyword_user"))
                this.keyword_user = State["SearchPage_keyword_user"] as string;
            if (State.ContainsKey("SearchPage_currentIndex"))
            {
                this.currentIndex = (int)State["SearchPage_currentIndex"];
                this.Pivot.SelectedIndex = currentIndex; base.OnNavigatedTo(e);
            }
            if (State.ContainsKey("SearchPage_keyword_list"))
                this.keyword_list = State["SearchPage_keyword_list"] as ObservableCollection<Status>;
            if (State.ContainsKey("SearchPage_keyword_user_list"))
                this.keyword_user_list = State["SearchPage_keyword_user_list"] as ObservableCollection<User>;
            if (State.ContainsKey("SearchPage_SearchTimelineEnd"))
                this.SearchTimelineEnd = (bool)State["SearchPage_SearchTimelineEnd"];
            if (State.ContainsKey("SearchPage_SearchUserEnd"))
                this.SearchUserEnd = (bool)State["SearchPage_SearchUserEnd"];
            if (State.ContainsKey("SearchPage_SearchUserCount"))
                this.SearchUserCount = (int)State["SearchPage_SearchUserCount"];

            if (FanfouWP.API.FanfouAPI.Instance.SavedSearch == null)
                FanfouWP.API.FanfouAPI.Instance.SavedSearchList();
            else
            {
                this.SearchText.FilterMode = AutoCompleteFilterMode.None;
                this.SearchText.MinimumPrefixLength = 0;
                this.SearchText.ItemsSource = from s in FanfouWP.API.FanfouAPI.Instance.SavedSearch as List<FanfouWP.API.Items.Search> select s.query;
            }
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
                        FanfouWP.API.FanfouAPI.Instance.SearchTimeline(this.SearchText.Text);
                    }
                    this.Pivot.SelectedIndex = 0;
                }

                if (keyword_user != null && keyword_user != "")
                {
                    this.UserSearchText.Text = keyword_user;
                    if (keyword_user_list == null)
                    {
                        this.loading.Visibility = System.Windows.Visibility.Visible;
                        (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = false;
                        FanfouWP.API.FanfouAPI.Instance.SearchUser(this.UserSearchText.Text);
                    }
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
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
                (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = true;

                if (e.UserStatus.Count == 0)
                {
                    SearchUserEnd = true;
                    return;
                }

                SearchUserCount++;

                if (keyword_user_list == null)
                    this.keyword_user_list = e.UserStatus;
                else
                {
                    foreach (var item in e.UserStatus)
                        keyword_user_list.Add(item);
                }
                if (this.UserStatusListBox.ItemsSource == null)
                    this.UserStatusListBox.ItemsSource = keyword_user_list;

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
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
                (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = true;

                if (e.UserStatus.Count == 0)
                {
                    SearchTimelineEnd = true;
                    return;
                }

                if (keyword_list == null)
                    this.keyword_list = e.UserStatus;
                else
                {
                    foreach (var item in e.UserStatus)
                        keyword_list.Add(item);
                }
                if (this.SearchStatusListBox.ItemsSource == null)
                    this.SearchStatusListBox.ItemsSource = keyword_list;
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
                    this.Focus();
                });
                if (this.Pivot.SelectedIndex == 0)
                {
                    keyword = this.SearchText.Text;
                    keyword_list = null;
                    this.SearchStatusListBox.ItemsSource = null;
                    SearchTimelineEnd = false;
                    FanfouWP.API.FanfouAPI.Instance.SearchTimeline(keyword);
                }
                else if (this.Pivot.SelectedIndex == 1)
                {
                    keyword_user = this.UserSearchText.Text;
                    keyword_user_list = null;
                    this.UserStatusListBox.ItemsSource = null;
                    SearchUserEnd = false;
                    SearchUserCount = 0;
                    FanfouWP.API.FanfouAPI.Instance.SearchUser(this.UserSearchText.Text);
                }
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

        private void UserStatusListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.UserStatusListBox.SelectedItem != null)
            {
                var item = this.UserStatusListBox.SelectedItem;
                this.UserStatusListBox.SelectedItem = null;

                if (PhoneApplicationService.Current.State.ContainsKey("UserPage"))
                {
                    PhoneApplicationService.Current.State.Remove("UserPage");
                }
                PhoneApplicationService.Current.State.Add("UserPage", item);
                NavigationService.Navigate(new Uri("/UserPage.xaml", UriKind.Relative));
            }
        }

        private void SearchText_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.SearchText.Text = SearchText.SelectedItem as string;
        }

        private void SearchStatusListBox_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            if (e.Container.DataContext == this.SearchStatusListBox.ItemsSource[this.SearchStatusListBox.ItemsSource.Count - 1] && !SearchTimelineEnd)
            {
                this.loading.Visibility = System.Windows.Visibility.Visible;
                FanfouWP.API.FanfouAPI.Instance.SearchTimeline(keyword, 60, (e.Container.DataContext as Status).id);
            }
        }

        private void UserStatusListBox_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            if (e.Container.DataContext == this.UserStatusListBox.ItemsSource[this.UserStatusListBox.ItemsSource.Count - 1] && !SearchUserEnd)
            {
                this.loading.Visibility = System.Windows.Visibility.Visible;
                FanfouWP.API.FanfouAPI.Instance.SearchUser(keyword_user, 60, SearchUserCount);
            }

        }

    }
}