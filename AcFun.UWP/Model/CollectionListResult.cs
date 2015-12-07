using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace AcFun.UWP.Model
{
    public class CollectionBindingModel : CollectionListResult.Content, INotifyPropertyChanged
    {
        public string TimeStr => AcFun.UWP.Helper.Time.getTime(ReleaseDate).ToString("MM.dd HH:mm:ss");

        public Brush TitleForeground
        {
            get
            {
                var local =
                    IsolatedStorageFile.GetUserStoreForApplication();

                if (local.FileExists("CacheHtmlFolder\\" + Cid))
                {
                    return new SolidColorBrush(Colors.Gray);
                }
                else
                {
                    return new SolidColorBrush(Color.FromArgb(255, 1, 135, 197));
                }
            }
        }

        public string DescriptionShow => Description.Replace("<br/>", "");


        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class CollectionListResult
    {
        public class Rootobject
        {
            [DataMember(Name = "page")]
            public Page Page { get; set; }

            [DataMember(Name = "totalpage")]
            public int Totalpage { get; set; }

            [DataMember(Name = "totalcount")]
            public int Totalcount { get; set; }

            [DataMember(Name = "contents")]
            public CollectionBindingModel[] Contents { get; set; }

            [DataMember(Name = "success")]
            public bool Success { get; set; }
        }

        public class Page
        {
            [DataMember(Name = "pageNo")]
            public int PageNo { get; set; }

            [DataMember(Name = "pageSize")]
            public int PageSize { get; set; }

            [DataMember(Name = "totalCount")]
            public int TotalCount { get; set; }

            [DataMember(Name = "totalPage")]
            public int TotalPage { get; set; }

            [DataMember(Name = "prePage")]
            public int PrePage { get; set; }

            [DataMember(Name = "nextPage")]
            public int NextPage { get; set; }
        }

        public class Content
        {
            [DataMember(Name = "username")]
            public string Username { get; set; }

            [DataMember(Name = "userId")]
            public int UserId { get; set; }

            [DataMember(Name = "userImg")]
            public string UserImg { get; set; }

            [DataMember(Name = "avatar")]
            public string Avatar { get; set; }

            [DataMember(Name = "aid")]
            public int Aid { get; set; }

            [DataMember(Name = "cid")]
            public int Cid { get; set; }

            [DataMember(Name = "title")]
            public string Title { get; set; }

            [DataMember(Name = "titleImg")]
            public string TitleImg { get; set; }

            [DataMember(Name = "url")]
            public string Url { get; set; }

            [DataMember(Name = "releaseDate")]
            public long ReleaseDate { get; set; }

            [DataMember(Name = "description")]
            public string Description { get; set; }

            [DataMember(Name = "channelId")]
            public int ChannelId { get; set; }

            [DataMember(Name = "tags")]
            public string Tags { get; set; }

            [DataMember(Name = "contentClass")]
            public string ContentClass { get; set; }

            [DataMember(Name = "author")]
            public string Author { get; set; }

            [DataMember(Name = "allowDanmaku")]
            public int AllowDanmaku { get; set; }

            [DataMember(Name = "views")]
            public int Views { get; set; }

            [DataMember(Name = "stows")]
            public int Stows { get; set; }

            [DataMember(Name = "comments")]
            public int Comments { get; set; }

            [DataMember(Name = "score")]
            public int Score { get; set; }

            [DataMember(Name = "time")]
            public int Time { get; set; }

            [DataMember(Name = "isArticle")]
            public int IsArticle { get; set; }

            [DataMember(Name = "success")]
            public bool Success { get; set; }

            [DataMember(Name = "errorlog")]
            public string Errorlog { get; set; }

            [DataMember(Name = "sign")]
            public string Sign { get; set; }
        }
    }
}
