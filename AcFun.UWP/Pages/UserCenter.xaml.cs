using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
using AcFun.UWP.Module;
using AcFunJX.UWP.Util;
using BaicaiMobileService.Helper;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace AcFun.UWP.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UserCenter : Page
    {
        private int CollectionPageCount { get; set; } = 1;
        private int AtPageCount { get; set; } = 1;

        public UserCenter()
        {
            this.NavigationCacheMode = NavigationCacheMode.Required;
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.NavigationMode == NavigationMode.New)
            {
                LoadHuangLi();
                GetCollectionListData(true);
            }
        }

        private void LoadHuangLi()
        {
            var good = new List<ListItem>();
            var bad = new List<ListItem>();
            CommUtils.getTableItemArray(ref good, ref bad);
            GoodListBox.ItemsSource = good;
            BadListBox.ItemsSource = bad;
            var f = CommUtils.getFortune();
            var ls = CommUtils.getFortune(f);

            int i = new Random(DateTime.Now.Millisecond).Next(54);

            dateTextBlock.Text = DateTime.Now + " " + CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(DateTime.Now.DayOfWeek);
            ndateTextBlock.Text = LunarUtil.GetLunarDay(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            jxTextBlock.Text = ls;
        }

        private async void GetAtListData(bool isRefresh = false)
        {
            if (isRefresh)
            {
                AtPageCount = 1;
            }
            var http = Http.Instance;
            var url = string.Format(AppData.AtListUrl, AtPageCount);
            var str = await http.GetStringAsync(url);
            var obj = str.ToJsonObject<AtListResult.Rootobject>();
            if (isRefresh)
            {
                User.CollectionListViewData.Clear();
            }
            var data = obj?.Data?.Page?.CommentList;
            if (data != null && data.Any())
            {
                foreach (var item in data)
                {
                    User.AtListViewData.Add(item.ContentId, item);
                }
                AtPageCount++;
            }
        }

        private async void GetCollectionListData(bool isRefresh = false)
        {
            try
            {
                if (isRefresh)
                {
                    CollectionPageCount = 1;
                }
                var http = Http.Instance;
                var str = await http.GetStringAsync(string.Format(AppData.CollectionListUrl, CollectionPageCount));
                var obj = str.ToJsonObject<CollectionListResult.Rootobject>();
                if (isRefresh)
                {
                    User.CollectionListViewData.Clear();
                }
                var data = obj?.Contents;
                if (data != null && data.Any())
                {
                    foreach (var item in data)
                    {
                        User.CollectionListViewData.Add(item.Cid, item);
                    }
                    CollectionPageCount++;
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
            }
        }

        private async void CheckIn_OnClick(object sender, RoutedEventArgs e)
        {
            using (var stream = await Http.Instance.GetStreamAsync(AppData.CheckInUrl))
            {
                var obj = stream.ToJsonObject<CommonResult.Rootobject>();
                ToastPrompt.Show(obj.Success ? "签到成功" : obj.Result);
            }
        }

        private async void CollectionListView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var model = e.ClickedItem as CollectionListResult.Content;
            if (model != null)
            {
                while (MainPage.Instance.SFrame.CanGoBack)
                {
                    MainPage.Instance.SFrame.GoBack();
                }
                MainPage.Instance.SFrameProgressRing.IsActive = true;
                var result = await Cache.GetCachedHtml(model.Cid);
                if (!string.IsNullOrEmpty(result))
                {
                    var obj = result.ToJsonObject<InfoResult.Rootobject>();
                    MainPage.Instance.SFrame.Navigate(typeof(ContentPage), obj.Data.FullArticle);
                    //PopupFrame.Instance.Hide();
                }
                MainPage.Instance.SFrameProgressRing.IsActive = false;
            }
        }

        private void CollectionListView_OnBottomArrived(object sender, EventArgs e)
        {
            GetCollectionListData();
        }
    }
}
