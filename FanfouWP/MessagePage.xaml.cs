﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Collections.ObjectModel;
using FanfouWP.API;
using System.Threading;
using System.Windows.Threading;

namespace FanfouWP
{
    public partial class MessagePage : PhoneApplicationPage
    {
        private FanfouWP.API.Items.DirectMessageItem dm;
        private List<FanfouWP.API.Items.DirectMessage> list;
        private FanfouWP.API.Items.User user;

        public MessagePage()
        {
            InitializeComponent();

            if (PhoneApplicationService.Current.State.ContainsKey("MessagePage"))
            {
                this.dm = PhoneApplicationService.Current.State["MessagePage"] as FanfouWP.API.Items.DirectMessageItem;
                PhoneApplicationService.Current.State.Remove("MessagePage");

            }

            if (dm != null)
            {
                if (dm.dm.sender.id == dm.otherid)
                    this.user = dm.dm.sender;
                else
                    this.user = dm.dm.recipient;
            }

            if (PhoneApplicationService.Current.State.ContainsKey("MessagePage_User"))
            {
                this.user = PhoneApplicationService.Current.State["MessagePage_User"] as FanfouWP.API.Items.User;
                PhoneApplicationService.Current.State.Remove("MessagePage_User");
            }



            this.Loaded += MessagePage_Loaded;
        }

        void MessagePage_Loaded(object sender, RoutedEventArgs e)
        {
            FanfouWP.API.FanfouAPI.Instance.DirectMessageConversationSuccess += Instance_DirectMessageConversationSuccess;
            FanfouWP.API.FanfouAPI.Instance.DirectMessageConversationFailed += Instance_DirectMessageConversationFailed;

            FanfouWP.API.FanfouAPI.Instance.DirectMessageNewSuccess += Instance_DirectMessageNewSuccess;
            FanfouWP.API.FanfouAPI.Instance.DirectMessageNewFailed += Instance_DirectMessageNewFailed;

            FanfouWP.API.FanfouAPI.Instance.DirectMessagesConversation(this.user.id);

            Dispatcher.BeginInvoke(() =>
            {
                this.title.Text = "与" + this.user.screen_name + "的对话";
            });

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(60);
            timer.Tick += (s, et) =>
            {
                this.loading.Visibility = System.Windows.Visibility.Visible;
                FanfouWP.API.FanfouAPI.Instance.DirectMessagesConversation(this.user.id);
            };
            timer.Start();
        }

        void Instance_DirectMessageConversationFailed(object sender, API.Event.FailedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.loading.Visibility = System.Windows.Visibility.Collapsed;

                this.toast.NewToast("私信列表获取失败:( " + e.error.error);
            });
        }

        void Instance_DirectMessageConversationSuccess(object sender, API.Event.UserTimelineEventArgs<API.Items.DirectMessage> e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
                list = e.UserStatus.Reverse().ToList();
                this.MessageListBox.ItemsSource = list;
                if (list.Count > 0)
                    this.MessageListBox.ScrollTo(list.Last());
            });
        }

        private void send_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.loading.Visibility = System.Windows.Visibility.Visible;
                this.send.IsEnabled = false;
                this.message.IsEnabled = false;
            });

            FanfouWP.API.FanfouAPI.Instance.DirectMessagesNew(user.id, this.message.Text, list.Count > 0 ? list.Last().id : null);
        }
        void Instance_DirectMessageNewFailed(object sender, API.Event.FailedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
                this.send.IsEnabled = true;
                this.message.IsEnabled = true;
                this.message.Text = "";
                this.toast.NewToast("私信发送失败:( " + e.error.error);
            });
        }

        void Instance_DirectMessageNewSuccess(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
                this.send.IsEnabled = true;
                this.message.Text = "";
                this.message.IsEnabled = true;

                this.list.Add(sender as FanfouWP.API.Items.DirectMessage);

                this.MessageListBox.ItemsSource = list;
                this.MessageListBox.ScrollTo(list.Last());
            });
        }

    }
}