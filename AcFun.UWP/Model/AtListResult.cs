using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AcFun.UWP.Model
{
    public class AtListResult
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
            [DataMember(Name = "totalCount")]
            public int TotalCount { get; set; }

            [DataMember(Name = "pageSize")]
            public int PageSize { get; set; }

            [DataMember(Name = "pageNo")]
            public int PageNo { get; set; }

            [DataMember(Name = "commentList")]
            public CommentItem[] CommentList { get; set; }

            [DataMember(Name = "quoteCommentList")]
            public CommentItem[] QuoteCommentList { get; set; }
        }

        public class CommentItem
        {
            [DataMember(Name = "id")]
            public int Id { get; set; }

            [DataMember(Name = "quoteId")]
            public int QuoteId { get; set; }

            [DataMember(Name = "refCount")]
            public int RefCount { get; set; }

            [DataMember(Name = "content")]
            public string Content { get; set; }

            [DataMember(Name = "time")]
            public long Time { get; set; }

            [DataMember(Name = "userId")]
            public int UserId { get; set; }

            [DataMember(Name = "username")]
            public string Username { get; set; }

            [DataMember(Name = "avatar")]
            public string Avatar { get; set; }

            [DataMember(Name = "floor")]
            public int Floor { get; set; }

            [DataMember(Name = "deep")]
            public int Deep { get; set; }

            [DataMember(Name = "type")]
            public int Type { get; set; }

            [DataMember(Name = "contentId")]
            public int ContentId { get; set; }

            [DataMember(Name = "title")]
            public string Title { get; set; }

            [DataMember(Name = "isArticle")]
            public int IsArticle { get; set; }

            [DataMember(Name = "channelId")]
            public int ChannelId { get; set; }

            [DataMember(Name = "isAt")]
            public int IsAt { get; set; }

            [DataMember(Name = "nameRed")]
            public int NameRed { get; set; }

            [DataMember(Name = "avatarFrame")]
            public int AvatarFrame { get; set; }
        }
    }
}
