using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Windows.ApplicationModel.Store;
using Microsoft.Phone.Controls;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Microsoft.Phone.Info;
using Microsoft.Phone.Tasks;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Net.NetworkInformation;
using Toast;
using System.Threading;
using HttpExtensions;
using System.Windows.Controls.Primitives;
using ACFUN.Controls;
using System.Windows.Input;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using YeepayLibrary.NetCommunication.Common.Helper;

namespace ACFUN
{
    public partial class MainPage : PhoneApplicationPage, INotifyPropertyChanged
    {
        public ObservableCollection<ACItem> listdata { get; set; }
        int nextcount = -1;
        private int PageCount { get; set; } = 1;

        public ObservableCollection<Apps> apps { get; set; }

        // 构造函数
        public MainPage()
        {
            listdata = new ObservableCollection<ACItem>();
            apps = new ObservableCollection<Apps>();
            InitializeComponent();
            DataContext = this;

            // 用于本地化 ApplicationBar 的示例代码
            //BuildLocalizedApplicationBar();

            this.Loaded += MainPage_Loaded;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Debug.WriteLine("MainPage:OnNavigatedFrom");
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            Debug.WriteLine("MainPage:OnNavigatedTo");
            if (StaticData.IsLogin)
            {
                denglu.Visibility = Visibility.Collapsed;
                zhuce.Visibility = Visibility.Collapsed;
                name.Text = StaticData.UserName;
                userlogo.Source = new BitmapImage(new Uri(StaticData.UserImg, UriKind.RelativeOrAbsolute));
                zhuxiao.Visibility = Visibility.Visible;
            }

            if (listdata.Count>0)
            {
                foreach (var acItem in listdata)
                {
                    acItem.NotifyPropertyChanged("opacity");
                }
            }

            if (e.NavigationMode == NavigationMode.New)
            {
                try
                {
                    switch (Settings.GetValueOrDefault<int>("Sort", 1))
                    {
                        case 1:
                            header.Text = "文章·综合";
                            break;
                        case 2:
                            header.Text = "工作·情感";
                            break;
                        case 3:
                            header.Text = "动漫·文化";
                            break;
                        case 4:
                            header.Text = "漫画·小说";
                            break;
                    }

                    autoLogin();

                    //CheckVersion();
                }
                catch { }
            }

            try
            {
                double size = await CacheFile.GetCacheSize();
                if (size>10240000)
                {
                    CacheFile.ClearCache();
                }
                //cachesize.Text = (size / 1024 / 1024).ToString("0.00") + "MB";
            }
            catch { }


            if (NetworkInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
            {
                net.Text = "wifi连接，建议开启图片显示";
            }
            else
            {
                net.Text = "手机网络连接，建议关闭图片显示";
            }
        }

        async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (listdata.Count <= 0)
                {
                    await getListNew();
                }
                //sendDevice();
                //if (DateTime.Now.Year == 2015 && DateTime.Now.Month == 2 && DateTime.Now.Day == 19)
                //{
                //    searchbox.Text =
                //        Convert.ToBase64String(DeviceExtendedProperties.GetValue("DeviceUniqueId") as byte[])
                //            .Substring(0, 6);
                //}
            }
            catch { }
        }

        private async void CheckVersion()
        {
            using (var stream = await new HttpHelp().Get("http://version.liubaicai.net/acfun.html"))
            {
                if (stream != null)
                {
                    var sr = new StreamReader(stream);
                    var version = sr.ReadToEnd();
                    if (version != "" && Int32.Parse(version) > App.AppVersionCount)
                    {
                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            var toast = new ToastPrompt();
                            toast.Click += (sender, args) =>
                            {
                                var task = new MarketplaceDetailTask();
                                task.ContentIdentifier = "393cf4cf-4946-4721-bee5-fb326195dab8";
                                task.ContentType = MarketplaceContentType.Applications;
                                task.Show();
                            };
                            toast.Show("新版本出没，更新下吧~");
                        });
                    }
                }
            }
        }

        private async void autoLogin()
        {
            try
            {
                if (!StaticData.IsLogin && Settings.GetValueOrDefault("username", "") != "" && Settings.GetValueOrDefault("password", "") != "")
                {
                    var httpHelp = new HttpHelp();
                    httpHelp.parameters.Add("username", Settings.GetValueOrDefault("username", ""));
                    httpHelp.parameters.Add("password", Settings.GetValueOrDefault("password", ""));
                    using (var stream = await httpHelp.PostSetCookie(
                        string.Format(StaticData.login, 
                        Settings.GetValueOrDefault("username", ""), 
                        Settings.GetValueOrDefault("password", ""))))
                    {
                        var json = new DataContractJsonSerializer(typeof(loginresult));
                        var result = json.ReadObject(stream) as loginresult;
                        if (result.success)
                        {
                            StaticData.UserName = result.username;
                            StaticData.UserImg = result.img;
                            StaticData.IsLogin = true;
                            denglu.Visibility = Visibility.Collapsed;
                            zhuce.Visibility = Visibility.Collapsed;
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
                var pivot = sender as Pivot;
                switch (pivot.SelectedIndex)
                {
                    case 1:
                        break;
                    case 2:
                        await getunRead();
                        break;
                }
            }
            catch { }
        }

        private async void getApps()
        {
            try
            {
                using (Stream stream = await new HttpHelp().Get("http://bms.liubaicai.net/apps.ashx?msid=393cf4cf-4946-4721-bee5-fb326195dab8"))
                {
                    DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(List<Apps>));
                    var result = json.ReadObject(stream) as List<Apps>;
                    if (result != null && result.Count > 0)
                    {
                        apps.Clear();
                        foreach (var item in result)
                        {
                            apps.Add(item);
                        }
                    }
                }
            }
            catch { }
        }

        private async void sendDevice()
        {
            try
            {
                var str = string.Format("http://bms.liubaicai.net/device.ashx?flag={0}&id={1}", "acfun",
                    HttpUtility.UrlEncode(Convert.ToBase64String(DeviceExtendedProperties.GetValue("DeviceUniqueId") as byte[])));
                using (Stream stream = await new HttpHelp().Get(str))
                {
                }
            }
            catch { }
        }

        public static List<Apps> SelectApp(string id)
        {
            var list = new List<Apps>();
            if (id == "all")
            {
                list.Add(AppMethod.CreateApp(
                    "http://pic.weifengke.com/attachments/1/1746/e0f3de26c3098182ad6f58fff7ab7d3a.jpg",
                    "56c55fbc-7dc7-42cd-87de-55143ece690b",
                    "数据开关",
                    "最最最最便捷的数据/手机网络开关"));
                list.Add(AppMethod.CreateApp(
                    "http://pic.weifengke.com/attachments/1/1792/e2dfdcb33449eca927c54e353b24ca06.jpg",
                    "3de1c61d-b456-48a3-bedb-04201a1cdf17",
                    "哎？手电筒",
                    "最快捷的手电筒要啥自行车啊"));
            }
            return list;
        }

        private async Task getunRead()
        {
            try
            {
                if (StaticData.IsLogin)
                {
                    using (var stream = await new HttpHelp().Get(StaticData.unRead))
                    {
                        var json = new DataContractJsonSerializer(typeof(unRead));
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

        private async Task getListNew()
        {
            await getList(true);
        }

        private async Task getListNext()
        {
            await getList();
        }

        private async Task getList(bool isRef = false)
        {
            indicator.IsVisible = true;
            if (isRef)
            {
                PageCount = 1;
                listdata.Clear();
            }
            var _sort = Settings.GetValueOrDefault("Sort", 1);
            var url = "";
            switch (_sort)
            {
                case 1:
                    header.Text = "文章·综合";
                    url = string.Format(StaticData.list1, PageCount, (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000);
                    break;
                case 2:
                    header.Text = "工作·情感";
                    url = string.Format(StaticData.list2, PageCount, (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000);
                    break;
                case 3:
                    header.Text = "动漫·文化";
                    url = string.Format(StaticData.list3, PageCount, (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000);
                    break;
                case 4:
                    header.Text = "漫画·小说";
                    url = string.Format(StaticData.list4, PageCount, (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000);
                    break;
            }

            Debug.WriteLine(url);
            var request = (HttpWebRequest)WebRequest.Create(new Uri(url));
            request.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; WOW64) Chrome/45.0.2454.101";
            request.Headers["deviceType"] = "1";
            var response = await request.GetResponseAsync();
            using (var stream = response.GetResponseStream())
            {
                var result = new StreamReader(stream).ReadToEnd();
                Debug.WriteLine(result);
                var obj = JObject.Parse(result);
                var pages = obj["data"]["list"];
                
                foreach (var item in pages)
                {
                    var acitem = new ACItem();
                    acitem.title = item["title"].ToString();
                    acitem.href = "/v/ac" + item["contentId"].ToString();
                    acitem.dis = item["comments"].ToString();
                    acitem.time = item["releaseDate"].ToString();
                    acitem.name = item["user"]["username"].ToString();
                    acitem.beizhu = item["description"].ToString();
                    listdata.Add(acitem);
                }
            }
            PageCount++;
            indicator.IsVisible = false;
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
            var lb = sender as ListBox;
            if (lb.SelectedIndex != -1)
            {
                var item = lb.SelectedItem as ACItem;
                StaticData.acitem = item;
                NavigationService.Navigate(new Uri("/DetailPage.xaml",UriKind.RelativeOrAbsolute));
            }
            lb.SelectedIndex = -1;
        }

        bool islistbusy = false;
        private async void ListBox_StretchingBottom(object sender, EventArgs e)
        {
            try
            {
                if (!islistbusy && listbox.Items.Count > 0 && PageCount > 1)
                {
                    islistbusy = true;
                    await getListNext();
                    NotifyPropertyChanged("listdata");
                    islistbusy = false;
                }
            }
            catch { }
            islistbusy = false;
        }

        private void mail_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText("373715806");
            MessageBox.Show("群号已复制");
            //var ect = new EmailComposeTask();
            //ect.To = "liushuai.baicai@hotmail.com";
            //ect.Show();
        }

        private void loveus_Click(object sender, RoutedEventArgs e)
        {
            var task = new MarketplaceReviewTask();
            task.Show();
        }

        private void denglu_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/LoginPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void reg_Click(object sender, RoutedEventArgs e)
        {
            var task = new WebBrowserTask();
            task.Uri = new Uri(StaticData.reg);
            task.Show();
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
                    zhuce.Visibility = Visibility.Visible;
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
                return App.AppVersion;
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
            get
            {
                return Settings.GetValueOrDefault("IsShowImage", false);
            }
            set
            {
                Settings.AddOrUpdateValue("IsShowImage", value);
            }
        }

        public bool IsTileColor
        {
            get
            {
                return Settings.GetValueOrDefault("IsTileColor", true);
            }
            set
            {
                Settings.AddOrUpdateValue("IsTileColor", value);
                ShellTileHelper.ToTileHelper(value);
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
            NavigationService.Navigate(new Uri("/CollectionPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void mentionsTile_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (!StaticData.IsLogin)
            {
                MessageBox.Show("请先登录");
                return;
            }
            newmention.Visibility = Visibility.Collapsed;
            NavigationService.Navigate(new Uri("/MentionsPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private async void refresh_Click(object sender, EventArgs e)
        {
            try
            {
                switch (mainpivot.SelectedIndex)
                {
                    case 0:
                        islistbusy = true;
                        await getListNew();
                        islistbusy = false;
                        break;
                    case 2:
                        await getunRead();
                        break;
                }
            }
            catch { }
        }

        private void sort_Click(object sender, EventArgs e)
        {
            var p = new Popup();
            var box = new SortBox();
            box.SortSelectCompleted += new EventHandler<SortSelectEventArgs>(async (sd, ea) =>
            {
                p.IsOpen = false;
                if (ea.IsChanged)
                {
                    islistbusy = true;
                    await getListNew();
                    islistbusy = false;
                }
            });
            p.Child = box;
            p.IsOpen = true;
        }

        private void bjchange_Click(object sender, EventArgs e)
        {
            if (Settings.GetValueOrDefault<int>("mode", 0) == 0)
            {
                Settings.AddOrUpdateValue("mode",1);
                ThemeManager.ToLightTheme();
            }
            else
            {
                Settings.AddOrUpdateValue("mode", 0);
                ThemeManager.ToDarkTheme();
            }
        }

        #region back按钮事件
        bool isBackQuit = false;
        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            try
            {
                while (this.NavigationService.CanGoBack)
                {
                    this.NavigationService.RemoveBackEntry();
                }
                if (isBackQuit)
                {
                    e.Cancel = false;
                    Application.Current.Terminate();
                }
                else
                {
                    e.Cancel = true;
                    isBackQuit = true;
                    var toast = new ToastPrompt();
                    toast.Show("再按一次后退键退出程序");
                    var t = new Thread(backWait);
                    t.Start();
                }
            }
            catch { e.Cancel = true; }
        }
        private void backWait()
        {
            Thread.Sleep(3000);
            isBackQuit = false;
        }
        #endregion

        private void AppListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var lb = sender as ListBox;
            if (lb.SelectedIndex != -1)
            {
                var item = lb.SelectedItem as Apps;
                var task = new MarketplaceDetailTask();
                task.ContentIdentifier = item.msid;
                task.ContentType = MarketplaceContentType.Applications;
                task.Show();
            }
            lb.SelectedIndex = -1;
        }

        private async void sign_Click(object sender, RoutedEventArgs e)
        {
            using (var stream  = await new HttpHelp().Get(StaticData.checkin))
            {
                var json = new DataContractJsonSerializer(typeof(ChickinResult));
                var result = json.ReadObject(stream) as ChickinResult;
                new ToastPrompt().Show(result.success ? "签到成功" : result.result);
            }
        }

        private async void SupputMe_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                try
                {
                    CurrentApp.ReportProductFulfillment("SupportMe");
                }
                catch
                {
                }
                var result = await CurrentApp.RequestProductPurchaseAsync("SupportMe", true);
                CurrentApp.ReportProductFulfillment("SupportMe");
            }
            catch
            {
            }
        }
    }
}