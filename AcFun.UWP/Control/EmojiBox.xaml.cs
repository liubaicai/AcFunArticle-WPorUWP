using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace AcFun.UWP.Control
{
    public sealed partial class EmojiBox : ContentDialog
    {

        public event EventHandler<string> EmojiSelectCompleted;

        List<Emoji> EmojiListAc { get; set; }
        List<Emoji> EmojiListAis { get; set; }
        List<Emoji> EmojiListTsj { get; set; }
        List<Emoji> EmojiListBrd { get; set; }
        List<Emoji> EmojiListTd { get; set; }

        public EmojiBox()
        {
            this.InitializeComponent();
            Init();
        }

        private void Emoji_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var em = e.ClickedItem as Emoji;
            if (em != null) EmojiSelectCompleted?.Invoke(this, em.Content);
        }

        private void Init()
        {
            EmojiListAc = new List<Emoji>();
            for (var i = 1; i <= 54; i++)
            {
                EmojiListAc.Add(new Emoji("[emot=ac," + i.ToString("00") + "/]", "ms-appx:///Assets/Emoji/ac/" + i.ToString("00") + ".png"));
            }
            ac.ItemsSource = EmojiListAc;

            EmojiListAis = new List<Emoji>();
            for (var i = 1; i <= 40; i++)
            {
                EmojiListAis.Add(new Emoji("[emot=ais," + i.ToString("00") + "/]", "ms-appx:///Assets/Emoji/ais/" + i.ToString("00") + ".png"));
            }
            ais.ItemsSource = EmojiListAis;

            EmojiListTsj = new List<Emoji>();
            for (var i = 1; i <= 40; i++)
            {
                EmojiListTsj.Add(new Emoji("[emot=tsj," + i.ToString("00") + "/]", "ms-appx:///Assets/Emoji/tsj/" + i.ToString("00") + ".png"));
            }
            tsj.ItemsSource = EmojiListTsj;

            EmojiListBrd = new List<Emoji>();
            for (var i = 1; i <= 40; i++)
            {
                EmojiListBrd.Add(new Emoji("[emot=brd," + i.ToString("00") + "/]", "ms-appx:///Assets/Emoji/brd/" + i.ToString("00") + ".png"));
            }
            brd.ItemsSource = EmojiListBrd;

            EmojiListTd = new List<Emoji>();
            for (var i = 1; i <= 40; i++)
            {
                EmojiListTd.Add(new Emoji("[emot=td," + i.ToString("00") + "/]", "ms-appx:///Assets/Emoji/td/" + i.ToString("00") + ".png"));
            }
            td.ItemsSource = EmojiListTd;
        }
    }

    public class Emoji
    {
        public string Content { get; set; }
        public string Url { get; set; }

        public Emoji(string content, string url)
        {
            this.Content = content;
            this.Url = url;
        }
    }
}
