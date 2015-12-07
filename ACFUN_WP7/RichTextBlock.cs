using Microsoft.Phone.Tasks;
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

namespace ACFUN
{
    public class RichTextBlock : System.Windows.Controls.RichTextBox
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(RichTextBlock),
                                                                                                new PropertyMetadata(OnBlockTextChanged));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); SetLinkedText(value); }
        }


        static void OnBlockTextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj != null && obj is RichTextBlock)
            {
                (obj as RichTextBlock).SetLinkedText((string)e.NewValue);
            }
        }


        public void SetLinkedText(string htmlFragment)
        {
            if (htmlFragment == null || htmlFragment.Length == 0)
                return;
            var regEx = new Regex(
                @"\<a\s(href\=""|[^\>]+?\shref\="")(?<link>[^""]+)"".*?\>(?<text>.*?)(\<\/a\>|$)",
                RegexOptions.IgnoreCase | RegexOptions.Multiline);

            this.Blocks.Clear();

            int nextOffset = 0;

            foreach (Match match in regEx.Matches(htmlFragment))
            {
                if (match.Index > nextOffset)
                {
                    this.AppendText(htmlFragment.Substring(nextOffset, match.Index - nextOffset));
                    nextOffset = match.Index + match.Length;
                    this.AppendLink(match.Groups["text"].Value, new Uri(match.Groups["link"].Value));
                }

            }

            if (nextOffset < htmlFragment.Length)
            {
                this.AppendText(htmlFragment.Substring(nextOffset));
            }
        }

        public void AppendText(string text)
        {
            Paragraph paragraph;

            if (this.Blocks.Count == 0 ||
                (paragraph = this.Blocks[this.Blocks.Count - 1] as Paragraph) == null)
            {
                paragraph = new Paragraph();
                this.Blocks.Add(paragraph);
            }

            paragraph.Inlines.Add(new Run { Text = HtmlHelp.NoHTML(text) });
        }

        public void AppendLink(string text, Uri uri)
        {
            Paragraph paragraph;

            if (this.Blocks.Count == 0 ||
                (paragraph = this.Blocks[this.Blocks.Count - 1] as Paragraph) == null)
            {
                paragraph = new Paragraph();
                this.Blocks.Add(paragraph);
            }

            var run = new Run { Text = HtmlHelp.NoHTML(text) };
            var link = new Hyperlink { };
            link.Click += new RoutedEventHandler((sender, e) =>
            {
                WebBrowserTask task = new WebBrowserTask();
                task.Uri = uri;
                task.Show();
            });

            link.Inlines.Add(run);
            paragraph.Inlines.Add(link);
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
            int nextOffset = 0;

            var regEx = new Regex(@"(\[emot=(?<emoji>.*?)/\])", RegexOptions.IgnoreCase | RegexOptions.Singleline);

            foreach (Match match in regEx.Matches(htmlFragment))
            {
                if (match.Index == nextOffset)
                {
                    nextOffset = match.Index + match.Length;
                    if (match.Groups["emoji"].Value.Split(',').Count() > 1)
                    {
                        Grid grid = new Grid();
                        grid.Background = new SolidColorBrush(Colors.White);
                        grid.Margin = new Thickness(1, 0, 1, 0);
                        Image image = new Image();
                        BitmapImage bi = new BitmapImage(new Uri("/Assets/Emoji/"
                            + match.Groups["emoji"].Value.Split(',')[0] + "/" + match.Groups["emoji"].Value.Split(',')[1] + ".png", UriKind.RelativeOrAbsolute));
                        image.Source = bi;
                        image.Height = 60;
                        grid.Children.Add(image);
                        InlineUIContainer container = new InlineUIContainer();
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
                        Grid grid = new Grid();
                        grid.Background = new SolidColorBrush(Colors.White);
                        grid.Margin = new Thickness(1, 0, 1, 0);
                        Image image = new Image();
                        BitmapImage bi = new BitmapImage(new Uri("/Assets/Emoji/"
                            + match.Groups["emoji"].Value.Split(',')[0] + "/" + match.Groups["emoji"].Value.Split(',')[1] + ".png", UriKind.RelativeOrAbsolute));
                        image.Source = bi;
                        image.Height = 60;
                        grid.Children.Add(image);
                        InlineUIContainer container = new InlineUIContainer();
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
            int nextOffset = 0;

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
            int nextOffset = 0;

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
                    paragraph.Inlines.Add(new Run { Text = HtmlHelp.NoTag(htmlFragment.Substring(nextOffset, match.Index - nextOffset)) });
                    nextOffset = match.Index + match.Length;
                    paragraph.Inlines.Add(new Run { Text = "(此处反白)->" + HtmlHelp.NoTag(match.Groups["colortxt"].Value) + "<-", Foreground = new SolidColorBrush(Colors.Red) });
                }
            }

            if (nextOffset < htmlFragment.Length)
            {
                paragraph.Inlines.Add(new Run { Text = HtmlHelp.NoTag(htmlFragment.Substring(nextOffset)) });
            }
        }
    }
}
