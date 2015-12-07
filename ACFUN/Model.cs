using System.Collections.Generic;
using System.ComponentModel;
using System.IO.IsolatedStorage;
using System.Windows;

namespace ACFUN
{
    public class ModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class ChickinResult
    {
        public string result { get; set; }
        public bool success { get; set; }
    }

    public static class AppMethod
    {
        public static Apps CreateApp(string logo, string msid, string title, string content)
        {
            var app = new Apps {logo = logo, msid = msid, title = title, content = content};
            return app;
        }
    }
    public class Apps
    {
        public string logo { get; set; }
        public string msid { get; set; }
        public string title { get; set; }
        public string content { get; set; }
    }

    public class Mention
    {
        public string title { get; set; }
        public string href { get; set; }
        public string dis { get; set; }
        public string time { get; set; }
        public string name { get; set; }
        public string beizhu { get; set; }

        public string content { get; set; }
        public string userName { get; set; }
        public string postDate { get; set; }
        public string userImg { get; set; }
        public int count { get; set; }

        public string pcontent { get; set; }
        public string puserName { get; set; }
        public string ppostDate { get; set; }
        public int pcount { get; set; }

        public string content_nohtml
        {
            get
            {
                return HtmlHelp.NoHTML(content);
            }
        }

        public string pcontent_nohtml
        {
            get
            {
                return (pcontent != null && pcontent != "") ? HtmlHelp.NoHTML(pcontent) : "";
            }
        }

        public Visibility isShowP
        {
            get
            {
                return (pcontent == null || pcontent == "") ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public string UserImg
        {
            get
            {
                if (StaticData.IsShowImage)
                {
                    return userImg;
                }
                else
                {
                    return "/Assets/avatar.jpg";
                }
            }
        }
    }

    public class tocollectionresult
    {
        public bool success { get; set; }
        public string result { get; set; }
    }

    public class unRead
    {
        public int newPush { get; set; }
        public int mention { get; set; }
        public int unReadMail { get; set; }
        public bool success { get; set; }
        public int newFollowed { get; set; }
    }

    public class commentuploadresult
    {
        public int status { get; set; }
        public bool success { get; set; }
        public string info { get; set; }
    }

    public class loginresult
    {
        public string username { get; set; }
        public string img { get; set; }
        public bool success { get; set; }
        public string result { get; set; }
    }

    public class commentdetail
    {
        public int cid { get; set; }
        public string content { get; set; }
        public string userName { get; set; }
        public int userID { get; set; }
        public string postDate { get; set; }
        public string userImg { get; set; }
        public string userClass { get; set; }
        public int quoteId { get; set; }
        public int count { get; set; }
        public int ups { get; set; }
        public int downs { get; set; }

        public int pcid { get; set; }
        public string pcontent { get; set; }
        public string puserName { get; set; }
        public int puserID { get; set; }
        public string ppostDate { get; set; }
        public int pquoteId { get; set; }
        public int pcount { get; set; }

        public string content_nohtml
        {
            get
            {
                return HtmlHelp.NoHTML(content);
            }
        }

        public string pcontent_nohtml
        {
            get
            {
                return (pcontent != null && pcontent!="")?HtmlHelp.NoHTML(pcontent):"";
            }
        }

        public Visibility isShowP
        {
            get
            {
                return (pcontent == null || pcontent == "") ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public string UserImg
        {
            get
            {
                if (StaticData.IsShowImage)
                {
                    return userImg;
                }
                else
                {
                    return "/Assets/avatar.jpg";
                }
            }
        }
    }

    public class comment
    {
        public int cid { get; set; }
        public string content { get; set; }
        public string userName { get; set; }
        public int userID { get; set; }
        public string postDate { get; set; }
        public string userImg { get; set; }
        public string userClass { get; set; }
        public int quoteId { get; set; }
        public int count { get; set; }
        public int ups { get; set; }
        public int downs { get; set; }
    }

    public class commentresult
    {
        public int page { get; set; }
        //public int nextPage { get; set; }
        //public int prePage { get; set; }
        public int totalCount { get; set; }
        public int totalPage { get; set; }
        public int pageSize { get; set; }
        public string desc { get; set; }
        public List<int> commentList { get; set; }
        public Dictionary<int,comment> commentContentArr { get; set; }
    }

    public class ACItem : ModelBase
    {
        public string title { get; set; }
        public string href { get; set; }
        public string dis { get; set; }
        public string time { get; set; }
        public string name { get; set; }
        public string beizhu { get; set; }

        public string timeStr => TimeFuc.getTime(long.Parse(time)).ToString("MM.dd HH:mm:ss");

        public double opacity
        {
            get
            {
                var local =
                    IsolatedStorageFile.GetUserStoreForApplication();

                if (local.FileExists("CacheHtmlFolder\\" + href.Remove(0, 3)))
                {
                    return 0.4;
                }
                else
                {
                    return 1;
                }
            }
        }

        public ACItem()
        {
        }

        public ACItem(string title, string url, string dis, string time, string name, string beizhu)
        {
            this.title = title;
            this.href = url;
            this.dis = dis;
            this.time = time;
            this.name = name;
            this.beizhu = beizhu;
        }
    }
}
