using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using AcFun.UWP.Control;
using AcFun.UWP.Helper;
using AcFun.UWP.Model;
using Baicai.UWP.Tools.Helpers;
using BaicaiMobileService.Helper;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace AcFun.UWP.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class CommentPage : Page
    {
        public NotRepeatObservableCollection<CommentBindingModel> CommentListData { get; set; } = new NotRepeatObservableCollection<CommentBindingModel>();
        private int PageCount { get; set; }

        private int TotalPage { get; set; }

        private int Id { get; set; }

        private bool IsBusy { get; set; } = false;

        public IDictionary<int, CommentBindingItemModel> CommentContentList = new Dictionary<int, CommentBindingItemModel>();

        public static CommentPage Instance { get; set; }

        public CommentPage()
        {
            this.NavigationCacheMode = NavigationCacheMode.Required;
            this.InitializeComponent();
            this.DataContext = this;
            Instance = this;
            if (!PlatformHelper.IsMobile)
            {
                ContentGrid.MaxWidth = 800;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.NavigationMode == NavigationMode.New)
            {
                Id = int.Parse(e.Parameter?.ToString());
                if (Id > 0)
                {
                    Refresh();
                }
            }
        }

        public async void Refresh()
        {
            await LoadMore(1);
        }

        private async Task LoadMore(int page)
        {
            if (IsBusy)
            {
                return;
            }
            try
            {
                IsBusy = true;
                CommentListData.Clear();
                CommentProgressRing.IsActive = true;
                var url = string.Format(AppData.CommentListUrl, Id, page);
                var str = await Http.Instance.GetStringAsync(url);
                var obj = str.ToJsonObject<CommentListResult.Rootobject>();
                TotalPage = (int)obj.Data.TotalPage;
                PageCount = (int) obj.Data.Page;
                foreach (var model in obj.Data.CommentContentList)
                {
                    if (!CommentContentList.ContainsKey(model.Key))
                    {
                        CommentContentList.Add(model);
                    }
                }
                foreach (var commentId in obj.Data.CommentList)
                {
                    CommentListData.Add(int.Parse(commentId), new CommentBindingModel() { CommentId = int.Parse(commentId), CommentContentList = CommentContentList });
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
            }
            IsBusy = false;
            CommentProgressRing.IsActive = false;
        }

        private async void FirstButton_ObClick(object sender, RoutedEventArgs e)
        {
            await LoadMore(1);
        }

        private async void PreviousButton_ObClick(object sender, RoutedEventArgs e)
        {
            if (PageCount > 1)
            {
                await LoadMore(PageCount - 1);
            }
        }

        private async void NextButton_ObClick(object sender, RoutedEventArgs e)
        {
            if (TotalPage> PageCount)
            {
                await LoadMore(PageCount + 1);
            }
        }

        private async void LastButton_ObClick(object sender, RoutedEventArgs e)
        {
            await LoadMore(TotalPage);
        }

        private async void RefreshButton_ObClick(object sender, RoutedEventArgs e)
        {
            await LoadMore(1);
        }

        private void CommentListItem_OnClick(object sender, ItemClickEventArgs e)
        {
            var model = e.ClickedItem as CommentBindingModel;
            if (model?.CommentContentList[model.CommentId].ParentId > 0)
            {
                if (PlatformHelper.IsMobile)
                {
                    App.RootFrame.Navigate(typeof(CommentListPage), e.ClickedItem);
                }
                else
                {
                    PopupFrame.Instance.Show(typeof(CommentListPage), e.ClickedItem);
                }
            }
        }
    }
}
