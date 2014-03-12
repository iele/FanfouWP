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

namespace FanfouWP
{
    public partial class LoginPage : PhoneApplicationPage
    {
        private FanfouAPI FanfouAPI = FanfouAPI.Instance;
        public LoginPage()
        {
            InitializeComponent();
            FanfouAPI.LoginSuccess += FanfouAPI_LoginSuccess;
            FanfouAPI.LoginFailed += FanfouAPI_LoginFailed;
        }

        private void FanfouAPI_LoginFailed(object sender, API.Event.FailedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                MessageBox.Show("登录失败");
            });
        }

        void FanfouAPI_LoginSuccess(object sender, EventArgs e)
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                this.NavigationService.Navigate(new Uri("/TimelinePanorama.xaml", UriKind.Relative));
            });
        }

        private void ClickButton_Click(object sender, EventArgs e)
        {
            FanfouAPI.Login(this.UsernameTest.Text, this.PasswordBox.Password);
    
        }
    }
}