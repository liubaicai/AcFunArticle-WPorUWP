using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcFun.UWP.Helper;

namespace AcFunJX.UWP.Util
{
    public class CommUtils
    {
        static CommUtils()
        {
            if (avatars == null)
            {
                avatars = new List<string>();
                for (int i = 1; i <= 54; i++)
                {
                    avatars.Add("/Assets/Emoji/ac/" + i.ToString("00") + ".png");
                }
            }
        }

        private static List<string> avatars = null;

        public static void getTableItemArray(ref List<ListItem> good, ref List<ListItem> bad)
        {
            var date = DateTime.Now;
            long seed = 37621 * date.Year + 539 * date.Month + date.Day;

            List<ResultItem> list = new List<ResultItem>()
            {
                new ResultItem() {name = "看AV",good = "释放压力，重铸自我",bad = "会被家人撞到"},
                new ResultItem() {name = "组模型",good = "今天的喷漆会很完美",bad = "精神不集中板件被剪断了"},
                new ResultItem() {name = "投稿情感区",good = "问题圆满解决",bad = "会被当事人发现"},
                new ResultItem() {name = "逛匿名版",good = "今天也要兵库北",bad = "看到丧尸在晒妹"},
                new ResultItem() {name = "和女神聊天",good = "女神好感度上升",bad = "我去洗澡了，呵呵"},
                new ResultItem() {name = "啪啪啪",good = "啪啪啪啪啪啪啪",bad = "会卡在里面"},
                new ResultItem() {name = "熬夜",good = "夜间的效率更高",bad = "明天有很重要的事"},
                new ResultItem() {name = "锻炼",good = "八分钟给你比利般的身材",bad = "会拉伤肌肉"},
                new ResultItem() {name = "散步",good = "遇到妹子主动搭讪",bad = "走路会踩到水坑"},
                new ResultItem() {name = "打排位赛",good = "遇到大腿上分500",bad = "我方三人挂机"},
                new ResultItem() {name = "汇报工作",good = "被夸奖工作认真",bad = "上班偷玩游戏被扣工资"},
                new ResultItem() {name = "抚摸猫咪",good = "才不是特意蹭你的呢",bad = "死开！愚蠢的人类"},
                new ResultItem() {name = "遛狗",good = "遇见女神遛狗搭讪",bad = "狗狗随地大小便被罚款"},
                new ResultItem() {name = "烹饪",good = "黑暗料理界就由我来打败",bad = "难道这就是……仰望星空派？"},
                new ResultItem() {name = "告白",good = "其实我也喜欢你好久了",bad = "对不起，你是一个好人"},
                new ResultItem() {name = "求站内信",good = "最新种子入手",bad = "收到有码葫芦娃"},
                new ResultItem() {name = "追新番",good = "完结之前我绝不会死",bad = "会被剧透"},
                new ResultItem() {name = "打卡日常",good = "怒回首页",bad = "会被老板发现"},
                new ResultItem() {name = "下副本",good = "配合默契一次通过",bad = "会被灭到散团"},
                new ResultItem() {name = "抢沙发",good = "沙发入手弹无虚发",bad = "会被挂起来羞耻play"},
                new ResultItem() {name = "网购",good = "商品大减价",bad = "问题产品需要退换"},
                new ResultItem() {name = "跳槽",good = "新工作待遇大幅提升",bad = "再忍一忍就加薪了"},
                new ResultItem() {name = "读书",good = "知识就是力量",bad = "注意力完全无法集中"},
                new ResultItem() {name = "早睡",good = "早睡早起方能养生",bad = "会在半夜醒来，然后失眠"},
                new ResultItem() {name = "逛街",good = "物美价廉大优惠",bad = "会遇到奸商"},
            };

            good.Clear();
            bad.Clear();

            long sg = rnd(seed, 8) % 100;
            for (long i = 0, l = rnd(seed, 9) % 3 + 2; i < l; i++)
            {
                int n = (int)(sg * 0.01 * list.Count());
                ResultItem a = list[n];
                int m = (int)(rnd(seed, (3 + i)) % 100 * 0.01 * avatars.Count());
                good.Add(new ListItem(){avatar = avatars[m],name = a.name,result = a.good});
                list.RemoveAt(n);
                avatars.RemoveAt(m);
            }

            long sb = rnd(seed, 4) % 100;
            for (long i = 0, l = rnd(seed, 7) % 3 + 2; i < l; i++)
            {
                int n = (int)(sb * 0.01 * list.Count());
                ResultItem a = list[n];
                int m = (int)(rnd(seed, (2 + i)) % 100 * 0.01 * avatars.Count());
                bad.Add(new ListItem() { avatar = avatars[m], name = a.name, result = a.bad });
                list.RemoveAt(n);
                avatars.RemoveAt(m);
            }
        }

        public static long getFortune()
        {
            var date = DateTime.Now;
            long seed = 37621 * date.Year + 539 * date.Month + date.Day;

            // A站用的是uid，这里用时间戳代替
            int uid = int.Parse(Cookie.Uid);
            //uid = 624755;

            long fortune = rnd(seed * uid, 6) % 100;

            return fortune;
        }

        public static string getFortune(long fortune)
        {

            String fortuneLevel = "末吉";
            // if
            if (fortune < 5)
            {
                // 5%
                fortuneLevel = "大凶";
            }
            else if (fortune < 20)
            {
                // 15%
                fortuneLevel = "凶";
            }
            else if (fortune < 50)
            {
                // 30%
                fortuneLevel = "末吉";
            }
            else if (fortune < 60)
            {
                // 10%
                fortuneLevel = "半吉";
            }
            else if (fortune < 70)
            {
                // 10%
                fortuneLevel = "吉";
            }
            else if (fortune < 80)
            {
                // 10%
                fortuneLevel = "小吉";
            }
            else if (fortune < 90)
            {
                // 10%
                fortuneLevel = "中吉";
            }
            else
            {
                // 5%
                fortuneLevel = "大吉";
            }
            return fortuneLevel;
        }

        private static long rnd(long a, long b)
        {
            long n = a % 11117;
            for (long i = 0; i < 25 + b; i++)
            {
                n = n * n;
                n = n % 11117;
            }
            return n;
        }
    }
}
