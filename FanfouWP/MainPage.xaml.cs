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
using System.Threading.Tasks;

namespace FanfouWP
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();

            this.Loaded += MainPage_Loaded;
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            FanfouWP.API.FanfouAPI.Instance.RestoreDataSuccess += Instance_RestoreDataSuccess;
            FanfouWP.API.FanfouAPI.Instance.RestoreDataFailed += Instance_RestoreDataFailed;
            FanfouWP.API.FanfouAPI.Instance.TryRestoreData();
        }

        void Instance_RestoreDataFailed(object sender, API.Event.FailedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/LoginPage.xaml", UriKind.Relative));
        }

        void Instance_RestoreDataSuccess(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/TimelinePage.xaml", UriKind.Relative));
        }


    }
}