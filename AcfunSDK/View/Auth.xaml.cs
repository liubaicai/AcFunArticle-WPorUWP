using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace AcfunSDK.View
{
    public partial class Auth : UserControl
    {
        public Auth()
        {
            InitializeComponent();
            this.Loaded += Auth_Loaded;
        }

        void Auth_Loaded(object sender, RoutedEventArgs e)
        {
            web.Navigate(new Uri("https://ssl.acfun.com/oauth2/authorize.aspx?state=&response_type=token&client_id=u8ZMaCE7aTpwgHkE&redirect_uri=https://ssl.acfun.com/authSuccess.aspx&scope="));
            web.Navigating += web_Navigating;
        }

        void web_Navigating(object sender, NavigatingEventArgs e)
        {
            if (e.Uri.ToString().StartsWith("https://ssl.acfun.com/authSuccess.aspx"))
            {
                MessageBox.Show(e.Uri.ToString().Split('#')[1]);
            }
        }
    }
}
