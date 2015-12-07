using System.Windows;
using Microsoft.Phone.Controls;
using System.Runtime.Serialization.Json;

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
            if (string.IsNullOrEmpty(tbusername.Text))
            { MessageBox.Show("用户名不能为空"); return; }
            if (string.IsNullOrEmpty(tbpassword.Password))
            { MessageBox.Show("密码不能为空"); return; }

            indicator.IsVisible = true;
            var httpHelp = new HttpHelp();
            httpHelp.parameters.Add("username", tbusername.Text);
            httpHelp.parameters.Add("password", tbpassword.Password);
            using (var stream = await httpHelp.PostSetCookie(string.Format(StaticData.login, tbusername.Text, tbpassword.Password)))
            {
                if (stream != null)
                {
                    var json = new DataContractJsonSerializer(typeof(loginresult));
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
                    else if (!string.IsNullOrEmpty(result.result))
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