using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ACFUN.Controls
{
    public class CommentBox : StackPanel
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(CommentBox),
                                                                                                   new PropertyMetadata(OnCommentBoxChanged));
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); SetContent(value); }
        }
        private static void OnCommentBoxChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj != null && obj is CommentBox)
            {
                (obj as CommentBox).SetContent((string)e.NewValue);
            }
        }

        private void SetContent(string content)
        {
            this.Children.Clear();
            string[] arr = Regex.Split(content, @"\r\n", RegexOptions.IgnoreCase);
            foreach (var s in arr)
            {
                var item = new EmojiBlock();
                item.Text = s;
                item.Margin = new Thickness(-12, 0, -12, 0);
                item.Width = this.Width;
                item.FontSize = 22;
                item.LineHeight = 30;
                item.FontFamily = new FontFamily("DengXian");
                this.Children.Add(item);
            }
        }
    }
    public class EmojiBlock : System.Windows.Controls.RichTextBox
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(EmojiBlock),
                                                                                                   new PropertyMetadata(OnBlockTextChanged));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); SetEmojiText(value); }
        }


        static void OnBlockTextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj != null && obj is EmojiBlock)
            {
                (obj as EmojiBlock).SetEmojiText((string)e.NewValue);
            }
        }


        private void SetEmojiText(string htmlFragment)
        {
            if (htmlFragment == null || htmlFragment.Length == 0)
                return;

            this.Blocks.Clear();

            Paragraph paragraph;

            if (this.Blocks.Count == 0 ||
                (paragraph = this.Blocks[this.Blocks.Count - 1] as Paragraph) == null)
            {
                paragraph = new Paragraph();
                this.Blocks.Add(paragraph);
            }
            var nextOffset = 0;

            var regEx = new Regex(@"(\[emot=(?<emoji>.*?)/\])", RegexOptions.IgnoreCase | RegexOptions.Singleline);

            foreach (Match match in regEx.Matches(htmlFragment))
            {
                if (match.Index == nextOffset)
                {
                    nextOffset = match.Index + match.Length;
                    if (match.Groups["emoji"].Value.Split(',').Count() > 1)
                    {
                        var grid = new Grid();
                        grid.Background = new SolidColorBrush(Colors.White);
                        grid.Margin = new Thickness(1, 0, 1, 0);
                        var image = new Image();
                        var bi = new BitmapImage(new Uri("/Assets/Emoji/"
                            + match.Groups["emoji"].Value.Split(',')[0] + "/" + match.Groups["emoji"].Value.Split(',')[1] + ".png", UriKind.RelativeOrAbsolute));
                        image.Source = bi;
                        image.Height = 60;
                        grid.Children.Add(image);
                        var container = new InlineUIContainer();
                        container.Child = grid;
                        paragraph.Inlines.Add(container);
                    }
                    else
                    {
                        this.SetAtText(HtmlHelp.NoHTML(match.Groups["emoji"].Value));
                    }
                }
                else if (match.Index > nextOffset)
                {
                    this.SetAtText(HtmlHelp.NoHTML(htmlFragment.Substring(nextOffset, match.Index - nextOffset)));
                    nextOffset = match.Index + match.Length;
                    if (match.Groups["emoji"].Value.Split(',').Count() > 1)
                    {
                        var grid = new Grid();
                        grid.Background = new SolidColorBrush(Colors.White);
                        grid.Margin = new Thickness(1, 0, 1, 0);
                        var image = new Image();
                        var bi = new BitmapImage(new Uri("/Assets/Emoji/"
                            + match.Groups["emoji"].Value.Split(',')[0] + "/" + match.Groups["emoji"].Value.Split(',')[1] + ".png", UriKind.RelativeOrAbsolute));
                        image.Source = bi;
                        image.Height = 60;
                        grid.Children.Add(image);
                        var container = new InlineUIContainer();
                        container.Child = grid;
                        paragraph.Inlines.Add(container);
                    }
                    else
                    {
                        this.SetAtText(HtmlHelp.NoHTML(match.Groups["emoji"].Value));
                    }
                }
            }

            if (nextOffset < htmlFragment.Length)
            {
                this.SetAtText(HtmlHelp.NoHTML(htmlFragment.Substring(nextOffset)));
            }
        }

        private void SetAtText(string htmlFragment)
        {
            if (htmlFragment == null || htmlFragment.Length == 0)
                return;

            Paragraph paragraph;

            if (this.Blocks.Count == 0 ||
                (paragraph = this.Blocks[this.Blocks.Count - 1] as Paragraph) == null)
            {
                paragraph = new Paragraph();
                this.Blocks.Add(paragraph);
            }
            var nextOffset = 0;

            var regEx = new Regex(@"(\[at\](?<name>.*?)\[/at\])", RegexOptions.IgnoreCase | RegexOptions.Singleline);

            foreach (Match match in regEx.Matches(htmlFragment))
            {
                if (match.Index == nextOffset)
                {
                    nextOffset = match.Index + match.Length;
                    paragraph.Inlines.Add(new Run { Text = "@" + match.Groups["name"].Value + " ", Foreground = new SolidColorBrush(Color.FromArgb(255, 150, 230, 230)) });
                }
                else if (match.Index > nextOffset)
                {
                    this.SetColorText(htmlFragment.Substring(nextOffset, match.Index - nextOffset));
                    nextOffset = match.Index + match.Length;
                    paragraph.Inlines.Add(new Run { Text = "@" + match.Groups["name"].Value + " ", Foreground = new SolidColorBrush(Color.FromArgb(255, 150, 230, 230)) });
                }
            }

            if (nextOffset < htmlFragment.Length)
            {
                this.SetColorText(htmlFragment.Substring(nextOffset));
            }
        }

        private void SetColorText(string htmlFragment)
        {
            if (htmlFragment == null || htmlFragment.Length == 0)
                return;

            Paragraph paragraph;

            if (this.Blocks.Count == 0 ||
                (paragraph = this.Blocks[this.Blocks.Count - 1] as Paragraph) == null)
            {
                paragraph = new Paragraph();
                this.Blocks.Add(paragraph);
            }
            var nextOffset = 0;

            var regEx = new Regex(@"(\[color=#FFFFFF\](?<colortxt>.*?)\[/color\])", RegexOptions.IgnoreCase | RegexOptions.Singleline);

            foreach (Match match in regEx.Matches(htmlFragment))
            {
                if (match.Index == nextOffset)
                {
                    nextOffset = match.Index + match.Length;
                    paragraph.Inlines.Add(new Run { Text = "(此处反白)->" + HtmlHelp.NoTag(match.Groups["colortxt"].Value) + "<-", Foreground = new SolidColorBrush(Colors.Red) });
                }
                else if (match.Index > nextOffset)
                {
                    this.SetImgText(htmlFragment.Substring(nextOffset, match.Index - nextOffset));
                    nextOffset = match.Index + match.Length;
                    paragraph.Inlines.Add(new Run { Text = "(此处反白)->" + HtmlHelp.NoTag(match.Groups["colortxt"].Value) + "<-", Foreground = new SolidColorBrush(Colors.Red) });
                }
            }

            if (nextOffset < htmlFragment.Length)
            {
                this.SetImgText(htmlFragment.Substring(nextOffset));
            }
        }

        private void SetImgText(string htmlFragment)
        {
            if (htmlFragment == null || htmlFragment.Length == 0)
                return;

            Paragraph paragraph;

            if (this.Blocks.Count == 0 ||
                (paragraph = this.Blocks[this.Blocks.Count - 1] as Paragraph) == null)
            {
                paragraph = new Paragraph();
                this.Blocks.Add(paragraph);
            }
            var nextOffset = 0;

            var regEx = new Regex(@"(\[img=(?<imgname>.*?)\](?<imgtxt>.*?)\[/img\])", RegexOptions.IgnoreCase | RegexOptions.Singleline);

            foreach (Match match in regEx.Matches(htmlFragment))
            {
                if (match.Index == nextOffset)
                {
                    nextOffset = match.Index + match.Length;

                    var run = new Run { Text = "[此处有图片]", Foreground = new SolidColorBrush(Color.FromArgb(255, 150, 230, 230)) };
                    var link = new Hyperlink { };
                    link.Click += new RoutedEventHandler((sender, e) =>
                    {
                        Navi.NavigationTo("/ImageView.xaml?url=" + HttpUtility.UrlEncode(match.Groups["imgtxt"].Value));
                    });
                    link.Inlines.Add(run);
                    paragraph.Inlines.Add(link);
                }
                else if (match.Index > nextOffset)
                {
                    this.SetImgText(htmlFragment.Substring(nextOffset, match.Index - nextOffset));
                    nextOffset = match.Index + match.Length;

                    var run = new Run { Text = "[此处有图片]", Foreground = new SolidColorBrush(Color.FromArgb(255, 150, 230, 230)) };
                    var link = new Hyperlink { };
                    link.Click += new RoutedEventHandler((sender, e) =>
                    {
                        Navi.NavigationTo("/ImageView.xaml?url=" + HttpUtility.UrlEncode(match.Groups["imgtxt"].Value));
                    });
                    link.Inlines.Add(run);
                    paragraph.Inlines.Add(link);
                }
            }

            if (nextOffset < htmlFragment.Length)
            {
                paragraph.Inlines.Add(new Run { Text = HtmlHelp.NoTag(htmlFragment.Substring(nextOffset)) });
            }
        }
    }
}
