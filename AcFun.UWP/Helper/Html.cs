using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using AcFun.UWP.Control;

namespace AcFun.UWP.Helper
{
    public class Html
    {
        public static List<Block> GetContent(string txt)
        {
            txt = txt.Replace("<div", "<p").Replace("</div>", "</p>");
            var list = new List<Block>();

            var regEx = new Regex(
                @"(\<p.*?\>)(?<text>.*?)(\<\/p\>|$)",
                RegexOptions.IgnoreCase | RegexOptions.Singleline);
            foreach (Match match in regEx.Matches(txt))
            {
                var item = match.Groups["text"].Value.Trim();
                if (item.Contains("<img"))
                {
                    var paragraph = new Paragraph();
                    var regImg = new Regex(
                        @"<img\b[^<>]*?\bsrc[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>",
                        RegexOptions.IgnoreCase);
                    foreach (Match matchImg in regImg.Matches(item))
                    {
                        var image = new Image();
                        image.Source = new BitmapImage(new Uri(matchImg.Groups["imgUrl"].Value.Trim()));
                        image.Stretch = Stretch.None;
                        image.HorizontalAlignment = HorizontalAlignment.Center;
                        image.Tapped += async (sender, args) =>
                        {
                            ImgView cd = new ImgView();
                            cd.Source = new BitmapImage(new Uri(matchImg.Groups["imgUrl"].Value));
                            cd.Tapped += (o, eventArgs) =>
                            {
                                cd.Hide();
                            };
                            cd.RightTapped += async (o, eventArgs) =>
                            {
                                await Launcher.LaunchUriAsync(new Uri(matchImg.Groups["imgUrl"].Value));
                            };
                            await cd.ShowAsync();
                        };
                        var container = new InlineUIContainer();
                        container.Child = image;
                        paragraph.Inlines.Add(container);
                    }
                    list.Add(paragraph);
                }
                else if (item.Contains("<span"))
                {
                    var paragraph = new Paragraph();
                    paragraph.Inlines.Add(new Run() { Text = NoHTML(item),
                        Foreground = new SolidColorBrush(Color.FromArgb(255, 34, 34, 34)), FontSize = 16 });
                    paragraph.LineHeight = 26;
                    list.Add(paragraph);
                }
                else
                {
                    var paragraph = new Paragraph();
                    paragraph.Inlines.Add(new Run() { Text = NoHTML(item),
                        Foreground = new SolidColorBrush(Color.FromArgb(255, 34, 34, 34)), FontSize = 16 });
                    paragraph.LineHeight = 26;
                    list.Add(paragraph);
                }
            }

            return list;
        }

        /// <summary>
        /// 用正则表达式去掉Html中的script脚本和html标签
        /// </summary>
        /// <param name="Htmlstring"></param>
        /// <returns></returns>
        /// 
        /// NoHTMLContent正文用
        public static string NoHTMLContent(string Htmlstring)
        {
            //删除脚本   
            Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);

            Htmlstring = WebUtility.HtmlDecode(Htmlstring).Replace("<br/>", "\r\n").Replace("<br>", "\r\n").Replace("</p>", "\r\n").Trim();

            //删除HTML   
            Htmlstring = Regex.Replace(Htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            //Htmlstring = Regex.Replace(Htmlstring, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"([\t])[\s]+", "", RegexOptions.IgnoreCase);
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
            Htmlstring = Regex.Replace(Htmlstring, @"\r\r", Environment.NewLine, RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"\r\n\r\n\r\n", Environment.NewLine, RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"\r\n\r\n", Environment.NewLine, RegexOptions.IgnoreCase);

            return Htmlstring;
        }
        public static string NoHTML(string Htmlstring)
        {
            //删除脚本   
            Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            //删除HTML   
            Htmlstring = Regex.Replace(Htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            //Htmlstring = Regex.Replace(Htmlstring, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"([\t])[\s]+", "", RegexOptions.IgnoreCase);
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
            Htmlstring = Regex.Replace(Htmlstring, @"\r\r", Environment.NewLine, RegexOptions.IgnoreCase);

            return Htmlstring;
        }

        public static string NoTag(string Htmlstring)
        {
            Htmlstring = Regex.Replace(Htmlstring, @"\[color=(.*?)\]", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"\[size=(.*?)\]", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"\[back=(.*?)\]", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"\[font=(.*?)\]", "", RegexOptions.IgnoreCase);

            Htmlstring = Htmlstring.Replace("[/u]", "");
            Htmlstring = Htmlstring.Replace("[u]", "");
            Htmlstring = Htmlstring.Replace("[/b]", "");
            Htmlstring = Htmlstring.Replace("[b]", "");
            Htmlstring = Htmlstring.Replace("[/i]", "");
            Htmlstring = Htmlstring.Replace("[i]", "");
            Htmlstring = Htmlstring.Replace("[/s]", "");
            Htmlstring = Htmlstring.Replace("[s]", "");
            Htmlstring = Htmlstring.Replace("[/color]", "");
            Htmlstring = Htmlstring.Replace("[/size]", "");
            Htmlstring = Htmlstring.Replace("[/back]", "");
            Htmlstring = Htmlstring.Replace("[/font]", "");

            return Htmlstring;
        }
    }
}
