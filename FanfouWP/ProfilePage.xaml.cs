using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using FanfouWP.Utils;

namespace FanfouWP
{
    public partial class ProfilePage : PhoneApplicationPage
    {
        private ToastUtil toast = new ToastUtil();
        public ProfilePage()
        {
            InitializeComponent();

            FanfouWP.API.FanfouAPI.Instance.AccountUpdateProfileSuccess += Instance_AccountUpdateProfileSuccess;
            FanfouWP.API.FanfouAPI.Instance.AccountUpdateProfileFailed += Instance_AccountUpdateProfileFailed;
        }

        void Instance_AccountUpdateProfileFailed(object sender, API.Event.FailedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
                (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = true;
                toast.NewToast("资料更新失败:( " + e.error.error);
            });
        }

        void Instance_AccountUpdateProfileSuccess(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
                (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = true;
                MessageBox.Show("资料更新成功，用户资料更新需要一段时间请稍做等待");
            });
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.loading.Visibility = System.Windows.Visibility.Visible;
                (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = false;
                FanfouWP.API.FanfouAPI.Instance.AccountUpdateProfile(this.url.Text, this.location.Text, this.description.Text, "", this.email.Text);
            });
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (e.NavigationMode != NavigationMode.Back)
            {
                State["ProfilePage_url"] = this.url.Text;
                State["ProfilePage_email"] = this.email.Text;
                State["ProfilePage_location"] = this.location.Text;
                State["ProfilePage_description"] = this.description.Text;
            }

            base.OnNavigatedFrom(e);
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (State.ContainsKey("ProfilePage_url"))
                this.url.Text = State["ProfilePage_url"] as string;
            if (State.ContainsKey("ProfilePage_email"))
                this.email.Text = State["ProfilePage_email"] as string;
            if (State.ContainsKey("ProfilePage_location"))
                this.location.Text = State["ProfilePage_location"] as string;
            if (State.ContainsKey("ProfilePage_description"))
                this.description.Text = State["ProfilePage_description"] as string;

            base.OnNavigatedTo(e);
        }
    }
}