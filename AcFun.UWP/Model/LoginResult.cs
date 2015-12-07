using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AcFun.UWP.Model
{
    public class LoginResult
    {
        public class Rootobject
        {
            [DataMember(Name = "img")]
            public string Img { get; set; }

            [DataMember(Name = "success")]
            public bool Success { get; set; }

            [DataMember(Name = "username")]
            public string Username { get; set; }

            [DataMember(Name = "result")]
            public string Result { get; set; }
        }
    }
}
