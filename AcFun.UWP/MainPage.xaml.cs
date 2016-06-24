using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using AcFun.UWP.Control;
using AcFun.UWP.Helper;
using AcFun.UWP.Model;
using AcFun.UWP.Pages;
using Baicai.UWP.Tools.Helpers;
using BaicaiMobileService.Helper;
using User = AcFun.UWP.Module.User;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AcFun.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        public static MainPage Instance { get; set; }

        public Frame SFrame => SecondFrame;

        public ProgressRing SFrameProgressRing => SecondFrameProgressRing;

        public NotRepeatObservableCollection<ChannelBindingModel> ChannelListData { get; set; } =
            new NotRepeatObservableCollection<ChannelBindingModel>();
        private int PageCount { get; set; } = 1;

        private int ChannelId
        {
            get
            {
                return Settings.Get("ChannelId", 110);
            }
            set
            {
                Settings.Set("ChannelId", value);
                NotifyPropertyChanged("ChannelTitle");
            }
        }

        public string ChannelTitle
        {
            get
            {
                switch (ChannelId)
                {
                    case 73:
                        return "工作·情感";
                    case 74:
                        return "动漫·文化";
                    case 75:
                        return "漫画·轻小说";
                    default:
                        return "综合";
                }
            }
        }

        public static readonly DependencyProperty IsCanCloseSplitViewProperty
            = DependencyProperty.Register("IsCanCloseSplitView", typeof(bool), typeof(MainPage), null);
        public bool IsCanCloseSplitView
        {
            get { return (bool)GetValue(IsCanCloseSplitViewProperty); }
            set { SetValue(IsCanCloseSplitViewProperty, value); }
        }

        public MainPage()
        {
            this.NavigationCacheMode = NavigationCacheMode.Required;
            Instance = this;
            this.DataContext = this;
            this.InitializeComponent();
            if (PlatformHelper.IsMobile)
            {
                IsCanCloseSplitView = true;
                FirstFrame.Width = Window.Current.Bounds.Width;
                SecondGrid.Visibility = Visibility.Collapsed;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.NavigationMode == NavigationMode.New)
            {
                GetChannelListData();
                AutoUserLogin();
            }
        }

        private async void AutoUserLogin()
        {
            await User.AutoLoginTask();
        }

        private async void GetChannelListData(bool isRefresh = false)
        {
            if (isRefresh)
            {
                PageCount = 1;
            }
            var http = Http.Instance;
            var str = await http.GetStringAsync(string.Format(AppData.ContentChannelUrl, ChannelId, PageCount));
            var obj = str.ToJsonObject<ChannelResult.Rootobject>();
            if (isRefresh)
            {
                ChannelListData.Clear();
                FirstFrameProgressRing.IsActive = true;
            }
            var data = obj?.Data?.List;
            if (data != null&& data.Any())
            {
                foreach (var item in data)
                {
                    ChannelListData.Add(item.ContentId, item);
                }
                PageCount++;
            }
            FirstFrameProgressRing.IsActive = false;
        }

        private void HamburgButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (IsCanCloseSplitView)
            {
                SplitViewControl.IsPaneOpen = !SplitViewControl.IsPaneOpen;
            }
        }

        private void ListView_OnBottomArrived(object sender, EventArgs e)
        {
            GetChannelListData();
        }

        private async void ListView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var channelBindingModel = e.ClickedItem as ChannelBindingModel;
            if (channelBindingModel != null)
            {
                if (PlatformHelper.IsMobile)
                {
                    FirstFrameProgressRing.IsActive = true;
                    var result = await Cache.GetCachedHtml(channelBindingModel.ContentId);
                    FirstFrameProgressRing.IsActive = false;
                    if (!string.IsNullOrEmpty(result))
                    {
                        channelBindingModel.NotifyPropertyChanged("TitleForeground");
                        var obj = result.ToJsonObject<InfoResult.Rootobject>();
                        if (obj.Success)
                        {
                            App.RootFrame.Navigate(typeof(ContentPage), obj.Data.FullArticle);
                        }
                        else
                        {
                            await new MessageDialog(obj.Msg).ShowAsync();
                        }
                    }
                }
                else
                {
                    while (SecondFrame.CanGoBack)
                    {
                        SecondFrame.GoBack();
                    }
                    SecondFrameProgressRing.IsActive = true;
                    var result = await Cache.GetCachedHtml(channelBindingModel.ContentId);
                    if (!string.IsNullOrEmpty(result))
                    {
                        channelBindingModel.NotifyPropertyChanged("TitleForeground");
                        var obj = result.ToJsonObject<InfoResult.Rootobject>();
                        if (obj.Success)
                        {
                            SecondFrame.Navigate(typeof(ContentPage), obj.Data.FullArticle);
                        }
                        else
                        {
                            await new MessageDialog(obj.Msg).ShowAsync();
                        }
                    }
                    SecondFrameProgressRing.IsActive = false;
                }
            }
        }

        private async void ChannelButton_Click(object sender, RoutedEventArgs e)
        {
            //MessageDialog md = new MessageDialog("选择频道");
            //if (ChannelId != 110)
            //    md.Commands.Add(new UICommand("综合", ChannelSelectAction, 110));
            //if (ChannelId != 73)
            //    md.Commands.Add(new UICommand("工作·情感", ChannelSelectAction, 73));
            //if (ChannelId != 74)
            //    md.Commands.Add(new UICommand("动漫·文化", ChannelSelectAction, 74));
            //if (!PlatformHelper.IsMobile)
            //{
            //    if (ChannelId != 75)
            //        md.Commands.Add(new UICommand("漫画·轻小说", ChannelSelectAction, 75));
            //}
            //await md.ShowAsync();

            var channelBox = new ChannelBox();
            channelBox.ChannelSelectedAction = i =>
            {
                ChannelId = i;
                GetChannelListData(true);
            };
            await channelBox.ShowAsync();
        }

        //private void ChannelSelectAction(IUICommand command)
        //{
        //    ChannelId = (int)command.Id;
        //    GetChannelListData(true);
        //}

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            GetChannelListData(true);
        }

        private void SettingButtonOnClick(object sender, RoutedEventArgs e)
        {
            if (PlatformHelper.IsMobile)
            {
                App.RootFrame.Navigate(typeof(SettingPage));
            }
            else
            {
                PopupFrame.Instance350.Show(typeof(SettingPage));
            }
        }

        private async void UserButtonOnClick(object sender, RoutedEventArgs e)
        {
            if (User.IsLogin)
            {
                if (PlatformHelper.IsMobile)
                {
                    App.RootFrame.Navigate(typeof(UserCenter));
                }
                else
                {
                    PopupFrame.Instance.Show(typeof(UserCenter));
                }
            }
            else
            {
                LoginBox lb = new LoginBox();
                lb.LoginCompleted += (o, rootobject) =>
                {
                    Debug.WriteLine("欢迎你:" + rootobject.Username);
                };
                await lb.ShowAsync();
            }
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            GetChannelListData(true);
        }

        private async void marktReviewButton_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-windows-store://review/?ProductId=9WZDNCRDPRZP"));
        }

        private void HideSplitViewPanel()
        {
            if (IsCanCloseSplitView)
            {
                SplitViewControl.IsPaneOpen = false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void SplitViewControl_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (IsCanCloseSplitView)
            {
                SplitViewControl.IsPaneOpen = true;
            }
        }

        private void SplitViewControl_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            HideSplitViewPanel();
        }
    }
}
