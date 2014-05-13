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
    public partial class ConversationsPage : PhoneApplicationPage
    {
        private int currentPage = 1;
        private dynamic list;

        private ToastUtil toast = new ToastUtil();

        public ConversationsPage()
        {
            InitializeComponent();

            this.Loaded += ConversationsPage_Loaded;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (e.NavigationMode != NavigationMode.Back)
            {
                State["ConversationsPage_currentPage"] = this.currentPage;
                State["ConversationsPage_list"] = this.list;
            }

            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (State.ContainsKey("ConversationsPage_currentPage"))
            {
                this.currentPage = (int)State["ConversationsPage_currentPage"];
                this.page.Text = "第" + currentPage.ToString() + "页";
            }
            if (State.ContainsKey("ConversationsPage_list"))
            {
                this.list = State["ConversationsPage_list"];
                Dispatcher.BeginInvoke(() =>
                {
                    this.loading.Visibility = System.Windows.Visibility.Collapsed;
                });
            }
            base.OnNavigatedTo(e);
        }

        void ConversationsPage_Loaded(object sender, RoutedEventArgs e)
        {
            FanfouWP.API.FanfouAPI.Instance.DirectMessageConversationListSuccess += Instance_DirectMessageConversationListSuccess;
            FanfouWP.API.FanfouAPI.Instance.DirectMessageConversationListFailed += Instance_DirectMessageConversationListFailed;

            if (this.list != null)
            {
                this.ConversationListBox.ItemsSource = list;
                return;
            }

            FanfouWP.API.FanfouAPI.Instance.DirectMessagesConversationList();

            Dispatcher.BeginInvoke(() =>
            {
                this.loading.Visibility = System.Windows.Visibility.Visible;
            });
        }


        void Instance_DirectMessageConversationListFailed(object sender, FailedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
                this.toast.NewToast("私信列表获取失败:( " + e.error.error);

            });
        }

        void Instance_DirectMessageConversationListSuccess(object sender, UserTimelineEventArgs<DirectMessageItem> e)
        {
            Dispatcher.BeginInvoke(() =>
           {
               this.loading.Visibility = System.Windows.Visibility.Collapsed;
               if ((e as UserTimelineEventArgs<DirectMessageItem>).UserStatus.Count != 0)
               {
                   list = e.UserStatus;
                   this.ConversationListBox.ItemsSource = list;
                   changeMenu(false);
                   this.page.Text = "第" + currentPage.ToString() + "页";
               }
               else
               {
                   changeMenu(true);
               }
           });
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

        private void ForeButton_Click(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.loading.Visibility = System.Windows.Visibility.Visible;
            });
            if (currentPage >= 1)
            {
                currentPage--;
                FanfouWP.API.FanfouAPI.Instance.DirectMessagesConversationList(currentPage);
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
            FanfouWP.API.FanfouAPI.Instance.DirectMessagesConversationList(currentPage);
            changeMenu(false, true);
        }

        private void ConversationListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.ConversationListBox.SelectedItem != null)
            {
                var item = this.ConversationListBox.SelectedItem;
                this.ConversationListBox.SelectedItem = null;

                if (PhoneApplicationService.Current.State.ContainsKey("MessagePage"))
                {
                    PhoneApplicationService.Current.State.Remove("MessagePage");
                }
                PhoneApplicationService.Current.State.Add("MessagePage", item);
                NavigationService.Navigate(new Uri("/MessagePage.xaml", UriKind.Relative));
            }
        }
    }
}