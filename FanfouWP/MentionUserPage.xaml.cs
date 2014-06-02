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
    public partial class MentionUserPage : PhoneApplicationPage
    {
        private string keyword;
        private dynamic keyword_list;

        private Toast toast = new Toast();

        public MentionUserPage()
        {
            InitializeComponent();

            this.Loaded += MentionUserPage_Loaded;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (e.NavigationMode != NavigationMode.Back)
            {
                this.keyword = this.SearchText.Text;

                State["MentionUserPage_keyword"] = this.keyword;
                State["MentionUserPagee_keyword_list"] = this.keyword_list;
            }

            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (State.ContainsKey("MentionUserPage_keyword"))
                this.keyword = State["MentionUserPage_keyword"] as string;
            if (State.ContainsKey("MentionUserPage_keyword_list"))
                this.keyword_list = State["MentionUserPage_keyword_list"];
    
            base.OnNavigatedTo(e);
        }

        void MentionUserPage_Loaded(object sender, RoutedEventArgs e)
        {
            FanfouWP.API.FanfouAPI.Instance.SearchUserSuccess += Instance_SearchUserSuccess;
            FanfouWP.API.FanfouAPI.Instance.SearchUserFailed += Instance_SearchUserFailed;

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
                        FanfouWP.API.FanfouAPI.Instance.SearchUser(keyword);
                    }
                }
            });

        }

        void Instance_SearchUserFailed(object sender, FailedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
                (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = true;
                this.toast.NewToast("时间线搜索失败:( " + e.error.error);
            });
        }

        void Instance_SearchUserSuccess(object sender, UserTimelineEventArgs<User> e)
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
                FanfouWP.API.FanfouAPI.Instance.SearchUser(this.SearchText.Text);
            }
        }

        private void SearchStatusListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.SearchStatusListBox.SelectedItem != null)
            {
                var item = this.SearchStatusListBox.SelectedItem;
                this.SearchStatusListBox.SelectedItem = null;

                if (PhoneApplicationService.Current.State.ContainsKey("SendPage_Friend"))
                {
                    PhoneApplicationService.Current.State.Remove("SendPage_Friend");
                }
                PhoneApplicationService.Current.State.Add("SendPage_Friend", item);
                NavigationService.GoBack();

            }
        }
    }
}