using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AcFun.UWP.Model
{
    public class SearchResult
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
            public List[] List { get; set; }
        }

        public class List
        {
            [DataMember(Name = "contentId")]
            public string ContentId { get; set; }

            [DataMember(Name = "title")]
            public string Title { get; set; }

            [DataMember(Name = "description")]
            public string Description { get; set; }

            [DataMember(Name = "tags")]
            public string[] Tags { get; set; }

            [DataMember(Name = "channelId")]
            public int ChannelId { get; set; }

            [DataMember(Name = "parentChannelId")]
            public int ParentChannelId { get; set; }

            [DataMember(Name = "views")]
            public int Views { get; set; }

            [DataMember(Name = "stows")]
            public int Stows { get; set; }

            [DataMember(Name = "comments")]
            public int Comments { get; set; }

            [DataMember(Name = "userId")]
            public int UserId { get; set; }

            [DataMember(Name = "avatar")]
            public string Avatar { get; set; }

            [DataMember(Name = "titleImg")]
            public string TitleImg { get; set; }

            [DataMember(Name = "username")]
            public string Username { get; set; }

            [DataMember(Name = "releaseDate")]
            public long ReleaseDate { get; set; }

            [DataMember(Name = "recommend")]
            public bool Recommend { get; set; }

            [DataMember(Name = "status")]
            public int Status { get; set; }

            [DataMember(Name = "url")]
            public string Url { get; set; }

            [DataMember(Name = "channelIds")]
            public int[] ChannelIds { get; set; }

            [DataMember(Name = "time")]
            public int Time { get; set; }

            [DataMember(Name = "display")]
            public int Display { get; set; }

            [DataMember(Name = "contentSize")]
            public int ContentSize { get; set; }

            [DataMember(Name = "tudouDomain")]
            public int TudouDomain { get; set; }
        }
    }
}
