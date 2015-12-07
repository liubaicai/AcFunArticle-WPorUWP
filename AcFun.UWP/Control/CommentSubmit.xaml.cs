using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using AcFun.UWP.Helper;
using AcFun.UWP.Model;
using AcFun.UWP.Module;
using BaicaiMobileService.Helper;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace AcFun.UWP.Control
{
    public sealed partial class CommentSubmit : ContentDialog
    {
        public EventHandler<bool> SubmitCompleted;
        private void OnSubmitCompleted(bool isSuccess = true)
        {
            SubmitCompleted?.Invoke(this, isSuccess);
        }

        public int ContentId { get; set; }

        public CommentSubmit()
        {
            this.InitializeComponent();
            if (Comment.Instance.QuoteId > 0)
            {
                ClearQuoteLink.Content = $"正在引用#{Comment.Instance.QuoteFloor},点击取消";
                ClearQuoteLink.Visibility = Visibility.Visible;
            }
            else
            {
                ClearQuoteLink.Visibility = Visibility.Collapsed;
            }
        }

        public CommentSubmit(int contentId)
        {
            this.ContentId = contentId;
            this.InitializeComponent();
            if (Comment.Instance.QuoteId > 0)
            {
                ClearQuoteLink.Content = $"正在引用#{Comment.Instance.QuoteFloor},点击取消";
                ClearQuoteLink.Visibility = Visibility.Visible;
            }
            else
            {
                ClearQuoteLink.Visibility = Visibility.Collapsed;
            }
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            args.Cancel = true;
            if (string.IsNullOrEmpty(CommentTextBox.Text) || CommentTextBox.Text.Length < 5)
            {
                NoticeText.Text = "不得少于五个字~";
                return;
            }
            var http = Http.Instance;
            var SubmitText = CommentTextBox.Text.Replace(@"\r\n", @"\r\u003cbr/\u003e") + "   --UWP文章区";
            SubmitText = WebUtility.UrlEncode(SubmitText);
            StringContent content;
            string url;
            if (Comment.Instance.QuoteId > 0)
            {
                content = new StringContent($"text={SubmitText}&contentId={ContentId}&quoteId={Comment.Instance.QuoteId}");
                url = AppData.SendCommentUrl + $"?text={SubmitText}&contentId={ContentId}&quoteId={Comment.Instance.QuoteId}";
            }
            else
            {
                content = new StringContent($"text={SubmitText}&contentId={ContentId}");
                url = AppData.SendCommentUrl + $"?text={SubmitText}&contentId={ContentId}";
            }
            var result = await http.PostAsync(url, content);
            var str = await result.Content.ReadAsStringAsync();
            var obj = str.ToJsonObject<CommentSubmitResult.Rootobject>();
            if (obj.Success)
            {
                OnSubmitCompleted();
                Comment.Instance.SetQuote(-1, -1);
                ClearQuoteLink.Visibility = Visibility.Collapsed;
                this.Hide();
            }
            else
            {
                NoticeText.Text = obj.Info;
            }
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            args.Cancel = false;
        }

        private void ClearQuoteLink_OnClick(object sender, RoutedEventArgs e)
        {
            Comment.Instance.SetQuote(-1,-1);
            ClearQuoteLink.Visibility = Visibility.Collapsed;
        }

        private async void EmojiButton_Click(object sender, RoutedEventArgs e)
        {
            EmojiBox eb = new EmojiBox();
            eb.EmojiSelectCompleted += async (o, s) =>
            {
                eb.Hide();
                CommentTextBox.Text = CommentTextBox.Text + s;
                CommentTextBox.SelectionStart = CommentTextBox.Text.Length;
                await ShowAsync();
            };
            Hide();
            await eb.ShowAsync();
        }
    }
}
