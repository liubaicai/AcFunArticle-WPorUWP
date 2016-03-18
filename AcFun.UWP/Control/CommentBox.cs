using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using AcFun.UWP.Helper;

namespace AcFun.UWP.Control
{
    public class CommentBox : Grid
    {
        public static readonly DependencyProperty TextProperty
            = DependencyProperty.Register("Text", typeof(string), typeof(CommentBox), new PropertyMetadata("", OnBlockTextChanged));
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); SetEmojiText(value); }
        }
        static void OnBlockTextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var box = obj as CommentBox;
            if (box != null)
            {
                box.SetEmojiText((string)e.NewValue);
                box.Children.Clear();
                box.Children.Add(box.Rtb);
            }
        }

        private readonly RichTextBlock Rtb = new RichTextBlock() { TextWrapping = TextWrapping.Wrap };

        private void SetEmojiText(string htmlFragment)
        {
            if (string.IsNullOrEmpty(htmlFragment))
                return;

            Rtb.Blocks.Clear();

            Paragraph paragraph;

            if (Rtb.Blocks.Count == 0 ||
                (paragraph = Rtb.Blocks[Rtb.Blocks.Count - 1] as Paragraph) == null)
            {
                paragraph = new Paragraph();
                Rtb.Blocks.Add(paragraph);
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
                        var bi = new BitmapImage(new Uri("ms-appx:///Assets/Emoji/"
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
                        this.SetAtText(Html.NoHTML(match.Groups["emoji"].Value));
                    }
                }
                else if (match.Index > nextOffset)
                {
                    this.SetAtText(Html.NoHTML(htmlFragment.Substring(nextOffset, match.Index - nextOffset)));
                    nextOffset = match.Index + match.Length;
                    if (match.Groups["emoji"].Value.Split(',').Count() > 1)
                    {
                        var grid = new Grid();
                        grid.Background = new SolidColorBrush(Colors.White);
                        grid.Margin = new Thickness(1, 0, 1, 0);
                        var image = new Image();
                        var bi = new BitmapImage(new Uri("ms-appx:///Assets/Emoji/"
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
                        this.SetAtText(Html.NoHTML(match.Groups["emoji"].Value));
                    }
                }
            }

            if (nextOffset < htmlFragment.Length)
            {
                this.SetAtText(Html.NoHTML(htmlFragment.Substring(nextOffset)));
            }
        }

        private void SetAtText(string htmlFragment)
        {
            if (string.IsNullOrEmpty(htmlFragment))
                return;

            Paragraph paragraph;

            if (Rtb.Blocks.Count == 0 ||
                (paragraph = Rtb.Blocks[Rtb.Blocks.Count - 1] as Paragraph) == null)
            {
                paragraph = new Paragraph();
                Rtb.Blocks.Add(paragraph);
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
            if (string.IsNullOrEmpty(htmlFragment))
                return;

            Paragraph paragraph;

            if (Rtb.Blocks.Count == 0 ||
                (paragraph = Rtb.Blocks[Rtb.Blocks.Count - 1] as Paragraph) == null)
            {
                paragraph = new Paragraph();
                Rtb.Blocks.Add(paragraph);
            }
            var nextOffset = 0;

            var regEx = new Regex(@"(\[color=#FFFFFF\](?<colortxt>.*?)\[/color\])", RegexOptions.IgnoreCase | RegexOptions.Singleline);

            foreach (Match match in regEx.Matches(htmlFragment))
            {
                if (match.Index == nextOffset)
                {
                    nextOffset = match.Index + match.Length;
                    paragraph.Inlines.Add(new Run { Text = "(此处反白)->" + Html.NoTag(match.Groups["colortxt"].Value) + "<-", Foreground = new SolidColorBrush(Colors.Red) });
                }
                else if (match.Index > nextOffset)
                {
                    this.SetImgText(htmlFragment.Substring(nextOffset, match.Index - nextOffset));
                    nextOffset = match.Index + match.Length;
                    paragraph.Inlines.Add(new Run { Text = "(此处反白)->" + Html.NoTag(match.Groups["colortxt"].Value) + "<-", Foreground = new SolidColorBrush(Colors.Red) });
                }
            }

            if (nextOffset < htmlFragment.Length)
            {
                this.SetImgText(htmlFragment.Substring(nextOffset));
            }
        }

        private void SetImgText(string htmlFragment)
        {
            if (string.IsNullOrEmpty(htmlFragment))
                return;

            Paragraph paragraph;

            if (Rtb.Blocks.Count == 0 ||
                (paragraph = Rtb.Blocks[Rtb.Blocks.Count - 1] as Paragraph) == null)
            {
                paragraph = new Paragraph();
                Rtb.Blocks.Add(paragraph);
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
                    link.Click += async (sender, args) =>
                    {
                        ImgView cd = new ImgView();
                        cd.Source = new BitmapImage(new Uri(match.Groups["imgtxt"].Value));
                        cd.Tapped += (o, eventArgs) =>
                        {
                            cd.Hide();
                        };
                        cd.RightTapped += async (o, eventArgs) =>
                        {
                            await Launcher.LaunchUriAsync(new Uri(match.Groups["imgtxt"].Value));
                        };
                        await cd.ShowAsync();
                    };
                    link.Inlines.Add(run);
                    paragraph.Inlines.Add(link);
                }
                else if (match.Index > nextOffset)
                {
                    this.SetImgText(htmlFragment.Substring(nextOffset, match.Index - nextOffset));
                    nextOffset = match.Index + match.Length;

                    var run = new Run { Text = "[此处有图片]", Foreground = new SolidColorBrush(Color.FromArgb(255, 150, 230, 230)) };
                    var link = new Hyperlink { };
                    link.Click += async (sender, args) =>
                    {
                        ImgView cd = new ImgView();
                        cd.Source = new BitmapImage(new Uri(match.Groups["imgtxt"].Value));
                        cd.Tapped += (o, eventArgs) =>
                        {
                            cd.Hide();
                        };
                        cd.RightTapped += async (o, eventArgs) =>
                        {
                            await Launcher.LaunchUriAsync(new Uri(match.Groups["imgtxt"].Value));
                        };
                        await cd.ShowAsync();
                    };
                    link.Inlines.Add(run);
                    paragraph.Inlines.Add(link);
                }
            }

            if (nextOffset < htmlFragment.Length)
            {
                paragraph.Inlines.Add(new Run { Text = Html.NoTag(htmlFragment.Substring(nextOffset)), Foreground = new SolidColorBrush(Colors.Black) });
            }
        }
    }
}
