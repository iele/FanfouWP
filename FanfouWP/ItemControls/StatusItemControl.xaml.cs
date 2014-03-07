using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace FanfouWP.ItemControls
{
    public partial class StatusItemControl : UserControl
    {
        public StatusItemControl()
        {
            InitializeComponent();

            this.Loaded += StatusItemControl_Loaded;
        }

        void StatusItemControl_Loaded(object sender, RoutedEventArgs e)
        {
         
        }
    }
}
