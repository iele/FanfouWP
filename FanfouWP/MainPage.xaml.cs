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
using Microsoft.Phone.Scheduler;
using System.Threading;

namespace FanfouWP
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();

            Dispatcher.BeginInvoke(async () => await Utils.GeoLocatorUtils.getGeolocator());

            this.Loaded += MainPage_Loaded;
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            FanfouWP.API.FanfouAPI.Instance.RestoreDataSuccess += Instance_RestoreDataSuccess;
            FanfouWP.API.FanfouAPI.Instance.RestoreDataFailed += Instance_RestoreDataFailed;

            Thread.Sleep(500);

            FanfouWP.API.FanfouAPI.Instance.TryRestoreData();
     }

        void Instance_RestoreDataFailed(object sender, API.Event.FailedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/LoginPage.xaml", UriKind.Relative));
        }

        void Instance_RestoreDataSuccess(object sender, EventArgs e)
        {
            if (PhoneApplicationService.Current.State.ContainsKey("TimelinePage"))
            {
                PhoneApplicationService.Current.State.Remove("TimelinePage");
            }
            PhoneApplicationService.Current.State["TimelinePage"] = true;
            NavigationService.Navigate(new Uri("/TimelinePage.xaml", UriKind.Relative));
        }

      
    }
}