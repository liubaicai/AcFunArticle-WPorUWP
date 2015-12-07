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
    public class InfoBindingModel : InfoResult.Fullarticle, INotifyPropertyChanged
    {
        public string SubHeader => 
            $"{User.Username} / 发布于{Time.getTime(ReleaseDate).ToString("yyyy年M月d(dddd) hh时mm分")} / 点击:{Views}  评论:{Comments}  收藏:{Stows}";

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
            [DataMember(Name = "fullArticle")]
            public InfoBindingModel FullArticle { get; set; }
        }

        public class Fullarticle
        {
            [DataMember(Name = "user")]
            public User User { get; set; }

            [DataMember(Name = "tags")]
            public string[] Tags { get; set; }

            [DataMember(Name = "txt")]
            public string Txt { get; set; }

            [DataMember(Name = "contentId")]
            public int ContentId { get; set; }

            [DataMember(Name = "releaseDate")]
            public long ReleaseDate { get; set; }

            [DataMember(Name = "isArticle")]
            public int IsArticle { get; set; }

            [DataMember(Name = "channelId")]
            public int ChannelId { get; set; }

            [DataMember(Name = "title")]
            public string Title { get; set; }

            [DataMember(Name = "cover")]
            public string Cover { get; set; }

            [DataMember(Name = "views")]
            public int Views { get; set; }

            [DataMember(Name = "stows")]
            public int Stows { get; set; }

            [DataMember(Name = "comments")]
            public int Comments { get; set; }

            [DataMember(Name = "isRecommend")]
            public int IsRecommend { get; set; }

            [DataMember(Name = "toplevel")]
            public int TopLevel { get; set; }

            [DataMember(Name = "viewOnly")]
            public int ViewOnly { get; set; }

            [DataMember(Name = "tudouDomain")]
            public int TudouDomain { get; set; }

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
