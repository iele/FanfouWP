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

namespace FanfouWP.UserControls
{
    public partial class PanoramaTitleControl : UserControl
    {
        public delegate void AvatarTapHandler(object sender, EventArgs e);
        public event AvatarTapHandler AvatarTap;


        public PanoramaTitleControl()
        {
            InitializeComponent();
        }

        private void AvatarImage_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (AvatarTap != null)
                AvatarTap(this, new EventArgs());
        }

    }
}
