using Microsoft.Phone.Tasks;
using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
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

            var nextOffset = 0;

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
                paragraph.Inlines.Add(new Run { Text = "    " });
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
                paragraph.Inlines.Add(new Run { Text = "    " });
            }

            var run = new Run { Text = HtmlHelp.NoHTML(text) };
            var link = new Hyperlink { };
            link.Click += new RoutedEventHandler((sender, e) =>
            {
                var task = new WebBrowserTask();
                task.Uri = uri;
                task.Show();
            });

            link.Inlines.Add(run);
            paragraph.Inlines.Add(link);
        }
    }
}
