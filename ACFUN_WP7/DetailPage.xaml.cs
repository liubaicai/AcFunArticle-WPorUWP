using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using HtmlAgilityPack;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Tasks;
using System.Windows.Media;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization.Json;

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
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                if (e.NavigationMode == NavigationMode.New)
                {
                    indicator.IsVisible = true;

                    title.Text = StaticData.acitem.title;
                    beizhu.Text = StaticData.acitem.beizhu;
                    name.Text = StaticData.acitem.name;
                    time.Text = StaticData.acitem.time;

                    using (var stream = await new HttpHelp().Get("http://www.acfun.tv" + StaticData.acitem.href))
                    {
                        var doc = new HtmlDocument();
                        doc.Load(stream);

                        try
                        {
                            var node = getListNode(doc);
                            if (node != null)
                            {
                                list.Clear();
                                foreach (var p in node.ChildNodes)
                                {
                                    if (p.Name.Equals("p"))
                                    {
                                        list.Add(p.InnerHtml);
                                    }
                                    else if (p.Name.Equals("div") && p.Id.Equals("ozoom"))
                                    {
                                        foreach (var f in p.ChildNodes)
                                        {
                                            if (f.Name.Equals("founder-content"))
                                            {
                                                foreach (var p0 in f.ChildNodes)
                                                {
                                                    if (p0.Name.Equals("p"))
                                                    {
                                                        list.Add(p0.InnerHtml);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                List<int> removelist = new List<int>();
                                for (int i = 0; i < list.Count; i++)
                                {
                                    if (list[i] == "&nbsp;" || list[i] == "<br>" || (list[i].StartsWith("<sohuadcode") && list[i].EndsWith("</sohuadcode>")))
                                    {
                                        removelist.Add(i);
                                    }
                                }
                                removelist.Reverse();
                                foreach (var i in removelist)
                                {
                                    list.RemoveAt(i);
                                }

                                if (list.Count == 0)
                                {
                                    content.Visibility = Visibility.Collapsed;
                                    browser.Visibility = Visibility.Visible;
                                    browser.NavigateToString(node.InnerHtml);
                                    //list.Add(HtmlHelp.NoHTML(node.InnerHtml));
                                }
                            }
                            else
                            {
                                content.Visibility = Visibility.Collapsed;
                                browser.Visibility = Visibility.Visible;
                                browser.NavigateToString(doc.DocumentNode.InnerHtml);
                            }
                        }
                        catch (Exception) { MessageBox.Show("出错啦！~"); }

                        setContent();

                        int flag = 0;
                        for (int i = count; i < 10 + count && i < contentlisttemp.Count; i++)
                        {
                            flag++;
                            contentlist.Add(contentlisttemp[i]);
                        }
                        count = count + flag;
                    }

                    using (var stream = await new HttpHelp().Get(string.Format(StaticData.iscollection, StaticData.acitem.href.Remove(0, 5))))
                    {
                        StreamReader sr = new StreamReader(stream);
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

        void setContent()
        {
            try
            {
                foreach (var strhtml in list)
                {
                    var regEx = new Regex(@"\<img\s(src\=""|[^\>]+?\ssrc\="")(?<img>[^""]+)"".*?>",
                        RegexOptions.IgnoreCase | RegexOptions.Multiline);

                    int nextOffset = 0;

                    foreach (Match match in regEx.Matches(strhtml))
                    {
                        if (match.Index == nextOffset)
                        {
                            nextOffset = match.Index + match.Length;
                            if (StaticData.IsShowImage)
                            {
                                contentlisttemp.Add(new DetailItem(match.Groups["img"].Value,null));
                            }
                            else
                            {
                                contentlisttemp.Add(new DetailItem("/Assets/noimg.jpg", null));
                            }
                        }
                        else if (match.Index > nextOffset)
                        {
                            contentlisttemp.Add(new DetailItem(null, strhtml));

                            nextOffset = match.Index + match.Length;

                            if (StaticData.IsShowImage)
                            {
                                contentlisttemp.Add(new DetailItem(match.Groups["img"].Value, null));
                            }
                            else
                            {
                                contentlisttemp.Add(new DetailItem("/Assets/noimg.jpg", null));
                            }
                        }
                    }

                    if (nextOffset < strhtml.Length)
                    {
                        contentlisttemp.Add(new DetailItem(null, strhtml));
                    }
                }
            }
            catch { MessageBox.Show("出错啦！~"); }
        }

        private HtmlNode getListNode(HtmlAgilityPack.HtmlDocument doc)
        {
            try
            {
                foreach (var html in doc.DocumentNode.ChildNodes)
                {
                    if (html.Name.Equals("html"))
                    {
                        foreach (var body in html.ChildNodes)
                        {
                            if (body.Name.Equals("body"))
                            {
                                foreach (var div1 in body.ChildNodes)
                                {
                                    if (div1.Name.Equals("div"))
                                    {
                                        if (div1.HasAttributes && div1.Attributes.Count > 0 && div1.Attributes[0].Value.Equals("area-player"))
                                        {
                                            return div1;
                                        }
                                        if (div1.HasChildNodes)
                                        {
                                            foreach (var div2 in div1.ChildNodes)
                                            {
                                                if (div2.HasAttributes && div2.Attributes.Count > 0 && div2.Attributes[0].Value.Equals("area-player"))
                                                {
                                                    return div2;
                                                }
                                                if (div2.HasChildNodes)
                                                {
                                                    foreach (var div3 in div2.ChildNodes)
                                                    {
                                                        if (div3.HasAttributes && div3.Attributes.Count > 0 && div3.Attributes[0].Value.Equals("area-player"))
                                                        {
                                                            return div3;
                                                        }
                                                        if (div3.HasChildNodes)
                                                        {
                                                            foreach (var div4 in div3.ChildNodes)
                                                            {
                                                                if (div4.HasAttributes && div4.Attributes.Count > 0 && div4.Attributes[0].Value.Equals("area-player"))
                                                                {
                                                                    return div4;
                                                                }
                                                                if (div4.HasChildNodes)
                                                                {
                                                                    foreach (var div5 in div4.ChildNodes)
                                                                    {
                                                                        if (div5.HasAttributes && div5.Attributes.Count > 0 && div5.Attributes[0].Value.Equals("area-player"))
                                                                        {
                                                                            return div5;
                                                                        }
                                                                        if (div5.HasChildNodes)
                                                                        {
                                                                            foreach (var div6 in div5.ChildNodes)
                                                                            {
                                                                                if (div6.HasAttributes && div6.Attributes.Count > 0 && div6.Attributes[0].Value.Equals("area-player"))
                                                                                {
                                                                                    return div6;
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch { }
            return null;
        }

        private void go_Click(object sender, EventArgs e)
        {
            WebBrowserTask task = new WebBrowserTask();
            task.Uri = new Uri("http://www.acfun.tv" + StaticData.acitem.href);
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
                    int flag = 0;
                    for (int i = count; i < 10 + count && i < contentlisttemp.Count; i++)
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
                HttpHelp httpHelp = new HttpHelp();
                httpHelp.parameters.Add("cId", StaticData.acitem.href.Remove(0, 5));
                httpHelp.parameters.Add("operate", "1");
                using (var stream = await httpHelp.Post(StaticData.tocollection))
                {
                    DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(tocollectionresult));
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