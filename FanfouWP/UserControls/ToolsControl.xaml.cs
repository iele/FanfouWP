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

namespace FanfouWP.UserControls
{
    public partial class ToolsControl : UserControl
    {
        public ToolsControl()
        {
            InitializeComponent();
        }

        public void GroupTileAnimationEnable(bool enable)
        {
            if (enable)
                HubTileService.UnfreezeGroup("tile");
            else
                HubTileService.FreezeGroup("tile");
        }

        private void DirectMsgTile_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

            App.RootFrame.Navigate(new Uri("/ConversationsPage.xaml", UriKind.Relative));
        }

        private void FavTile_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var user = this.DataContext as FanfouWP.API.Items.User;

            if (PhoneApplicationService.Current.State.ContainsKey("FavStatusPage"))
            {
                PhoneApplicationService.Current.State.Remove("FavStatusPage");
            }
            PhoneApplicationService.Current.State.Add("FavStatusPage", user);
            App.RootFrame.Navigate(new Uri("/FavStatusPage.xaml", UriKind.Relative));
        }

        private void TagTile_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var user = this.DataContext as FanfouWP.API.Items.User;

            if (PhoneApplicationService.Current.State.ContainsKey("TagPage"))
            {
                PhoneApplicationService.Current.State.Remove("TagPage");
            }
            PhoneApplicationService.Current.State.Add("TagPage", user);
            App.RootFrame.Navigate(new Uri("/TagPage.xaml", UriKind.Relative));
        }

        private void SearchTile_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            App.RootFrame.Navigate(new Uri("/SearchPage.xaml", UriKind.Relative));
        }

        private void TrendTile_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            App.RootFrame.Navigate(new Uri("/TrendsPage.xaml", UriKind.Relative));
        }

        private void SettingTile_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            App.RootFrame.Navigate(new Uri("/SettingPage.xaml", UriKind.Relative));
        }
    }
}
