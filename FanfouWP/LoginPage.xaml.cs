using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using FanfouWP.API;
using FanfouWP.Storage;

namespace FanfouWP
{
    public partial class LoginPage : PhoneApplicationPage
    {
        private FanfouAPI FanfouAPI = FanfouAPI.Instance;
        private SettingManager settings = SettingManager.GetInstance();
        public LoginPage()
        {
            InitializeComponent();
            FanfouAPI.LoginSuccess += FanfouAPI_LoginSuccess;
            FanfouAPI.LoginFailed += FanfouAPI_LoginFailed;

            this.Loaded += LoginPage_Loaded;
        }

        void LoginPage_Loaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(() => { this.NavigationService.RemoveBackEntry(); });
        }

        private void FanfouAPI_LoginFailed(object sender, API.Event.FailedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                MessageBox.Show("登录失败");
                this.loading.Visibility = System.Windows.Visibility.Visible;
                (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = true;
            });
        }

        void FanfouAPI_LoginSuccess(object sender, EventArgs e)
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                this.loading.Visibility = System.Windows.Visibility.Visible;
                this.NavigationService.Navigate(new Uri("/TimelinePage.xaml", UriKind.Relative));
                (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = true;
            });
        }

        private void ClickButton_Click(object sender, EventArgs e)
        {
            FanfouAPI.Login(this.UsernameTest.Text, this.PasswordBox.Password);

            Dispatcher.BeginInvoke(() =>
            {
                this.loading.Visibility = System.Windows.Visibility.Visible;
                (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = false;
            });
        }
    }
}