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
    public partial class ConversationControl : UserControl
    {
        public ConversationControl()
        {
            InitializeComponent();

            this.Loaded += ConversationControl_Loaded;
        }

        void ConversationControl_Loaded(object sender, RoutedEventArgs e)
        {            
        }

    }
}
