using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcFunBlue.DataModels
{
    public static class StaticData
    {
        public static string Cookie { get; set; }
        public static bool IsLogin { get; set; }
        public static string UserName { get; set; }
        public static string UserImg { get; set; }

        public static bool IsShowImage { get; set; }

        static StaticData()
        {
            IsLogin = false;

            //IsShowImage = Settings.GetValueOrDefault<bool>("IsShowImage", false);
        }

        #region api参数
        public static string versionapi = "http://acfun.liubaicai.com/version.html";

        public static string list1 = "http://www.acfun.com/v/list63/index_{0}.htm";
        public static string list2 = "http://www.acfun.com/v/list73/index_{0}.htm";
        public static string list3 = "http://www.acfun.com/v/list74/index_{0}.htm";
        public static string list4 = "http://www.acfun.com/v/list75/index_{0}.htm";

        public static string login = "http://www.acfun.com/login.aspx";
        public static string logout = "http://www.acfun.com/logout.aspx";

        public static string reg = "http://www.acfun.com/reg.aspx";

        public static string comment = "http://www.acfun.com/comment_list_json.aspx?contentId={0}&currentPage={1}";
        public static string postname = "sendComm()";
        public static string posttoken = "mimiko";
        public static string postcooldown = "5000";

        public static string unRead = "http://www.acfun.com/member/unRead.aspx";
        public static string collection = "http://www.acfun.com/api/member.aspx?name=collection&count=10&pageNo={0}&channelId=63";
        public static string mention = "http://www.acfun.com/api/member.aspx?name=mentions&pageNo={0}&pageSize=10";

        public static string tocollection = "http://www.acfun.com/member/collect.aspx";
        public static string iscollection = "http://www.acfun.com/member/collect_exist.aspx?cId={0}";
        #endregion
    }
}
