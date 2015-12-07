using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ACFUN.Controls
{
    public class ScrollableTextBlock : System.Windows.Controls.Control
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(ScrollableTextBlock), new PropertyMetadata(OnTextPropertyChanged));
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        private StackPanel stackPanel;
        private TextBlock measureBlock;

        public ScrollableTextBlock()
        {
            this.DefaultStyleKey = typeof(ScrollableTextBlock);
        }

        private static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null && d is ScrollableTextBlock)
            {
                (d as ScrollableTextBlock).ParseText((string)e.NewValue);
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.stackPanel = this.GetTemplateChild("StackPanel") as StackPanel;
            if (this.Text != null)
            {
                this.ParseText(this.Text);
            }
        }


        private void ParseTextHide(string value)
        {
            if (this.stackPanel == null)
            {
                return;
            }
            // Clear previous TextBlocks
            this.stackPanel.Children.Clear();
            // Calculate max char count
            int maxTexCount = this.GetMaxTextSize();

            if (value.Length < maxTexCount)
            {
                TextBlock textBlock = this.GetTextBlock();
                textBlock.Text = value;
                this.stackPanel.Children.Add(textBlock);
            }
            else
            {
                int n = value.Length / maxTexCount;
                int start = 0;

                // Add textblocks
                for (int i = 0; i < n; i++)
                {
                    TextBlock textBlock = this.GetTextBlock();
                    textBlock.Text = value.Substring(start, maxTexCount);
                    this.stackPanel.Children.Add(textBlock);
                    start = maxTexCount;
                }

                // Pickup the leftover text
                if (value.Length % maxTexCount > 0)
                {
                    TextBlock textBlock = this.GetTextBlock();
                    textBlock.Text = value.Substring(maxTexCount * n, value.Length - maxTexCount * n);
                    this.stackPanel.Children.Add(textBlock);
                }
            }
        }

        private void ParseText(string value)
        {
            if (value == null)
            {
                return;
            }
            StringReader reader = new StringReader(value);

            if (this.stackPanel == null)
            {
                return;
            }
            // Clear previous TextBlocks  
            this.stackPanel.Children.Clear();
            // Calculate max char count  
            int maxTexCount = this.GetMaxTextSize();

            // fix: disable this clause  
            //if (value.Length < maxTexCount)  
            //{  
            //    TextBlock textBlock = this.GetTextBlock();  
            //    textBlock.Text = value;  
            //    this.stackPanel.Children.Add(textBlock);  
            //}  
            //else  
            {
                string line = "";

                while (reader.Peek() > 0)
                {
                    line = reader.ReadLine();
                    ParseLine(line);
                }
            }

            //StringReader reader = new StringReader(value);

            //if (this.stackPanel == null)
            //{
            //    return;
            //}
            //// Clear previous TextBlocks
            //this.stackPanel.Children.Clear();
            //// Calculate max char count
            //int maxTexCount = this.GetMaxTextSize();

            //if (value.Length < maxTexCount)
            //{
            //    TextBlock textBlock = this.GetTextBlock();
            //    textBlock.Text = value;
            //    this.stackPanel.Children.Add(textBlock);
            //}
            //else
            //{                         
            //    string line = "";

            //    while (reader.Peek() > 0)
            //    {
            //        line = reader.ReadLine();
            //        ParseLine(line);                  
            //    }

            //}

        }

        private void ParseLine(string line)
        {
            // fix: next two lines added  
            if (line.Length == 0)
                line = " ";

            int lineCount = 0;
            int maxLineCount = GetMaxLineCount();
            string tempLine = line;
            StringBuilder sbLine = new StringBuilder();

            while (lineCount < maxLineCount)
            {
                int charactersFitted = MeasureString(tempLine, (int)this.Width);
                string leftSide = tempLine.Substring(0, charactersFitted);
                sbLine.Append(leftSide);
                tempLine = tempLine.Substring(charactersFitted, tempLine.Length - (charactersFitted));
                lineCount++;
            }

            TextBlock textBlock = this.GetTextBlock();
            textBlock.Text = sbLine.ToString();
            this.stackPanel.Children.Add(textBlock);

            if (tempLine.Length > 0)
            {
                ParseLine(tempLine);
            }

            //int lineCount = 0;
            //int maxLineCount = GetMaxLineCount();
            //string tempLine = line;
            //StringBuilder sbLine = new StringBuilder();

            //while (lineCount < maxLineCount)
            //{
            //    int charactersFitted = MeasureString(tempLine, (int)this.Width);
            //    string leftSide = tempLine.Substring(0, charactersFitted);
            //    sbLine.Append(leftSide);
            //    tempLine = tempLine.Substring(charactersFitted, tempLine.Length - (charactersFitted));
            //    lineCount++;
            //}

            //TextBlock textBlock = this.GetTextBlock();
            //textBlock.Text = sbLine.ToString();
            //this.stackPanel.Children.Add(textBlock);

            //if (tempLine.Length > 0)
            //{
            //    ParseLine(tempLine);
            //}           
        }

        private int MeasureString(string text, int desWidth)
        {

            int nWidth = 0;
            int charactersFitted = 0;

            StringBuilder sb = new StringBuilder();

            //get original size
            Size size = MeasureString(text);

            if (size.Width > desWidth)
            {
                string[] words = text.Split(' ');
                sb.Append(words[0]);

                for (int i = 1; i < words.Length; i++)
                {
                    sb.Append(" " + words[i]);
                    nWidth = (int)MeasureString(sb.ToString()).Width;

                    if (nWidth > desWidth)
                    {

                        sb.Remove(sb.Length - words[i].Length, words[i].Length);
                        break;
                    }
                }

                charactersFitted = sb.Length;
            }
            else
            {
                charactersFitted = text.Length;
            }

            return charactersFitted;
        }

        private Size MeasureString(string text)
        {
            if (this.measureBlock == null)
            {
                this.measureBlock = this.GetTextBlock();
            }

            this.measureBlock.Text = text;
            return new Size(measureBlock.ActualWidth, measureBlock.ActualHeight);
        }

        private int GetMaxTextSize()
        {
            // Get average char size
            Size size = this.MeasureText(" ");
            // Get number of char that fit in the line
            int charLineCount = (int)(this.Width / size.Width);
            // Get line count
            int lineCount = (int)(2048 / size.Height);

            return charLineCount * lineCount / 2;
        }

        private int GetMaxLineCount()
        {
            Size size = this.MeasureText(" ");
            // Get number of char that fit in the line
            int charLineCount = (int)(this.Width / size.Width);
            // Get line count
            int lineCount = (int)(2048 / size.Height) - 5;

            return lineCount;
        }

        private TextBlock GetTextBlock()
        {
            TextBlock textBlock = new TextBlock();
            textBlock.TextWrapping = TextWrapping.Wrap;
            textBlock.FontSize = this.FontSize;
            textBlock.FontFamily = this.FontFamily;
            // textBlock.FontStyle = this.FontStyle;
            textBlock.FontWeight = this.FontWeight;
            textBlock.Foreground = this.Foreground;
            textBlock.LineHeight = this.FontSize * 1.4;
            textBlock.Margin = new Thickness(0, 0, MeasureText(" ").Width, 0);
            return textBlock;
        }

        private Size MeasureText(string value)
        {
            TextBlock textBlock = this.GetTextBlockOnly();
            textBlock.Text = value;
            return new Size(textBlock.ActualWidth, textBlock.ActualHeight);
        }

        private TextBlock GetTextBlockOnly()
        {
            TextBlock textBlock = new TextBlock();
            textBlock.TextWrapping = TextWrapping.Wrap;
            textBlock.FontSize = this.FontSize;
            textBlock.FontFamily = this.FontFamily;
            textBlock.FontWeight = this.FontWeight;
            return textBlock;
        }

    }
}
