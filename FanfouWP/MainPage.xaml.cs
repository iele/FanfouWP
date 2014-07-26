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
using Windows.Phone.Speech.VoiceCommands;
using Microsoft.Phone.Notification;
using FanfouWP.API.Items;

namespace FanfouWP
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();

            Dispatcher.BeginInvoke(async () =>
            {
                try
                {
                    await VoiceCommandService.InstallCommandSetsFromFileAsync(new Uri("ms-appx:///VoiceCommandDefinition.xml"));
                }
                catch (Exception e)
                {
                    if (!Storage.SettingManager.GetInstance().voiceError)
                    {
                        MessageBox.Show("注册语音命令错误,请安装中文语音包");
                        Storage.SettingManager.GetInstance().voiceError = true;
                        Storage.SettingManager.GetInstance().SaveSettings();
                    }
                }
            });


            Dispatcher.BeginInvoke(async () => await Utils.GeoLocator.getGeolocator());

            this.Loaded += MainPage_Loaded;
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            FanfouWP.API.FanfouAPI.Instance.RestoreDataSuccess += Instance_RestoreDataSuccess;
            FanfouWP.API.FanfouAPI.Instance.RestoreDataFailed += Instance_RestoreDataFailed;

            Thread.Sleep(500);

            if (FanfouWP.Storage.SettingManager.GetInstance().currentList.Count != 0)
            {
                var list = FanfouWP.Storage.SettingManager.GetInstance().currentList as List<User>;
                if (FanfouAPI.Instance.CurrentUser != null)
                {
                    var i = from l in list where l.id == FanfouWP.Storage.SettingManager.GetInstance().currentUser.id select l;
                    if (i.Count() != 0)
                    {
                        Dispatcher.BeginInvoke(async () => await FanfouWP.API.FanfouAPI.Instance.TryRestoreData());
                        return;
                    }
                }
                while (NavigationService.CanGoBack) NavigationService.RemoveBackEntry();
                Dispatcher.BeginInvoke(() => NavigationService.Navigate(new Uri("/AccountsPage.xaml", UriKind.Relative)));
            }
            else
            {
                while (NavigationService.CanGoBack) NavigationService.RemoveBackEntry();
                Dispatcher.BeginInvoke(() => NavigationService.Navigate(new Uri("/LoginPage.xaml", UriKind.Relative)));
            }
        }

        void Instance_RestoreDataFailed(object sender, API.Event.FailedEventArgs e)
        {
            while (NavigationService.CanGoBack) NavigationService.RemoveBackEntry();
            Dispatcher.BeginInvoke(() => NavigationService.Navigate(new Uri("/AccountPage.xaml", UriKind.Relative)));
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