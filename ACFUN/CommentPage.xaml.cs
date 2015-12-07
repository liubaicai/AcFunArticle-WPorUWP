using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using System.IO;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using GestureEventArgs = System.Windows.Input.GestureEventArgs;

namespace ACFUN
{
    public partial class CommentPage : PhoneApplicationPage, INotifyPropertyChanged
    {
        string commenturl = StaticData.comment;
        commentresult result;

        int page = 1;
        int count = 0;

        int replayid = 0;

        public ObservableCollection<commentdetail> commentList { get; set; }
        public List<commentdetail> commentListTemp { get; set; }

        public CommentPage()
        {
            FloorsList = new ObservableCollection<commentdetail>();
            commentList = new ObservableCollection<commentdetail>();
            commentListTemp = new List<commentdetail>();

            result = new commentresult();
            result.commentList = new List<int>();
            result.commentContentArr = new Dictionary<int, comment>();

            InitializeComponent();
            DataContext = this;

            this.Loaded += CommentPage_Loaded;
        }

        System.Windows.Controls.Image image;
        void CommentPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!Settings.GetValueOrDefault<bool>("commentnotice", false))
            {
                Settings.AddOrUpdateValue("commentnotice", true);
                image = new System.Windows.Controls.Image();
                image.Source = new BitmapImage(new Uri("/Assets/commentnotice.jpg", UriKind.RelativeOrAbsolute));
                image.Stretch = Stretch.Fill;
                image.Tap += image_Tap;
                LayoutRoot.Children.Add(image);
            }
        }

        void image_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            try
            {
                if (LayoutRoot.Children.Contains(image))
                    LayoutRoot.Children.Remove(image);
            }
            catch { }
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.New)
            {
                try
                {
                    indicator.IsVisible = true;
                    title.Text = StaticData.acitem.title;
                    var id = StaticData.acitem.href.Substring(5, StaticData.acitem.href.Length - 5);
                    using (var stream = await new HttpHelp().Get(string.Format(commenturl, id, page)))
                    {
                        try
                        {
                            var sr = new StreamReader(stream);
                            var obj = JObject.Parse(sr.ReadToEnd())["data"];

                            result.page = (int)obj["page"];
                            result.totalCount = (int)obj["totalCount"];
                            result.totalPage = (int)obj["totalPage"];
                            result.pageSize = (int)obj["pageSize"];
                            result.desc = (string)obj["desc"];
                            foreach (var commentid in (obj["commentList"] as JArray))
                            {
                                result.commentList.Add((int)commentid);
                            }
                            foreach (var item in obj["commentContentArr"].Values())
                            {
                                result.commentContentArr.Add(item.ToObject<comment>().cid, item.ToObject<comment>());
                            }

                            if (page < result.totalPage)
                            {
                                page++;
                            }
                            else
                            {
                                page = 0;
                            }
                        }
                        catch (Exception) { }
                    }
                    commentList.Clear();
                    commentListTemp.Clear();
                    foreach (var commentid in result.commentList)
                    {
                        try
                        {
                            commentListTemp.Add(getdetail(commentid));
                        }
                        catch (Exception) { }
                    }
                    var flag = 0;
                    for (var i = count; i < 10 + count && i < commentListTemp.Count; i++)
                    {
                        flag++;
                        commentList.Add(commentListTemp[i]);
                    }
                    count = count + flag;

                    NotifyPropertyChanged("commentList");
                }
                catch (Exception) { }
                indicator.IsVisible = false;
            }
        }

        private async void flush()
        {
            try
            {
                indicator.IsVisible = true;

                result.commentList.Clear();
                result.commentContentArr.Clear();
                commentList.Clear();
                commentListTemp.Clear(); 
                commenturl = StaticData.comment;
                page = 1;
                count = 0;
                replayid = 0;

                title.Text = StaticData.acitem.title;
                var id = StaticData.acitem.href.Substring(5, StaticData.acitem.href.Length - 5);
                using (var stream = await new HttpHelp().Get(string.Format(commenturl, id, page)))
                {
                    try
                    {
                        var sr = new StreamReader(stream);
                        var obj = JObject.Parse(sr.ReadToEnd())["data"];

                        result.page = (int)obj["page"];
                        result.totalCount = (int)obj["totalCount"];
                        result.totalPage = (int)obj["totalPage"];
                        result.pageSize = (int)obj["pageSize"];
                        result.desc = (string)obj["desc"];
                        foreach (var commentid in (obj["commentList"] as JArray))
                        {
                            result.commentList.Add((int)commentid);
                        }
                        foreach (var item in obj["commentContentArr"].Values())
                        {
                            result.commentContentArr.Add(item.ToObject<comment>().cid, item.ToObject<comment>());
                        }

                        if (page < result.totalPage)
                        {
                            page++;
                        }
                        else
                        {
                            page = 0;
                        }
                    }
                    catch (Exception) { }
                }
                commentList.Clear();
                commentListTemp.Clear();
                foreach (var commentid in result.commentList)
                {
                    try
                    {
                        commentListTemp.Add(getdetail(commentid));
                    }
                    catch (Exception) { }
                }
                var flag = 0;
                for (var i = count; i < 10 + count && i < commentListTemp.Count; i++)
                {
                    flag++;
                    commentList.Add(commentListTemp[i]);
                }
                count = count + flag;

                NotifyPropertyChanged("commentList");
            }
            catch (Exception) { }
            indicator.IsVisible = false;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var listBox = sender as ListBox;
                if (listBox.SelectedIndex == -1)
                {
                    return;
                }
                var item = listBox.SelectedItem as commentdetail;
                replayid = item.cid;
                floor.Text = item.count + "";
            }
            catch(Exception) { }
        }

        bool islistbusy = false;
        private async void listBox_StretchingBottom(object sender, EventArgs e)
        {
            if (!islistbusy && commentList.Count > 0 && (page > 0 || count < commentListTemp.Count))
            {
                try
                {
                    indicator.IsVisible = true;
                    islistbusy = true;

                    if (count < commentListTemp.Count)
                    {
                        var flag = 0;
                        for (var i = count; i < 10 + count && i < commentListTemp.Count; i++)
                        {
                            flag++;
                            commentList.Add(commentListTemp[i]);
                        }
                        count = count + flag;
                    }
                    else
                    {
                        title.Text = StaticData.acitem.title;
                        var id = StaticData.acitem.href.Substring(5, StaticData.acitem.href.Length - 5);
                        using (var stream = await new HttpHelp().Get(string.Format(commenturl, id, page)))
                        {
                            try
                            {
                                var sr = new StreamReader(stream);
                                var obj = JObject.Parse(sr.ReadToEnd())["data"];

                                result.page = (int)obj["page"];
                                result.totalCount = (int)obj["totalCount"];
                                result.totalPage = (int)obj["totalPage"];
                                result.pageSize = (int)obj["pageSize"];
                                result.desc = (string)obj["desc"];
                                foreach (var commentid in (obj["commentList"] as JArray))
                                {
                                    result.commentList.Add((int)commentid);
                                }
                                foreach (var item in obj["commentContentArr"].Values())
                                {
                                    if (!result.commentContentArr.ContainsKey(item.ToObject<comment>().cid))
                                    {
                                        result.commentContentArr.Add(item.ToObject<comment>().cid, item.ToObject<comment>());
                                    }
                                }

                                if (page < result.totalPage)
                                {
                                    page++;
                                }
                                else
                                {
                                    page = 0;
                                }
                            }
                            catch (Exception) { }
                        }
                        var flag = 0;
                        for (var i = count; i < 10 + count && i < commentListTemp.Count; i++)
                        {
                            flag++;
                            commentList.Add(commentListTemp[i]);
                        }
                        count = count + flag;
                        commentListTemp.Clear();
                        foreach (var commentid in result.commentList)
                        {
                            try
                            {
                                commentListTemp.Add(getdetail(commentid));
                            }
                            catch (Exception) { }
                        }
                        flag = 0;
                        for (var i = count; i < 10 + count && i < commentListTemp.Count; i++)
                        {
                            flag++;
                            commentList.Add(commentListTemp[i]);
                        }
                        count = count + flag;
                    }

                    NotifyPropertyChanged("commentList");
                }
                catch(Exception) { }
            }
            indicator.IsVisible = false;
            islistbusy = false;
        }

        private commentdetail getdetail(int commentid)
        {
            try
            {
                var detail = new commentdetail();
                var comment = result.commentContentArr[commentid];
                detail.cid = comment.cid; ;
                detail.content = comment.content;
                detail.userName = comment.userName;
                detail.userID = comment.userID;
                detail.postDate = comment.postDate;
                detail.userImg = comment.userImg;
                detail.userClass = comment.userClass;
                detail.quoteId = comment.quoteId;
                detail.count = comment.count;
                detail.ups = comment.ups;
                detail.downs = comment.downs;
                if (comment.quoteId != 0)
                {
                    var pcomment = result.commentContentArr[comment.quoteId];
                    detail.pcid = pcomment.cid;
                    detail.pcontent = pcomment.content;
                    detail.puserName = pcomment.userName;
                    detail.puserID = pcomment.userID;
                    detail.ppostDate = pcomment.postDate;
                    detail.pquoteId = pcomment.quoteId;
                    detail.pcount = pcomment.count;
                }
                return detail;
            }
            catch { return null; }
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

        private void ClearFloor_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            replayid = 0;
            floor.Text = "无引用";
            listBox.SelectedIndex = -1;
        }

        private async void send_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!StaticData.IsLogin)
                {
                    MessageBox.Show("请先登录");
                    return;
                }
                if (content.Text != null && content.Text != "")
                {
                    if (noemoji(content.Text).Length<5)
                    {
                        MessageBox.Show("字数这么少~!");
                        return;
                    }
                    indicator.IsVisible = true;
                    var httpHelp = new HttpHelp();
                    //httpHelp.parameters.Add("name", StaticData.postname);
                    //httpHelp.parameters.Add("token", StaticData.posttoken);
                    //httpHelp.parameters.Add("text", content.Text + Environment.NewLine + "--WP文章区");
                    //httpHelp.parameters.Add("quoteId", replayid > 0 ? (replayid + "") : "0");
                    //httpHelp.parameters.Add("contentId", StaticData.acitem.href.Substring(5, StaticData.acitem.href.Length - 5));
                    //httpHelp.parameters.Add("cooldown", StaticData.postcooldown);
                    //httpHelp.parameters.Add("quoteName", "");
                    using (var stream = await httpHelp.PostUrl("http://www.acfun.tv/comment.aspx?" 
                        + "name=" + StaticData.postname
                        + "&token=" + StaticData.posttoken
                        + "&text=" + content.Text + Environment.NewLine + "--WP文章区"
                        + "&quoteId=" + (replayid > 0 ? (replayid + "") : "0")
                        + "&contentId=" + StaticData.acitem.href.Substring(5, StaticData.acitem.href.Length - 5)
                        + "&cooldown=" + StaticData.postcooldown))
                    {
                        if (stream != null)
                        {
                            //StreamReader sr = new StreamReader(stream);
                            //Debug.WriteLine(sr.ReadToEnd());
                            //stream.Position = 0;
                            var json = new DataContractJsonSerializer(typeof(commentuploadresult));
                            var result = json.ReadObject(stream) as commentuploadresult;
                            if (result.success)
                            {
                                content.Text = "";
                                replayid = 0;
                                floor.Text = "无引用";

                                flush();
                            }
                            else if (result.info != null && result.info != "")
                            {
                                MessageBox.Show(result.info);
                            }
                            else
                            {
                                MessageBox.Show("回复失败");
                            }
                        }
                        else
                        {
                            MessageBox.Show("未知异常,多半是网络问题啦~");
                        }
                    }

                    indicator.IsVisible = false;
                }
            }
            catch { }
        }

        private string noemoji(string str)
        {
            var result = "";
            if (str != null && str != "")
            {
                var nextOffset = 0;
                var regEx = new Regex(@"(\[emot=(?<emoji>.*?)/\])", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                foreach (Match match in regEx.Matches(str))
                {
                    if (match.Index == nextOffset)
                    {
                        nextOffset = match.Index + match.Length;
                    }
                    else if (match.Index > nextOffset)
                    {
                        result += str.Substring(nextOffset, match.Index - nextOffset);
                        nextOffset = match.Index + match.Length;
                    }
                }
                if (nextOffset < str.Length)
                {
                    result += str.Substring(nextOffset);
                }
            }
            return result;
        }

        private void emoji_Click(object sender, RoutedEventArgs e)
        {
            emojibox.Open(this);
        }

        private void content_GotFocus(object sender, RoutedEventArgs e)
        {
            sendbox.TranslateY = 0;
        }

        private void shouqi_Click(object sender, RoutedEventArgs e)
        {
            sendbox.TranslateY = 70;
        }

        private void emojibox_EmojiSelectCompleted(object sender, Controls.EmojiSelectEventArgs e)
        {
            content.Text += e.content;
            content.SelectionStart = content.Text.Length;
        }

        private void menuitem_Click(object sender, RoutedEventArgs e)
        {
            var menu = sender as MenuItem;
            var item = menu.DataContext as commentdetail;
            SetFloorData(item);
            floorbox.Open(this);
        }

        private void ListItem_Hold(object sender, GestureEventArgs e)
        {
            var item = (sender as Grid).Tag as commentdetail;
            SetFloorData(item);
            floorbox.Open(this);
        }

        private void SetFloorData(commentdetail item)
        {
            try
            {
                if (FloorsList.Count > 0)
                {
                    floorbox.Reset(FloorsList[0]);
                }
                FloorsList.Clear();
                var list = new List<commentdetail>();
                GetParent(ref list, item.cid);
                list.Reverse();
                foreach (var comment in list)
                {
                    FloorsList.Add(comment);
                }
            }
            catch { }
        }

        private void GetParent(ref List<commentdetail> list,int cid)
        {
            try
            {
                list.Add(getdetail(cid));
                if (result.commentContentArr[cid].quoteId > 0)
                {
                    GetParent(ref list, result.commentContentArr[cid].quoteId);
                }
            }
            catch { }
        }

        public ObservableCollection<commentdetail> FloorsList { get; set; }
    }
}