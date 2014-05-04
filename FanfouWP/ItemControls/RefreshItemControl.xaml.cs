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
    public partial class RefreshItemControl : UserControl
    {
        public RefreshItemControl()
        {
            InitializeComponent();
        }

        private void TextBlock_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.TextBlock.Visibility = Visibility.Collapsed;
            this.progressBar.Visibility = Visibility.Visible;
        }
    }
}
