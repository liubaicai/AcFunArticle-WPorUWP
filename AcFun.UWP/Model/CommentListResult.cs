using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.UI;
using Windows.UI.Xaml.Media;
using AcFun.UWP.Helper;
using Newtonsoft.Json.Linq;

namespace AcFun.UWP.Model
{
    public class CommentBindingModel
    {
        public int CommentId { get; set; }

        public IDictionary<int, CommentBindingItemModel> CommentContentList { get; set; }
    }

    public class CommentBindingItemModel: CommentListResult.CommentContent
    {
        public int CommentId => int.Parse(Cid);

        public int ParentId => int.Parse(QuoteId);

        public string ContentShow => Html.NoHTML(WebUtility.HtmlDecode(Content));

        public Brush NameRedColor
        {
            get
            {
                if (NameRed>=1)
                {
                    return new SolidColorBrush(Color.FromArgb(255, 204, 102, 102));
                }
                else
                {
                    return new SolidColorBrush(Colors.Black);
                }
            }
        }
    }

    public class CommentListResult
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
            [DataMember(Name = "commentList")]
            public string[] CommentList { get; set; }

            [DataMember(Name = "totalPage")]
            public float TotalPage { get; set; }

            [DataMember(Name = "pageSize")]
            public float PageSize { get; set; }

            [DataMember(Name = "page")]
            public float Page { get; set; }

            [DataMember(Name = "totalCount")]
            public float TotalCount { get; set; }

            [DataMember(Name = "desc")]
            public bool Desc { get; set; }

            [DataMember(Name = "commentContentArr")]
            public JObject CommentContentArr { get; set; }

            public IDictionary<int, CommentBindingItemModel> CommentContentList
            {
                get
                {
                    var dic = new Dictionary<int, CommentBindingItemModel>();
                    foreach (var pair in CommentContentArr)
                    {
                        var item = pair.Value.ToObject<CommentBindingItemModel>();
                        if (!dic.ContainsKey(int.Parse(item.Cid)))
                        {
                            dic.Add(int.Parse(item.Cid), item);
                        }
                    }
                    return dic;
                }
            }
        }

        public class CommentContent
        {
            [DataMember(Name = "cid")]
            public string Cid { get; set; }

            [DataMember(Name = "quoteId")]
            public string QuoteId { get; set; }

            [DataMember(Name = "content")]
            public string Content { get; set; }

            [DataMember(Name = "postDate")]
            public string PostDate { get; set; }

            [DataMember(Name = "userID")]
            public float UserId { get; set; }

            [DataMember(Name = "userName")]
            public string UserName { get; set; }

            [DataMember(Name = "userImg")]
            public string UserImg { get; set; }

            [DataMember(Name = "count")]
            public float Count { get; set; }

            [DataMember(Name = "deep")]
            public float Deep { get; set; }

            [DataMember(Name = "refCount")]
            public float RefCount { get; set; }

            [DataMember(Name = "ups")]
            public float Ups { get; set; }

            [DataMember(Name = "downs")]
            public float Downs { get; set; }

            [DataMember(Name = "nameRed")]
            public float NameRed { get; set; }

            [DataMember(Name = "avatarFrame")]
            public float AvatarFrame { get; set; }
        }
    }
}
