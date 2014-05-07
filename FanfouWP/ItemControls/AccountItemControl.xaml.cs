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
using FanfouWP.API.Items;

namespace FanfouWP.ItemControls
{
    public partial class AccountItemControl : UserControl
    {
        public delegate void DeleteCompletedHandler(object sender, EventArgs e);
       
        public event DeleteCompletedHandler DeleteCompleted;
        private void OnDeleteCompleted(EventArgs eventArgs)
        {
            var handler = this.DeleteCompleted;
            if (handler != null)
            {
                handler(this.DataContext, eventArgs);
            }
        }
        public AccountItemControl()
        {
            InitializeComponent();

            this.Loaded += AccountItemControl_Loaded;
        }

        void AccountItemControl_Loaded(object sender, RoutedEventArgs e)
        {
            if ((this.DataContext as FanfouWP.API.Items.User).status == null)
            {
                this.message.Text = "此用户消息未公开";
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var item = DataContext as User;

            OnDeleteCompleted(new EventArgs());
            
            Dispatcher.BeginInvoke(() =>
            {
                FanfouWP.API.FanfouAPI.Instance.DeleteManager(item as User);
            });
        }
    }
}
