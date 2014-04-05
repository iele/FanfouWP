using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using System.Windows.Media;
using FanfouWP.Storage;

namespace YueFM.Pages
{
    public partial class SettingPage : PhoneApplicationPage
    {
        private SettingManager settingManager = SettingManager.GetInstance();
    
        public SettingPage()
        {
            InitializeComponent();
      
     
        }

    
    }
}