using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using Microsoft.Phone.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization.Json;
using Toast;
using UmengSocialSDK;
using UmengSocialSDK.Net.Request;
using UmengSocialSDK.UmEventArgs;
using System.Text;

namespace ACFUN
{
    public partial class DetailPage : PhoneApplicationPage, INotifyPropertyChanged
    {
        public ObservableCollection<DetailItem> contentlist { get; set; }
        List<DetailItem> contentlisttemp { get; set; }

        List<string> list;

        int count = 0;

        bool isCollection = false;

        public DetailPage()
        {
            contentlist = new ObservableCollection<DetailItem>();
            contentlisttemp = new List<DetailItem>();
            list = new List<string>();
            InitializeComponent();
            DataContext = this;
            this.Loaded += DetailPage_Loaded;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Debug.WriteLine("DetailPage:OnNavigatedTo");
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            Debug.WriteLine("DetailPage:OnNavigatingFrom");
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Debug.WriteLine("DetailPage:OnNavigatedFrom");
        }

        async void DetailPage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (list.Count == 0 && browser.Visibility == Visibility.Collapsed)
                {
                    indicator.IsVisible = true;

                    title.Text = StaticData.acitem.title;
                    beizhu.Text = StaticData.acitem.beizhu;
                    name.Text = StaticData.acitem.name;
                    time.Text = StaticData.acitem.timeStr;

                    using (var stream = await new CacheFile().GetCachedHtml(StaticData.acitem.href))
                    {
                        var doc = new HtmlDocument();
                        doc.Load(stream, Encoding.UTF8);

                        var node = doc.DocumentNode;
                        list.Clear();
                        var listcontent = GetHtmlContent(node.InnerHtml);
                        foreach (var detailItem in listcontent)
                        {
                            string[] arr = Regex.Split(detailItem, @"\r\n", RegexOptions.IgnoreCase);
                            foreach (var s in arr)
                            {
                                if (!string.IsNullOrWhiteSpace(s))
                                {
                                    int tmpLenght = 1200;
                                    string tmpStr = s.Trim();
                                    for (int i = 0; i <= tmpStr.Length / tmpLenght; i++)
                                    {
                                        if ((i + 1) * tmpLenght > tmpStr.Length)
                                        {
                                            list.Add(tmpStr.Substring(i * tmpLenght, tmpStr.Length % tmpLenght));
                                        }
                                        else
                                        {
                                            list.Add(tmpStr.Substring(i * tmpLenght, tmpLenght));
                                        }
                                    }
                                }
                            }
                        }

                        foreach (var i in list)
                        {
                            if (i.StartsWith("http://") && !i.EndsWith(".gif"))
                            {
                                contentlisttemp.Add(new DetailItem(i, null));
                            }
                            else
                            {
                                contentlisttemp.Add(new DetailItem(null, i));
                            }
                        }

                        var flag = 0;
                        string tmp = string.Empty;
                        var removelist1 = new List<int>();
                        for (int i = 0; i < contentlisttemp.Count; i++)
                        {
                            if (!string.IsNullOrEmpty(contentlisttemp[i].txt))
                            {
                                if (contentlisttemp[i].txt == tmp)
                                {
                                    removelist1.Add(i);
                                }
                                tmp = contentlisttemp[i].txt;
                            }
                        }
                        removelist1.Reverse();
                        foreach (var i in removelist1)
                        {
                            contentlisttemp.RemoveAt(i);
                        }
                        for (var i = count; i < 10 + count && i < contentlisttemp.Count; i++)
                        {
                            flag++;
                            contentlist.Add(contentlisttemp[i]);
                        }
                        count = count + flag;
                    }

                    using (var stream = await new HttpHelp().Get(string.Format(StaticData.iscollection, StaticData.acitem.href.Remove(0, 5))))
                    {
                        var sr = new StreamReader(stream);
                        if (sr.ReadToEnd().Contains("true"))
                        {
                            isCollection = true;
                        }
                    }
                }
            }
            catch (Exception) { MessageBox.Show("出错啦！多半是网络&服务器问题啦~"); }
            indicator.IsVisible = false;
        }

        List<string> GetHtmlContent(string strhtml)
        {
            var list = new List<string>();
            var regEx = new Regex(@"\<img\s(src\=""|[^\>]+?\ssrc\="")(?<img>[^""]+)"".*?>",
                RegexOptions.IgnoreCase | RegexOptions.Multiline);

            var nextOffset = 0;

            foreach (Match match in regEx.Matches(strhtml))
            {
                if (match.Index == nextOffset)
                {
                    nextOffset = match.Index + match.Length;
                    list.Add(match.Groups["img"].Value);
                }
                else if (match.Index > nextOffset)
                {
                    list.Add(HtmlHelp.NoHTMLContent(strhtml.Substring(nextOffset, match.Index - nextOffset)));

                    nextOffset = match.Index + match.Length;

                    list.Add(match.Groups["img"].Value);
                }
            }

            if (nextOffset < strhtml.Length)
            {
                list.Add(HtmlHelp.NoHTMLContent(strhtml.Substring(nextOffset)));
            }

            return list;
        }

        private void go_Click(object sender, EventArgs e)
        {
            var task = new WebBrowserTask();
            task.Uri = new Uri("http://m.acfun.tv/v/?" + StaticData.acitem.href.Remove(0, 3).Replace("ac", "ac="));
            task.Show();
        }

        private void viewcomment_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/CommentPage.xaml", UriKind.RelativeOrAbsolute));
        }

        #region 绑定接口实现
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        bool islistbusy = false;
        private void content_LayoutUpdated(object sender, EventArgs e)
        {
            if (!islistbusy && contentlist.Count > 0 && count < contentlisttemp.Count)
            {
                try
                {
                    islistbusy = true;
                    var flag = 0;
                    for (var i = count; i < 10 + count && i < contentlisttemp.Count; i++)
                    {
                        flag++;
                        contentlist.Add(contentlisttemp[i]);
                    }
                    count = count + flag;
                    NotifyPropertyChanged("contentlist");
                }
                catch { }
            }
            islistbusy = false;
        }

        private async void savewz_Click(object sender, EventArgs e)
        {
            try
            {
                if (!StaticData.IsLogin)
                {
                    MessageBox.Show("请先登录");
                    return;
                }
                if (isCollection)
                {
                    MessageBox.Show("已经收藏,可以到我的收藏中删除");
                    return;
                }
                var httpHelp = new HttpHelp();
                httpHelp.parameters.Add("cId", StaticData.acitem.href.Remove(0, 5));
                httpHelp.parameters.Add("operate", "1");
                using (var stream = await httpHelp.Post(StaticData.tocollection))
                {
                    var json = new DataContractJsonSerializer(typeof(tocollectionresult));
                    var result = json.ReadObject(stream) as tocollectionresult;
                    if (result.success)
                    {
                        MessageBox.Show("收藏成功");
                        isCollection = true;
                    }
                    else
                    {
                        MessageBox.Show("收藏失败");
                    }
                }
            }
            catch { }
        }

        private void Image_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            try
            {
                var img = sender as Image;
                if (!img.Tag.ToString().StartsWith("/"))
                {
                    NavigationService.Navigate(new Uri("/ImageView.xaml?url=" + HttpUtility.UrlEncode(img.Tag.ToString()), UriKind.RelativeOrAbsolute));
                }
            }
            catch (Exception)
            {
                new ToastPrompt().Show("图片出了点问题~");
            }
        }

        private void sharewz_Click(object sender, EventArgs e)
        {
            var shareData = new ShareData();
            shareData.Content = string.Format("《{0}》，UP主:{1}，原文:{2}。", StaticData.acitem.title, StaticData.acitem.name, "http://www.acfun.tv" + StaticData.acitem.href);
            var option = new ShareOption();
            option.ShareCompleted += args =>
            {
                if (args.StatusCode == Status.Successed)
                {
                    new ToastPrompt().Show("分享成功");
                }
                else
                {
                    new ToastPrompt().Show(args.Error.Message);
                }
            };
            UmengSocial.Share(App.UmengKey, shareData, null, this, option);
        }

        private void link_Click(object sender, EventArgs e)
        {
            Clipboard.SetText("http://www.acfun.tv" + StaticData.acitem.href);
            new ToastPrompt().Show("复制成功");
        }
    }

    public class DetailItem
    {
        public DetailItem(string img, string txt)
        {
            this.img = img;
            this.txt = txt;
        }

        public string img { get; set; }
        public string txt { get; set; }

        public string ShowImg
        {
            get
            {
                if (StaticData.IsShowImage)
                {
                    return img;
                }
                else
                {
                    return "/Assets/noimg.jpg";
                }
            }
        }

        public string nohtmltxt
        {
            get { return string.IsNullOrEmpty(txt) ? "" : HtmlHelp.NoHTML(txt); }
        }

        public Visibility imgshow
        {
            get
            {
                return img == null ? Visibility.Collapsed : Visibility.Visible;
            }
        }
        public Visibility txtshow
        {
            get
            {
                return txt == null ? Visibility.Collapsed : Visibility.Visible;
            }
        }
    }
}