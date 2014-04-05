using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;
using FanfouWP.API;

namespace FanfouWP.UserControls
{
    public partial class PanoramaTitleControl : UserControl
    {
      
        public PanoramaTitleControl()
        {
            InitializeComponent();
        }

        private void AvatarImage_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (FanfouAPI.Instance.CurrentUser != null)
            {
                if (PhoneApplicationService.Current.State.ContainsKey("UserPage"))
                {
                    PhoneApplicationService.Current.State.Remove("UserPage");
                }
                PhoneApplicationService.Current.State.Add("UserPage", FanfouAPI.Instance.CurrentUser);
                App.RootFrame.Navigate(new Uri("/UserPage.xaml", UriKind.Relative));
            }
        }

    }
}
