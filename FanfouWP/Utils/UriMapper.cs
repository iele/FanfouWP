﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace FanfouWP.Utils
{
    class UriMapper : UriMapperBase
    {
        public override Uri MapUri(Uri uri)
        {
            string tempUri = uri.ToString();
            string mappedUri;

            // Launch from the photo share picker.
            // Incoming URI example: /MainPage.xaml?Action=ShareContent&FileId=%7BA3D54E2D-7977-4E2B-B92D-3EB126E5D168%7D
            if ((tempUri.Contains("ShareContent")) && (tempUri.Contains("FileId")))
            {
                // Redirect to PhotoShare.xaml.
                mappedUri = tempUri.Replace("MainPage", "SendPage");
                return new Uri(mappedUri, UriKind.Relative);
            }

            // Otherwise perform normal launch.
            return uri;
        }
    }
}
