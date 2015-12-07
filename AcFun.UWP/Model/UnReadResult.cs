using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AcFun.UWP.Model
{
    class UnReadResult
    {
        public class Rootobject
        {
            [DataMember(Name = "special")]
            public object[] Special { get; set; }

            [DataMember(Name = "newPush")]
            public int NewPush { get; set; }

            [DataMember(Name = "newFollowed")]
            public int NewFollowed { get; set; }

            [DataMember(Name = "success")]
            public bool Success { get; set; }

            [DataMember(Name = "bangumi")]
            public object[] Bangumi { get; set; }

            [DataMember(Name = "unReadMail")]
            public int UnReadMail { get; set; }

            [DataMember(Name = "mention")]
            public int Mention { get; set; }

            [DataMember(Name = "setting")]
            public int Setting { get; set; }
        }
    }
}
