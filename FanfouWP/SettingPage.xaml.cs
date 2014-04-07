using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using System.Windows.Media;
using FanfouWP.Storage;
using Windows.Storage;

namespace YueFM.Pages
{
    public partial class SettingPage : PhoneApplicationPage
    {
        private SettingManager settingManager = SettingManager.GetInstance();

        public SettingPage()
        {
            InitializeComponent();


        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            settingManager.username = null;
            settingManager.password = null;
            settingManager.oauthToken = null;
            settingManager.oauthSecret = null;

            Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            Dispatcher.BeginInvoke(
            async () =>
            {
                var dataFolder = await localFolder.CreateFolderAsync("storage", CreationCollisionOption.OpenIfExists);
                await dataFolder.DeleteAsync();

                NavigationService.RemoveBackEntry();
                NavigationService.Navigate(new Uri("/LoginPage.xaml", UriKind.Relative));
            });
        }


    }
}