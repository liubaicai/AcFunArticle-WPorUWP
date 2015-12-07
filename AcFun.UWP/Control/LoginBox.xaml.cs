using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using AcFun.UWP.Model;
using AcFun.UWP.Module;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace AcFun.UWP.Control
{
    public sealed partial class LoginBox : ContentDialog
    {
        public EventHandler<LoginResult.Rootobject> LoginCompleted;
        public EventHandler LoginCancel;

        public LoginBox()
        {
            this.InitializeComponent();
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            args.Cancel = true;
            if (string.IsNullOrEmpty(UserNameBox.Text) || string.IsNullOrEmpty(PassWordBox.Password))
            {
                NoticeBlock.Text = "用户名密码不能为空";
            }
            else
            {
                var data = await User.LoginTask(UserNameBox.Text, PassWordBox.Password ,CaptchaBox.Text);
                if (data.Success)
                {
                    LoginCompleted?.Invoke(this, data);
                    this.Hide();
                }
                else
                {
                    NoticeBlock.Text = data.Result;
                    if (data.Result.Contains("captcha"))
                    {
                        CaptchaGrid.Visibility = Visibility.Visible;
                        Captcha.Source = new BitmapImage(new Uri("http://www.acfun.tv/captcha.svl?" + DateTime.Now.Millisecond));
                    }
                }
            }
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            LoginCancel?.Invoke(this, null);
        }
    }
}
