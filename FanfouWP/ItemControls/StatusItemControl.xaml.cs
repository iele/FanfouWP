using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Input;
using FanfouWP.API;

namespace FanfouWP.ItemControls
{
    public partial class StatusItemControl : UserControl
    {
        public StatusItemControl()
        {
            InitializeComponent();

            this.Loaded += StatusItemControl_Loaded;
        }

        void Instance_FavoritesDestroyFailed(object sender, API.Event.FailedEventArgs e)
        {
            FanfouAPI.Instance.FavoritesDestroySuccess -= Instance_FavoritesDestroySuccess;
            FanfouAPI.Instance.FavoritesDestroyFailed -= Instance_FavoritesDestroyFailed;
        }

        void Instance_FavoritesDestroySuccess(object sender, EventArgs e)
        {
            FanfouAPI.Instance.FavoritesDestroySuccess -= Instance_FavoritesDestroySuccess;
            FanfouAPI.Instance.FavoritesDestroyFailed -= Instance_FavoritesDestroyFailed;
        }

        void Instance_FavoritesCreateFailed(object sender, API.Event.FailedEventArgs e)
        {
            FanfouAPI.Instance.FavoritesCreateFailed -= Instance_FavoritesCreateFailed;
            FanfouAPI.Instance.FavoritesCreateSuccess -= Instance_FavoritesCreateSuccess;
        }

        void Instance_FavoritesCreateSuccess(object sender, EventArgs e)
        {
            FanfouAPI.Instance.FavoritesCreateFailed -= Instance_FavoritesCreateFailed;
            FanfouAPI.Instance.FavoritesCreateSuccess -= Instance_FavoritesCreateSuccess;
        }

        void StatusItemControl_Loaded(object sender, RoutedEventArgs e)
        {
            if ((this.DataContext as FanfouWP.API.Items.Status).favorited == false)
            {
                this.favMenuItem.Header = "收藏";
            }
            else {
                this.favMenuItem.Header = "取消收藏";            
            }
        }

        private void MenuItem1_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (PhoneApplicationService.Current.State.ContainsKey("Reply"))
            {
                PhoneApplicationService.Current.State.Remove("Reply");
            }
            PhoneApplicationService.Current.State.Add("Reply", this.DataContext);
            App.RootFrame.Navigate(new Uri("/SendPage.xaml", UriKind.Relative));
        }

        private void MenuItem2_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (PhoneApplicationService.Current.State.ContainsKey("ReSend"))
            {
                PhoneApplicationService.Current.State.Remove("ReSend");
            }
            PhoneApplicationService.Current.State.Add("ReSend", this.DataContext);
            App.RootFrame.Navigate(new Uri("/SendPage.xaml", UriKind.Relative));
        }

        private void MenuItem3_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if ((this.DataContext as FanfouWP.API.Items.Status).favorited == false)
            {
                FanfouWP.API.FanfouAPI.Instance.FavoritesCreate((this.DataContext as FanfouWP.API.Items.Status).id);
                FanfouAPI.Instance.FavoritesCreateSuccess += Instance_FavoritesCreateSuccess;
                FanfouAPI.Instance.FavoritesCreateFailed += Instance_FavoritesCreateFailed;
           
            }
            else {
                FanfouWP.API.FanfouAPI.Instance.FavoritesDestroy((this.DataContext as FanfouWP.API.Items.Status).id); 
                FanfouAPI.Instance.FavoritesDestroySuccess += Instance_FavoritesDestroySuccess;
                FanfouAPI.Instance.FavoritesDestroyFailed += Instance_FavoritesDestroyFailed;
     
            }
        }

    }
}
