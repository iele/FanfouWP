using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace FanfouWP
{
    public partial class MentionTimelinePage : PhoneApplicationPage
    {
        public MentionTimelinePage()
        {
            InitializeComponent();
            this.Loaded += MentionTimelinePage_Loaded;
        }

        void MentionTimelinePage_Loaded(object sender, RoutedEventArgs e)
        {
            this.MentionTimelineListBox.ItemsSource = FanfouWP.API.FanfouAPI.Instance.MentionTimeLineStatus;  
        }

        private void MentionTimelineListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.MentionTimelineListBox.SelectedItem != null)
            {
                var item = this.MentionTimelineListBox.SelectedItem;

                if (PhoneApplicationService.Current.State.ContainsKey("StatusPage"))
                {
                    PhoneApplicationService.Current.State.Remove("StatusPage");
                }
                PhoneApplicationService.Current.State.Add("StatusPage", item);
                NavigationService.Navigate(new Uri("/StatusPage.xaml", UriKind.Relative));
            }
        }
    }
}