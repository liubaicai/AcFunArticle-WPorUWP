using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;
using AcFun.UWP.Helper;

namespace AcFun.UWP.Model
{
    public class ChannelBindingModel : ChannelResult.List, INotifyPropertyChanged
    {
        public string TimeStr => Time.getTime(ReleaseDate).ToString("MM.dd HH:mm:ss");

        public string TitleStr => WebUtility.HtmlDecode(Title);

        public string DescriptionShow => WebUtility.HtmlDecode(Description.Replace("<br/>", ""));

        public Brush TitleForeground
        {
            get
            {
                var local =
                    IsolatedStorageFile.GetUserStoreForApplication();

                if (local.FileExists("CacheHtmlFolder\\" + ContentId))
                {
                    return new SolidColorBrush(Colors.Gray);
                }
                else
                {
                    return new SolidColorBrush(Color.FromArgb(255, 1, 135, 197));
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ChannelResult
    {
        public class Rootobject
        {
            [DataMember(Name = "success")]
            public bool Success { get; set; }

            [DataMember(Name = "msg")]
            public string Msg { get; set; }

            [DataMember(Name = "status")]
            public int Status { get; set; }

            [DataMember(Name = "data")]
            public Data Data { get; set; }
        }

        public class Data
        {
            [DataMember(Name = "page")]
            public Page Page { get; set; }
        }

        public class Page
        {
            [DataMember(Name = "pageNo")]
            public int PageNo { get; set; }

            [DataMember(Name = "pageSize")]
            public int PageSize { get; set; }

            [DataMember(Name = "totalCount")]
            public int TotalCount { get; set; }

            [DataMember(Name = "orderBy")]
            public int OrderBy { get; set; }

            [DataMember(Name = "list")]
            public ChannelBindingModel[] List { get; set; }
        }

        public class List
        {
            [DataMember(Name = "user")]
            public User User { get; set; }

            [DataMember(Name = "tags")]
            public object[] Tags { get; set; }

            [DataMember(Name = "cover")]
            public string Cover { get; set; }

            [DataMember(Name = "views")]
            public int Views { get; set; }

            [DataMember(Name = "stows")]
            public int Stows { get; set; }

            [DataMember(Name = "comments")]
            public int Comments { get; set; }

            [DataMember(Name = "title")]
            public string Title { get; set; }

            [DataMember(Name = "releaseDate")]
            public long ReleaseDate { get; set; }

            [DataMember(Name = "contentId")]
            public int ContentId { get; set; }

            [DataMember(Name = "isArticle")]
            public int IsArticle { get; set; }

            [DataMember(Name = "channelId")]
            public int ChannelId { get; set; }

            [DataMember(Name = "viewOnly")]
            public int ViewOnly { get; set; }

            [DataMember(Name = "toplevel")]
            public int TopLevel { get; set; }

            [DataMember(Name = "tudouDomain")]
            public int TudouDomain { get; set; }

            [DataMember(Name = "isRecommend")]
            public int IsRecommend { get; set; }

            [DataMember(Name = "description")]
            public string Description { get; set; }
        }

        public class User
        {
            [DataMember(Name = "userId")]
            public int UserId { get; set; }

            [DataMember(Name = "userImg")]
            public string UserImg { get; set; }

            [DataMember(Name = "username")]
            public string Username { get; set; }
        }
    }
}
