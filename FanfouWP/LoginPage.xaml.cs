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
using Microsoft.Phone.Tasks;
using Windows.UI.ViewManagement;

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
            FanfouAPI.VerifyCredentialsSuccess += FanfouAPI_VerifyCredentialsSuccess;
            FanfouAPI.VerifyCredentialsFailed += FanfouAPI_VerifyCredentialsFailed;

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
                toast.NewToast("登录失败，请检查用户名或密码后再试");
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
                (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = true;
            });
        }

        void FanfouAPI_LoginSuccess(object sender, EventArgs e)
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                FanfouAPI.VerifyCredentials();
            });
        }

        void FanfouAPI_VerifyCredentialsFailed(object sender, API.Event.FailedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                toast.NewToast("登录失败，请检查用户名或密码后再试");
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
                (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = true;
            });
        }

        void FanfouAPI_VerifyCredentialsSuccess(object sender, EventArgs e)
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
                this.NavigationService.Navigate(new Uri("/TimelinePage.xaml", UriKind.Relative));
                (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = true;
            });
        }

        private void ClickButton_Click(object sender, EventArgs e)
        {
            FanfouAPI.Login(this.UsernameText.Text, this.PasswordBox.Password);

            Dispatcher.BeginInvoke(() =>
            {
                this.loading.Visibility = System.Windows.Visibility.Visible;
                (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = false;
                this.Focus();
            });
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserTask wbt = new WebBrowserTask();
            wbt.Uri = new Uri("http://m.fanfou.com/register/");
            wbt.Show();
        }

        private void UsernameText_GotFocus(object sender, RoutedEventArgs e)
        {
            this.usernameTip.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            this.passwordTip.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void PasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (PasswordBox.Password.Length == 0)
                this.passwordTip.Visibility = System.Windows.Visibility.Visible;
            else
                this.passwordTip.Visibility = System.Windows.Visibility.Collapsed;

        }

        private void UsernameText_LostFocus(object sender, RoutedEventArgs e)
        {
            if (UsernameText.Text.Length == 0)
                this.usernameTip.Visibility = System.Windows.Visibility.Visible;
            else
                this.usernameTip.Visibility = System.Windows.Visibility.Collapsed;

        }
    }
}