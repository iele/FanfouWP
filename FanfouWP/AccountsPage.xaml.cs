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
using System.Collections.ObjectModel;
using FanfouWP.Storage;

namespace FanfouWP
{
    public partial class AccountsPage : PhoneApplicationPage
    {
        private SettingManager settings = SettingManager.GetInstance();

        public AccountsPage()
        {
            InitializeComponent();

            FanfouWP.API.FanfouAPI.Instance.RestoreDataSuccess += Instance_RestoreDataSuccess;
            FanfouWP.API.FanfouAPI.Instance.RestoreDataFailed += Instance_RestoreDataFailed;

            this.Loaded += AccountsPage_Loaded;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {

            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.AccountsStatusListBox.ItemsSource = settings.currentList;
            this.loading.Visibility = Visibility.Collapsed;

            base.OnNavigatedTo(e);
        }

        void AccountsPage_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void AccountsStatusListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.AccountsStatusListBox.SelectedItem != null)
            {
                var item = this.AccountsStatusListBox.SelectedItem;
                this.AccountsStatusListBox.SelectedIndex = -1;

                Dispatcher.BeginInvoke(() =>
                {
                   FanfouWP.API.FanfouAPI.Instance.UpdateManager(item as User);

                    Dispatcher.BeginInvoke(async () => await FanfouWP.API.FanfouAPI.Instance.TryRestoreData());
               });
            }
        }

        void Instance_RestoreDataFailed(object sender, API.Event.FailedEventArgs e)
        {
            NavigationService.RemoveBackEntry();
            Dispatcher.BeginInvoke(() => NavigationService.Navigate(new Uri("/LoginPage.xaml", UriKind.Relative)));
        }

        void Instance_RestoreDataSuccess(object sender, EventArgs e)
        {
            if (PhoneApplicationService.Current.State.ContainsKey("TimelinePage"))
            {
                PhoneApplicationService.Current.State.Remove("TimelinePage");
            }

            PhoneApplicationService.Current.State["TimelinePage"] = true;
     
            NavigationService.RemoveBackEntry();
            NavigationService.Navigate(new Uri("/TimelinePage.xaml", UriKind.Relative));
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() => NavigationService.Navigate(new Uri("/LoginPage.xaml", UriKind.Relative)));
        }
    }
}