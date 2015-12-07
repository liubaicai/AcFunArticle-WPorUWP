using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using Liubaicai.Toolkit;

namespace ACWZ
{
    public partial class MentionsPage : PhoneApplicationPage, INotifyPropertyChanged
    {
        int page = 1;

        public ObservableCollection<Mention> MentionList { get; set; }
        public Dictionary<int, comment> commentContentArr { get; set; }

        public MentionsPage()
        {
            MentionList = new ObservableCollection<Mention>();
            commentContentArr = new Dictionary<int, comment>();
            InitializeComponent();
            DataContext = this;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.New)
            {
                MentionList.Clear();
                page = 1;
                await getList();
            }
        }

        private void EasyListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EasyListBox lb = sender as EasyListBox;
            if (lb.SelectedIndex != -1)
            {
                Mention item = lb.SelectedItem as Mention;
                StaticData.acitem = new ACItem(item.title, item.href, item.dis, item.time, item.name, item.beizhu);
                NavigationService.Navigate(new Uri("/ACWZ;component/CommentPage.xaml", UriKind.RelativeOrAbsolute));
            }
            lb.SelectedIndex = -1;
        }

        private void title_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton hb = sender as HyperlinkButton;
            string str = hb.Tag.ToString();
            foreach (var item in MentionList)
            {
                if (item.href == str)
                {
                    StaticData.acitem = new ACItem(item.title, item.href, item.dis, item.time, item.name, item.beizhu);
                    NavigationService.Navigate(new Uri("/ACWZ;component/DetailPage.xaml", UriKind.RelativeOrAbsolute));
                }
            }
        }

        bool islistbusy = false;
        private async void EasyListBox_StretchingBottom(object sender, EventArgs e)
        {
            if (!islistbusy && MentionList.Count > 0 && page > 0)
            {
                await getList();
            }
        }

        private async Task getList()
        {
            islistbusy = true;
            indicator.IsVisible = true;
            try
            {
                using (Stream stream = await new HttpHelp().Get(string.Format(StaticData.mention, page)))
                {
                    StreamReader sr = new StreamReader(stream);
                    JObject obj = JObject.Parse(sr.ReadToEnd());

                    if (!(bool)obj["success"])
                    {
                        if ((string)obj["message"] != null)
                        {
                            MessageBox.Show((string)obj["message"]);
                        }
                        indicator.IsVisible = false;
                        return;
                    }

                    int totalPage = (int)obj["totalPage"];
                    if (page < totalPage)
                    {
                        page++;
                    }
                    else
                    {
                        page = 0;
                    }

                    List<int> commentList = new List<int>();
                    List<ACItem> contentList = new List<ACItem>();
                    foreach (var commentid in (obj["commentList"] as JArray))
                    {
                        commentList.Add((int)commentid);
                    }
                    foreach (var content in (obj["contentList"] as JArray))
                    {
                        contentList.Add(new ACItem((string)content["title"], (string)content["url"], (string)content["comments"], TimeFuc.getTime((long)content["releaseDate"]).ToString(), (string)content["username"], (string)content["description"]));
                    }
                    foreach (var item in obj["commentContentArr"].Values())
                    {
                        if (!commentContentArr.ContainsKey(item.ToObject<comment>().cid))
                        {
                            commentContentArr.Add(item.ToObject<comment>().cid, item.ToObject<comment>());
                        }
                    }

                    if (commentList.Count > 0)
                    {
                        for (int i = 0; i < commentList.Count; i++)
                        {
                            try
                            {
                                Mention item = new Mention();

                                item.title = contentList[i].title;
                                item.href = contentList[i].href;
                                item.dis = contentList[i].dis;
                                item.time = contentList[i].time;
                                item.name = contentList[i].name;
                                item.beizhu = contentList[i].beizhu;

                                comment comment = commentContentArr[commentList[i]];

                                item.content = comment.content;
                                item.userName = comment.userName;
                                item.postDate = comment.postDate;
                                item.userImg = comment.userImg;
                                item.count = comment.count;

                                if (comment.quoteId != 0)
                                {
                                    comment pcomment = commentContentArr[comment.quoteId];
                                    item.pcontent = pcomment.content;
                                    item.puserName = pcomment.userName;
                                    item.ppostDate = pcomment.postDate;
                                    item.pcount = pcomment.count;
                                }

                                MentionList.Add(item);
                            }
                            catch { }
                        }
                    }
                }
            }
            catch { }
            islistbusy = false;
            indicator.IsVisible = false;
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
    }
}