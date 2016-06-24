namespace ACFUN
{
    public static class StaticData
    {
        public static ACItem acitem { get; set; }

        public static string Cookie { get; set; }
        public static bool IsLogin { get; set; }
        public static string UserName { get; set; }
        public static string UserImg { get; set; }

        public static bool IsShowImage
        {
            get { return Settings.GetValueOrDefault("IsShowImage", false); }
            set { Settings.AddOrUpdateValue("IsShowImage", value); }
        }

        static StaticData()
        {
            IsLogin = false;
            acitem = new ACItem();
        }

        #region api参数
        public static string versionapi = "http://acfun.liubaicai.com/version.html";

        //http://api.aixifan.com/searches/channel?sort=4&pageNo=1&pageSize=20&channelIds=110
        public static string list1 = "http://api.aixifan.com/searches/channel?sort=4&pageSize=20&channelIds=110&pageNo={0}&_={1}";
        public static string list2 = "http://api.aixifan.com/searches/channel?sort=4&pageSize=20&channelIds=73&pageNo={0}&_={1}";
        public static string list3 = "http://api.aixifan.com/searches/channel?sort=4&pageSize=20&channelIds=74&pageNo={0}&_={1}";
        public static string list4 = "http://api.aixifan.com/searches/channel?sort=4&pageSize=20&channelIds=75&pageNo={0}&_={1}";

        public static string login = "http://m.acfun.tv/login.aspx?username={0}&password={1}";
        public static string logout = "http://www.acfun.tv/logout.aspx";

        public static string reg = "http://www.acfun.tv/reg.aspx";

        public static string comment = "http://www.acfun.tv/comment_list_json.aspx?contentId={0}&currentPage={1}";
        public static string postname = "sendComm()";
        public static string posttoken = "mimiko";
        public static string postcooldown = "5000";

        public static string unRead = "http://www.acfun.tv/member/unRead.aspx";
        public static string collection = "http://www.acfun.tv/api/member.aspx?name=collection&count=10&pageNo={0}&channelId=63";
        public static string mention = "http://www.acfun.tv/api/member.aspx?name=mentions&pageNo={0}&pageSize=10";
        public static string checkin = "http://www.acfun.tv/member/checkin.aspx";

        public static string tocollection = "http://www.acfun.tv/member/collect.aspx";
        public static string iscollection = "http://www.acfun.tv/member/collect_exist.aspx?cId={0}";

        public static string search = "http://api.acfun.tv/search?query={0}&exact=1&channelIds=63&orderId=0&orderBy=1&pageNo={1}&pageSize=10 ";
        #endregion
    }
}
