using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace ACFUN.Controls
{
    public partial class EmojiBox : UserControl
    {
        private PhoneApplicationPage _BasePage;

        public event EventHandler<EmojiSelectEventArgs> EmojiSelectCompleted;

        List<Emoji> EmojiList_ac { get; set; }
        List<Emoji> EmojiList_ais { get; set; }
        List<Emoji> EmojiList_tsj { get; set; }
        List<Emoji> EmojiList_brd { get; set; }
        List<Emoji> EmojiList_td { get; set; }

        public EmojiBox()
        {
            InitializeComponent();
            Init();
        }

        public void Open(PhoneApplicationPage basePage)
        {
            _BasePage = basePage;
            _BasePage.BackKeyPress += BasePage_BackKeyPress;
            OpenStory.Begin();
        }

        private void Close()
        {
            if (_BasePage != null)
            {
                _BasePage.BackKeyPress -= BasePage_BackKeyPress;
                OpenStory.Stop();
                CloseStory.Begin();
            }
            _BasePage = null;
        }

        private void BasePage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Close();
        }

        private void Init()
        {
            EmojiList_ac = new List<Emoji>();
            for (int i = 1; i <= 54; i++)
            {
                EmojiList_ac.Add(new Emoji("[emot=ac," + i.ToString("00") + "/]", "/Assets/Emoji/ac/" + i.ToString("00") + ".png"));
            }
            ac.ItemsSource = EmojiList_ac;

            EmojiList_ais = new List<Emoji>();
            for (int i = 1; i <= 40; i++)
            {
                EmojiList_ais.Add(new Emoji("[emot=ais," + i.ToString("00") + "/]", "/Assets/Emoji/ais/" + i.ToString("00") + ".png"));
            }
            ais.ItemsSource = EmojiList_ais;

            EmojiList_tsj = new List<Emoji>();
            for (int i = 1; i <= 40; i++)
            {
                EmojiList_tsj.Add(new Emoji("[emot=tsj," + i.ToString("00") + "/]", "/Assets/Emoji/tsj/" + i.ToString("00") + ".png"));
            }
            tsj.ItemsSource = EmojiList_tsj;

            EmojiList_brd = new List<Emoji>();
            for (int i = 1; i <= 40; i++)
            {
                EmojiList_brd.Add(new Emoji("[emot=brd," + i.ToString("00") + "/]", "/Assets/Emoji/brd/" + i.ToString("00") + ".png"));
            }
            brd.ItemsSource = EmojiList_brd;

            EmojiList_td = new List<Emoji>();
            for (int i = 1; i <= 40; i++)
            {
                EmojiList_td.Add(new Emoji("[emot=td," + i.ToString("00") + "/]", "/Assets/Emoji/td/" + i.ToString("00") + ".png"));
            }
            td.ItemsSource = EmojiList_td;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox box = sender as ListBox;
            if (box.SelectedIndex < 0)
            { return; }
            Emoji em = box.SelectedItem as Emoji;
            box.SelectedIndex = -1;
            if (EmojiSelectCompleted != null)
            {
                EmojiSelectCompleted.Invoke(this, new EmojiSelectEventArgs(em.content));
                this.Close();
            }
        }
    }

    public class EmojiSelectEventArgs : EventArgs
    {
        public string content { get; set; }

        public EmojiSelectEventArgs(string content)
        {
            this.content = content;
        }
    }

    public class Emoji
    {
        public string content { get; set; }
        public string url { get; set; }

        public Emoji(string content, string url)
        {
            this.content = content;
            this.url = url;
        }
    }
}
