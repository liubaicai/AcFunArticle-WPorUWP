using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AcFun.UWP.Model
{
    public class IsCollectResult
    {
        public class Rootobject
        {
            [DataMember(Name = "data")]
            public Data Data { get; set; }

            [DataMember(Name = "success")]
            public bool Success { get; set; }

            [DataMember(Name = "info")]
            public string Info { get; set; }

            [DataMember(Name = "status")]
            public int Status { get; set; }
        }

        public class Data
        {
            [DataMember(Name = "tagList")]
            public Taglist[] TagList { get; set; }

            [DataMember(Name = "banana")]
            public int Banana { get; set; }

            [DataMember(Name = "up")]
            public bool Up { get; set; }

            [DataMember(Name = "follow")]
            public bool Follow { get; set; }

            [DataMember(Name = "collect")]
            public bool Collect { get; set; }
        }

        public class Taglist
        {
            [DataMember(Name = "tagName")]
            public string TagName { get; set; }

            [DataMember(Name = "tagId")]
            public int TagId { get; set; }

            [DataMember(Name = "refCount")]
            public int RefCount { get; set; }
        }
    }
}
