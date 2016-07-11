using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using AcFun.UWP.Helper;

namespace AcFun.UWP.Model
{
    public class InfoBindingModel : InfoResult.Data, INotifyPropertyChanged
    {
        public string SubHeader =>
            $"{owner.name} / 发布于{Time.getTime(releaseDate).ToString("yyyy年M月d(dddd) hh时mm分")} / 点击:{visit.views}  评论:{visit.comments}  收藏:{visit.stows}";

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class InfoResult
    {
        public class Rootobject
        {
            public int code { get; set; }
            public InfoBindingModel data { get; set; }
            public string message { get; set; }
        }

        public class Data
        {
            public Article article { get; set; }
            public int channelId { get; set; }
            public int contentId { get; set; }
            public string cover { get; set; }
            public string description { get; set; }
            public int display { get; set; }
            public int isArticle { get; set; }
            public int isRecommend { get; set; }
            public Owner owner { get; set; }
            public long releaseDate { get; set; }
            public int status { get; set; }
            public string[] tags { get; set; }
            public string title { get; set; }
            public int topLevel { get; set; }
            public long updatedAt { get; set; }
            public int viewOnly { get; set; }
            public Visit visit { get; set; }
        }

        public class Article
        {
            public string content { get; set; }
        }

        public class Owner
        {
            public string avatar { get; set; }
            public int id { get; set; }
            public string name { get; set; }
        }

        public class Visit
        {
            public int comments { get; set; }
            public int danmakuSize { get; set; }
            public int goldBanana { get; set; }
            public int score { get; set; }
            public int stows { get; set; }
            public int ups { get; set; }
            public int views { get; set; }
        }
    }


}
