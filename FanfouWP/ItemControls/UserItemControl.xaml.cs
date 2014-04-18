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
    public partial class UserItemControl : UserControl
    {
        public UserItemControl()
        {
            InitializeComponent();

            this.Loaded += UserItemControl_Loaded;
        }

        void UserItemControl_Loaded(object sender, RoutedEventArgs e)
        {
            if ((this.DataContext as FanfouWP.API.Items.User).status == null)
            {
                this.message.Text = "此用户消息未公开";
            }
        }


    }
}
