using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using AcFun.UWP.Control;
using AcFun.UWP.Helper;
using AcFun.UWP.Model;
using BaicaiMobileService.Helper;
using AcFun.UWP.Module;
using Windows.System;
using User = AcFun.UWP.Module.User;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace AcFun.UWP.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ContentPage : Page
    {
        private InfoBindingModel Model { get; set; }

        public ContentPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.NavigationMode == NavigationMode.New)
            {
                Model = e.Parameter as InfoBindingModel;
                if (Model != null)
                {
                    MainTitle.Text = Model.Title;
                    SubHeader.Text = Model.SubHeader;
                    SetContent(Model.Txt);
                    CommentFrame.Navigate(typeof(CommentPage), Model.ContentId);
                    Comment.Instance.QuoteId = -1;
                }
                Comment.Instance.QuoteFloorChanged += QuoteFloorChanged;

                FavoriteButtonRef(true);
            }
        }

        private async void FavoriteButtonRef(bool isNet,string isCollect = "收藏")
        {
            try
            {
                if (isNet)
                {
                    var url = string.Format(AppData.IsCollectUrl, Model.ContentId);
                    var str = await Http.Instance.GetStringAsync(url);
                    var obj = str.ToJsonObject<IsCollectResult.Rootobject>();
                    if (obj.Success)
                    {
                        FavoriteButton.Label = obj.Data.Collect ? "取消收藏" : "收藏";
                    }
                }
                else
                {
                    FavoriteButton.Label = isCollect;
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
            }
        }

        private void QuoteFloorChanged(object sender, int i)
        {
            if (i > 0)
            {
                CommentButton.Width = 140;
                CommentButton.Label = $"评论(引用#{i})";
            }
            else
            {
                CommentButton.Width = 100;
                CommentButton.Label = $"评论";
            }
        }

        private void SetContent(string txt)
        {
            ContentRichBlock.Blocks.Clear();
            foreach (var block in Html.GetContent(txt))
            {
                ContentRichBlock.Blocks.Add(block);
            }
        }

        private async void CommentButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!User.IsLogin)
            {
                LoginBox lb = new LoginBox();
                lb.LoginCompleted += CommentSubmitShow;
                await lb.ShowAsync();
            }
            else
            {
                CommentSubmitShow(this, User.UserLoginData);
            }
        }

        private async void CommentSubmitShow(object sender, LoginResult.Rootobject rootobject)
        {
            (sender as LoginBox)?.Hide();
            CommentSubmit cs = new CommentSubmit(Model.ContentId);
            cs.SubmitCompleted += (o, b) =>
            {
                CommentPage.Instance.Refresh();
            };
            await cs.ShowAsync();
        }

        private async void DeleteSoButton_OnClick(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("http://www.delete.so/search.html?search=ac" + Model.ContentId));
        }

        private async void FavoriteButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!User.IsLogin)
            {
                ToastPrompt.Show("请先登录");
                return;
            }
            if (FavoriteButton.Label == "收藏")
            {
                var content = new StringContent($"operate=1&cId={Model.ContentId}");
                var url = string.Format(AppData.CollectUrl + $"?operate=1&cId={Model.ContentId}");
                var response = await Http.Instance.PostAsync(url, content);
                var str = await response.Content.ReadAsStringAsync();
                var obj = str.ToJsonObject<CommonResult.Rootobject>();
                if (obj.Success)
                {
                    FavoriteButtonRef(false, "取消收藏");
                    ToastPrompt.Show("收藏成功");
                }
                else
                {
                    ToastPrompt.Show(obj.Info);
                }
            }
            else
            {
                var content = new StringContent($"operate=0&cId={Model.ContentId}");
                var url = string.Format(AppData.CollectUrl + $"?operate=0&cId={Model.ContentId}");
                var response = await Http.Instance.PostAsync(url, content);
                var str = await response.Content.ReadAsStringAsync();
                var obj = str.ToJsonObject<CommonResult.Rootobject>();
                if (obj.Success)
                {
                    FavoriteButtonRef(false, "收藏");
                    ToastPrompt.Show("取消收藏成功");
                }
                else
                {
                    ToastPrompt.Show(obj.Info);
                }
            }
        }
    }
}
