using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Runtime.Serialization.Json;
using System.IO;

namespace ACFUN
{
    public partial class LoginPage : PhoneApplicationPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (tbusername.Text == null || tbusername.Text == "")
            { MessageBox.Show("用户名不能为空"); return; }
            if (tbpassword.Password == null || tbpassword.Password == "")
            { MessageBox.Show("密码不能为空"); return; }

            indicator.IsVisible = true;
            HttpHelp httpHelp = new HttpHelp();
            httpHelp.parameters.Add("username", tbusername.Text);
            httpHelp.parameters.Add("password", tbpassword.Password);
            using (var stream = await httpHelp.PostSetCookie("http://www.acfun.tv/login.aspx"))
            {
                if (stream != null)
                {
                    DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(loginresult));
                    var result = json.ReadObject(stream) as loginresult;
                    if (result.success)
                    {
                        Settings.AddOrUpdateValue("username", tbusername.Text);
                        Settings.AddOrUpdateValue("password", tbpassword.Password);
                        StaticData.UserName = result.username;
                        StaticData.UserImg = result.img;
                        StaticData.IsLogin = true;
                        if (NavigationService.CanGoBack)
                            NavigationService.GoBack();
                    }
                    else if (result.result != null && result.result != "")
                    {
                        MessageBox.Show(result.result);
                    }
                    else
                    {
                        MessageBox.Show("未知异常");
                    }
                }
                else
                {
                    MessageBox.Show("未知异常");
                }
            }

            indicator.IsVisible = false;
        }
    }
}