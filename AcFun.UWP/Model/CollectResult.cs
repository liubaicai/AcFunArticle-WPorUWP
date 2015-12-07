using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AcFun.UWP.Model
{
    public class CommonResult
    {
        public class Rootobject
        {
            [DataMember(Name = "result")]
            public string Result { get; set; }

            [DataMember(Name = "success")]
            public bool Success { get; set; }

            [DataMember(Name = "status")]
            public int Status { get; set; }

            [DataMember(Name = "info")]
            public string Info { get; set; }
        }

    }
}
