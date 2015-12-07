using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using ACWZ.Resources;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Microsoft.Phone.Tasks;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Windows.Media.Imaging;

namespace ACWZ
{
    public partial class MainPage : PhoneApplicationPage, INotifyPropertyChanged
    {
        string url1 = StaticData.list1;
        public ObservableCollection<ACItem> list1 { get; set; }
        int next1 = -1;

        string url2 = StaticData.list2;
        public ObservableCollection<ACItem> list2 { get; set; }
        int next2 = -1;

        string url3 = StaticData.list3;
        public ObservableCollection<ACItem> list3 { get; set; }
        int next3 = -1;

        // 构造函数
        public MainPage()
        {
            ResourceDictionary resourceDictionary = new ResourceDictionary();
            Application.LoadComponent(resourceDictionary, new Uri("/ACWZ;component/ResourceDictionary.xaml", UriKind.RelativeOrAbsolute));
            Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);

            list1 = new ObservableCollection<ACItem>();
            list2 = new ObservableCollection<ACItem>();
            list3 = new ObservableCollection<ACItem>();
            InitializeComponent();
            DataContext = this;

            // 用于本地化 ApplicationBar 的示例代码
            //BuildLocalizedApplicationBar();

            this.Loaded += MainPage_Loaded;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (StaticData.IsLogin)
            {
                denglu.Visibility = Visibility.Collapsed;
                name.Text = StaticData.UserName;
                userlogo.Source = new BitmapImage(new Uri(StaticData.UserImg, UriKind.RelativeOrAbsolute));
                zhuxiao.Visibility = Visibility.Visible;
            }

            if (e.NavigationMode == NavigationMode.New)
            {
                try
                {
                    using (Stream stream = await new HttpHelp().Get(StaticData.versionapi))
                    {
                        if (stream != null)
                        {
                            StreamReader sr = new StreamReader(stream);
                            string version = sr.ReadToEnd();
                            if (version != null && version != "" && Int32.Parse(version) > StaticData.AppVersionCount)
                            {
                                if (MessageBox.Show("新版本出没，更新下吧~", "有新的版本啦", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                                {
                                    MarketplaceDetailTask task = new MarketplaceDetailTask();
                                    task.ContentIdentifier = "393cf4cf-4946-4721-bee5-fb326195dab8";
                                    task.ContentType = MarketplaceContentType.Applications;
                                    task.Show();
                                }
                            }
                        }
                    }
                }
                catch { }
            }

            //try
            //{
            //    double size = await CacheFile.GetCacheSize();
            //    cachesize.Text = (size / 1024 / 1024).ToString("0.00") + "MB";
            //}
            //catch { }
        }

        async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!StaticData.IsLogin&&Settings.GetValueOrDefault("username", "") != "" && Settings.GetValueOrDefault("password", "") != "")
                {
                    HttpHelp httpHelp = new HttpHelp();
                    httpHelp.parameters.Add("username", Settings.GetValueOrDefault("username", ""));
                    httpHelp.parameters.Add("password", Settings.GetValueOrDefault("password", ""));
                    using (var stream = await httpHelp.PostSetCookie(StaticData.login))
                    {
                        DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(loginresult));
                        var result = json.ReadObject(stream) as loginresult;
                        if (result.success)
                        {
                            StaticData.UserName = result.username;
                            StaticData.UserImg = result.img;
                            StaticData.IsLogin = true;
                            denglu.Visibility = Visibility.Collapsed;
                            name.Text = result.username;
                            userlogo.Source = new BitmapImage(new Uri(StaticData.UserImg, UriKind.RelativeOrAbsolute));
                            zhuxiao.Visibility = Visibility.Visible;

                            await getunRead();
                        }
                    }
                }
            }
            catch { }
        }

        private async void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Pivot pivot = sender as Pivot;
                switch (pivot.SelectedIndex)
                {
                    case 0:
                        if (list1.Count <= 0)
                        {
                            list1 = await getList(new ObservableCollection<ACItem>(), string.Format(url1, 1, DateTime.Now.Millisecond));
                            NotifyPropertyChanged("list1");
                            listbox1.SelectedIndex = -1;
                            next1 = 2;
                        }
                        break;
                    case 1:
                        if (list2.Count <= 0)
                        {
                            list2 = await getList(new ObservableCollection<ACItem>(), string.Format(url2, 1, DateTime.Now.Millisecond));
                            NotifyPropertyChanged("list2");
                            listbox2.SelectedIndex = -1;
                            next2 = 2;
                        }
                        break;
                    case 2:
                        if (list3.Count <= 0)
                        {
                            list3 = await getList(new ObservableCollection<ACItem>(), string.Format(url3, 1, DateTime.Now.Millisecond));
                            NotifyPropertyChanged("list3");
                            listbox3.SelectedIndex = -1;
                            next3 = 2;
                        }
                        break;
                    case 4:
                        await getunRead();
                        break;
                }
            }
            catch { }
        }

        private async Task getunRead()
        {
            try
            {
                if (StaticData.IsLogin)
                {
                    using (Stream stream = await new HttpHelp().Get(StaticData.unRead))
                    {
                        DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(unRead));
                        var result = json.ReadObject(stream) as unRead;
                        if (result.success && result.mention > 0)
                        {
                            newmention.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            newmention.Visibility = Visibility.Collapsed;
                        }
                    }
                }
            }
            catch { }
        }

        private async Task<ObservableCollection<ACItem>> getList(ObservableCollection<ACItem> list, string url)
        {
            try
            {
                indicator.IsVisible = true;
                var request = (HttpWebRequest)WebRequest.Create(new Uri(url));
                request.UserAgent = @"Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; Trident/6.0)";
                var response = await request.GetResponseAsync();
                var doc = new HtmlDocument();
                doc.Load(response.GetResponseStream());


                var node = getListNode(doc);
                if (node != null)
                {
                    foreach (var item in node.ChildNodes)
                    {
                        if (item.Name.Equals("div"))
                        {
                            var listtemp = new List<HtmlNode>();
                            foreach (var label in item.ChildNodes)
                            {
                                if (label.Name.Equals("a") || label.Name.Equals("div"))
                                {
                                    listtemp.Add(label);
                                }
                            }

                            ACItem acitem = new ACItem();
                            acitem.title = listtemp[1].InnerText;
                            acitem.href = listtemp[0].Attributes[2].Value;
                            acitem.dis = listtemp[2].ChildNodes[3].InnerText;
                            acitem.time = listtemp[1].Attributes[3].Value;
                            acitem.name = HtmlHelp.NoHTML(listtemp[2].ChildNodes[1].InnerText);
                            acitem.beizhu = HtmlHelp.NoHTML(listtemp[3].InnerText);
                            list.Add(acitem);
                        }
                    }
                }
                indicator.IsVisible = false;
            }
            catch (WebException) { MessageBox.Show("网络不给力啊~"); }
            catch (Exception) { }
            indicator.IsVisible = false;
            return list;
        }

        // 用于生成本地化 ApplicationBar 的示例代码
        //private void BuildLocalizedApplicationBar()
        //{
        //    // 将页面的 ApplicationBar 设置为 ApplicationBar 的新实例。
        //    ApplicationBar = new ApplicationBar();

        //    // 创建新按钮并将文本值设置为 AppResources 中的本地化字符串。
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // 使用 AppResources 中的本地化字符串创建新菜单项。
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}

        private HtmlNode getListNode(HtmlAgilityPack.HtmlDocument doc)
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

        #region 绑定接口实现
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox lb = sender as ListBox;
            if (lb.SelectedIndex != -1)
            {
                ACItem item = lb.SelectedItem as ACItem;
                StaticData.acitem = item;
                NavigationService.Navigate(new Uri("/ACWZ;component/DetailPage.xaml", UriKind.RelativeOrAbsolute));
            }
            lb.SelectedIndex = -1;
        }

        bool islist1busy = false;
        private async void ListBox1_LayoutUpdated(object sender, EventArgs e)
        {
            try
            {
                if (!islist1busy && listbox1.Items.Count > 0 && next1 > 0)
                {
                    islist1busy = true;
                    list1 = await getList(list1, string.Format(url1, next1, DateTime.Now.Millisecond));
                    NotifyPropertyChanged("list1");
                    next1++;
                    islist1busy = false;
                }
            }
            catch { }
            islist1busy = false;
        }

        bool islist2busy = false;
        private async void ListBox2_LayoutUpdated(object sender, EventArgs e)
        {
            try
            {
                if (!islist2busy && listbox2.Items.Count > 0 && next2 > 0)
                {
                    islist2busy = true;
                    list2 = await getList(list2, string.Format(url2, next2, DateTime.Now.Millisecond));
                    NotifyPropertyChanged("list2");
                    next2++;
                    islist2busy = false;
                }
            }
            catch { }
            islist1busy = false;
        }

        bool islist3busy = false;
        private async void ListBox3_LayoutUpdated(object sender, EventArgs e)
        {
            try
            {
                if (!islist3busy && listbox3.Items.Count > 0 && next3 > 0)
                {
                    islist3busy = true;
                    list3 = await getList(list3, string.Format(url3, next3, DateTime.Now.Millisecond));
                    NotifyPropertyChanged("list3");
                    next3++;
                    islist3busy = false;
                }
            }
            catch { }
            islist1busy = false;
        }

        private void mail_Click(object sender, RoutedEventArgs e)
        {
            EmailComposeTask ect = new EmailComposeTask();
            ect.To = "liushuai.baicai@hotmail.com";
            ect.Show();
        }

        private void loveus_Click(object sender, RoutedEventArgs e)
        {
            MarketplaceReviewTask task = new MarketplaceReviewTask();
            task.Show();
        }

        private void denglu_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/ACWZ;component/LoginPage.xaml", UriKind.RelativeOrAbsolute));
        }

        //private void ClearCache_Click(object sender, RoutedEventArgs e)
        //{
        //    CacheFile.ClearCache();

        //    try
        //    {
        //        cachesize.Text = "0.00MB";
        //    }
        //    catch { }

        //    MessageBox.Show("清理完成~");
        //}

        private async void zhuxiao_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("确定要注销么？", "ACFUN文章区", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    await new HttpHelp().Get(StaticData.logout);
                    StaticData.Cookie = null;
                    StaticData.UserName = "";
                    StaticData.UserImg = "";
                    StaticData.IsLogin = false;
                    Settings.AddOrUpdateValue("username", "");
                    Settings.AddOrUpdateValue("password", "");
                    zhuxiao.Visibility = Visibility.Collapsed;
                    denglu.Visibility = Visibility.Visible;
                    name.Text = "";
                    userlogo.Source = new BitmapImage(new Uri("/Assets/Emoji/ac/26.png", UriKind.RelativeOrAbsolute));
                }
            }
            catch { }
        }

        #region 即时绑定数据
        public string Version
        {
            get
            {
                return StaticData.AppVersion;
            }
        }
        public string RandomAC
        {
            get
            {
                return "/Assets/Emoji/ac/" + (new Random(DateTime.Now.Millisecond).Next(54) + 1).ToString("00") + ".png";
            }
        }
        public string RandomAIS
        {
            get
            {
                return "/Assets/Emoji/ais/" + (new Random(DateTime.Now.Millisecond).Next(40) + 1).ToString("00") + ".png";
            }
        }
        //加载条
        public bool IsShowImage
        {
            get { return StaticData.IsShowImage; }
            set
            {
                if (StaticData.IsShowImage != value)
                {
                    StaticData.IsShowImage = value;
                    Settings.AddOrUpdateValue("IsShowImage", value);
                    NotifyPropertyChanged("IsShowImage");
                }
            }
        }
        #endregion

        private void collectionTile_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (!StaticData.IsLogin)
            {
                MessageBox.Show("请先登录");
                return;
            }
            NavigationService.Navigate(new Uri("/ACWZ;component/CollectionPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void mentionsTile_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (!StaticData.IsLogin)
            {
                MessageBox.Show("请先登录");
                return;
            }
            NavigationService.Navigate(new Uri("/ACWZ;component/MentionsPage.xaml", UriKind.RelativeOrAbsolute));
        }
    }
}