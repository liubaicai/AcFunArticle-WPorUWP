using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AcFunBlue.Common
{
    public class HtmlHelper
    {
        public static string NoHTML(string Htmlstring)
        {
            //删除脚本   
            Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            //删除HTML   
            Htmlstring = Regex.Replace(Htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"-->", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(nbsp|#160);", "   ", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);

            //Htmlstring = Htmlstring.Replace("<", "");
            //Htmlstring = Htmlstring.Replace(">", "");
            //Htmlstring = Htmlstring.Replace("\r\n", "");
            Htmlstring = WebUtility.HtmlDecode(Htmlstring).Replace("<br/>", "").Replace("<br>", "").Trim();

            return Htmlstring;
        }

        public static HtmlNode getListNode(HtmlAgilityPack.HtmlDocument doc)
        {
            try
            {
                foreach (var html in doc.DocumentNode.ChildNodes)
                {
                    if (html.Name.Equals("html"))
                    {
                        foreach (var body in html.ChildNodes)
                        {
                            if (body.Name.Equals("body"))
                            {
                                foreach (var div1 in body.ChildNodes)
                                {
                                    if (div1.Name.Equals("div"))
                                    {
                                        if (div1.HasAttributes && div1.Attributes[0].Value.Equals("mainer th-list"))
                                        {
                                            return div1;
                                        }
                                        if (div1.HasChildNodes)
                                        {
                                            foreach (var div2 in div1.ChildNodes)
                                            {
                                                if (div2.HasAttributes && div2.Attributes[0].Value.Equals("mainer th-list"))
                                                {
                                                    return div2;
                                                }
                                                if (div2.HasChildNodes)
                                                {
                                                    foreach (var div3 in div2.ChildNodes)
                                                    {
                                                        if (div3.HasAttributes && div3.Attributes[0].Value.Equals("mainer th-list"))
                                                        {
                                                            return div3;
                                                        }
                                                        if (div3.HasChildNodes)
                                                        {
                                                            foreach (var div4 in div3.ChildNodes)
                                                            {
                                                                if (div4.HasAttributes && div4.Attributes[0].Value.Equals("mainer th-list"))
                                                                {
                                                                    return div4;
                                                                }
                                                                if (div4.HasChildNodes)
                                                                {
                                                                    foreach (var div5 in div4.ChildNodes)
                                                                    {
                                                                        if (div5.HasAttributes && div5.Attributes[0].Value.Equals("mainer th-list"))
                                                                        {
                                                                            return div5;
                                                                        }
                                                                        if (div5.HasChildNodes)
                                                                        {
                                                                            foreach (var div6 in div5.ChildNodes)
                                                                            {
                                                                                if (div6.HasAttributes && div6.Attributes[0].Value.Equals("mainer th-list"))
                                                                                {
                                                                                    return div6;
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch { }
            return null;
        }
    }
}
