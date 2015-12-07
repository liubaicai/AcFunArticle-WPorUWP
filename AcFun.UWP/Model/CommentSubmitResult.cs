using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AcFun.UWP.Model
{
    public class CommentSubmitResult
    {
        public class Rootobject
        {
            [DataMember(Name = "success")]
            public bool Success { get; set; }

            [DataMember(Name = "commentId")]
            public int CommentId { get; set; }

            [DataMember(Name = "status")]
            public int Status { get; set; }

            [DataMember(Name = "info")]
            public string Info { get; set; }
        }
    }
}
