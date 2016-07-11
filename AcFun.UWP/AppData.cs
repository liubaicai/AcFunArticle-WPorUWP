using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcFun.UWP.Helper;

namespace AcFun.UWP
{
    public class AppData
    {
        #region URL地址 
        /// <summary>
        /// 列表地址(GET)
        /// http://api.aixifan.com/searches/channel?sort=4&pageNo=1&pageSize=20&channelIds=110
        /// </summary>
        public static string ContentChannelUrl => "http://api.aixifan.com/searches/channel?sort=4&pageSize=20&channelIds={0}&pageNo={1}&_=" + Time.getTimeSpan();

        /// <summary>
        /// 详情地址(GET)http://api.aixifan.com/contents/2883833
        /// </summary>
        public static string ContentInfoUrl => "http://api.aixifan.com/contents/{0}?_=" + Time.getTimeSpan();

        /// <summary>
        /// 评论地址(GET)
        /// </summary>
        public static string CommentListUrl => "http://www.acfun.tv/comment/content/web/list?contentId={0}&pageNo={1}&pageSize=20&_=" + Time.getTimeSpan();

        /// <summary>
        /// 发送评论地址(POST)
        /// text 评论内容
        /// contentId 文章id
        /// quoteId 引用楼层
        /// </summary>
        public static string SendCommentUrl => "http://m.acfun.tv/comment.aspx";

        /// <summary>
        /// 文章区搜索地址
        /// </summary>
        public static string SearchUrl => "http://search.acfun.tv/search?parentChannelId=63&pageSize=20&q={0}&pageNo={1}";

        /// <summary>
        /// 登录地址
        /// </summary>
        public static string LoginUrl => "http://m.acfun.tv/login.aspx?username={0}&password={1}";

        /// <summary>
        /// 注销地址
        /// </summary>
        public static string LogoutUrl => "http://www.acfun.tv/logout.aspx";

        /// <summary>
        /// 用户未读信息
        /// </summary>
        public static string UnReadUrl => "http://www.acfun.tv/member/unRead.aspx";

        /// <summary>
        /// 收藏
        /// </summary>
        public static string CollectUrl => "http://m.acfun.tv/member/collect.aspx";

        /// <summary>
        /// 是否收藏
        /// </summary>
        public static string IsCollectUrl => "http://m.acfun.tv/member/collect_up_exist.aspx?contentId={0}";

        /// <summary>
        /// 收藏列表
        /// </summary>
        public static string CollectionListUrl => "http://www.acfun.tv/member/collection.aspx?count=10&pageNo={0}&channelId=63";

        /// <summary>
        /// 签到
        /// </summary>
        public static string CheckInUrl => "http://www.acfun.tv/member/checkin.aspx";

        /// <summary>
        /// 提到我的
        /// </summary>
        public static string AtListUrl => "http://www.acfun.tv/comment/at/list?pageNo={0}&pageSize=10";

        #endregion
    }
}
