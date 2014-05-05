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
    public partial class ConversationOtherItemControl : UserControl
    {
        public ConversationOtherItemControl()
        {
            InitializeComponent();

            this.Loaded += ConversationControl_Loaded;
        }

        void ConversationControl_Loaded(object sender, RoutedEventArgs e)
        {
          
        }

    }
}
