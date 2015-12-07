using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcFunBlue.DataModels
{
    public class ACListItem
    {
        public string title { get; set; }
        public string href { get; set; }
        public string dis { get; set; }
        public string time { get; set; }
        public string name { get; set; }
        public string beizhu { get; set; }

        public ACListItem()
        {
        }

        public ACListItem(string title, string url, string dis, string time, string name, string beizhu)
        {
            this.title = title;
            this.href = url;
            this.dis = dis;
            this.time = time;
            this.name = name;
            this.beizhu = beizhu;
        }

        public override string ToString()
        {
            return this.title;
        }
    }
}
