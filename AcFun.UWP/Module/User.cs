using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AcFun.UWP.Control;
using AcFun.UWP.Helper;
using AcFun.UWP.Model;
using BaicaiMobileService.Helper;

namespace AcFun.UWP.Module
{
    public class User
    {
        public static bool IsLogin { get; set; } = false;

        public static LoginResult.Rootobject UserLoginData { get; set; }

        public static NotRepeatObservableCollection<AtListResult.CommentItem> AtListViewData { get; set; } = new NotRepeatObservableCollection<AtListResult.CommentItem>();

        public static NotRepeatObservableCollection<CollectionListResult.Content> CollectionListViewData { get; set; } = new NotRepeatObservableCollection<CollectionListResult.Content>();

        public static async Task<LoginResult.Rootobject> LoginTask(string username, string password, string captcha = "")
        {
            try
            {
                StringContent content;
                string url;
                if (string.IsNullOrEmpty(captcha))
                {
                    content = new StringContent($"username={username}&password={password}");
                    url = string.Format(AppData.LoginUrl, username, password);
                }
                else
                {
                    content = new StringContent($"username={username}&password={password}&captcha={captcha}");
                    url = string.Format(AppData.LoginUrl, username, password)+ $"&captcha={captcha}";
                }
                var response = await Http.Instance.PostAsync(url, content);
                response.Headers.GetCookie();
                var str = await response.Content.ReadAsStringAsync();
                var obj = str.ToJsonObject<LoginResult.Rootobject>();
                Debug.WriteLine("Login:" + obj.Success + obj.Result);
                if (obj.Success)
                {
                    IsLogin = true;
                    UserLoginData = obj;
                    response.Headers.GetCookie();
                    Settings.Set("username", username);
                    Settings.Set("password", password);
                }
                else
                {
                    IsLogin = false;
                    UserLoginData = null;
                    Settings.Set("username", "");
                    Settings.Set("password", "");
                }
                return obj;
            }
            catch (Exception exception)
            {
                IsLogin = false;
                Debug.WriteLine(exception.Message);
                return null;
            }
        }

        public static async Task AutoLoginTask()
        {
            try
            {
                string username = Settings.Get("username", "");
                string password = Settings.Get("password", "");
                if (string.IsNullOrEmpty(username)||string.IsNullOrEmpty(password))
                {
                    return;
                }
                var content = new StringContent($"username={username}&password={password}");
                var url = string.Format(AppData.LoginUrl, username, password);
                var response = await Http.Instance.PostAsync(url, content);
                response.Headers.GetCookie();
                var str = await response.Content.ReadAsStringAsync();
                var obj = str.ToJsonObject<LoginResult.Rootobject>();
                Debug.WriteLine("Login:" + obj.Success + obj.Result);
                if (obj.Success)
                {
                    IsLogin = true;
                    UserLoginData = obj;
                    Settings.Set("username", username);
                    Settings.Set("password", password);
                }
                else
                {
                    IsLogin = false;
                    UserLoginData = null;
                    Settings.Set("username", "");
                    Settings.Set("password", "");
                }
            }
            catch (Exception exception)
            {
                IsLogin = false;
                Debug.WriteLine(exception.Message);
            }
        }

        public static async Task Logout()
        {
            var url = string.Format(AppData.LogoutUrl);
            var str = await Http.Instance.GetStringAsync(url);
            Debug.WriteLine(str);
            IsLogin = false;
            UserLoginData = null;
            Settings.Set("username", "");
            Settings.Set("password", "");
        }
    }
}
