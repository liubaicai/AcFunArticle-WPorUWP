using AcFunBlue.Common;
using AcFunBlue.Controls;
using AcFunBlue.DataModels;
using AcFunBlue.Views;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=391641 上有介绍

namespace AcFunBlue
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private int nextPageCount = -1;

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;

            listView.Loaded += listView_Loaded;
        }

        void listView_Loaded(object sender, RoutedEventArgs e)
        {
            var verticalScrollBar = ListViewHelper.GetChildScrollBar(listView);
            if (verticalScrollBar != null)
                verticalScrollBar.ValueChanged += verticalScrollBar_ValueChanged;
        }

        bool isLoadedMore = false;
        private async void verticalScrollBar_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            ScrollBar scrollbar = sender as ScrollBar;
            if (e.NewValue > scrollbar.Maximum - 100 && !isLoadedMore && (this.DefaultViewModel["ListData"] as ObservableCollection<ACListItem>).Count>0)
            {
                // Can call API to Load More Items
                isLoadedMore = true;
                Debug.WriteLine("Load more items");
                await GetListMore(Settings.Get<int>("Sort", 1));
                isLoadedMore = false;
            }
        }

        void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            if (!this.Frame.CanGoBack)
                Application.Current.Exit();
        }

        /// <summary>
        /// 获取此 <see cref="Page"/> 的视图模型。
        /// 可将其更改为强类型视图模型。
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// 在此页将要在 Frame 中显示时进行调用。
        /// </summary>
        /// <param name="e">描述如何访问此页的事件数据。
        /// 此参数通常用于配置页。</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.New)
            {
                this.DefaultViewModel["Busy"] = false;
                if(!this.DefaultViewModel.ContainsKey("ListData")||this.DefaultViewModel["ListData"]==null)
                {
                    this.DefaultViewModel["ListData"] = new ObservableCollection<ACListItem>();
                }
                GetListNew(Settings.Get<int>("Sort",1));
            }
            // TODO: 准备此处显示的页面。

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }

        #region 获取列表数据
        private async void GetListNew(int flag)
        {
            string url = "";
            if (flag == 4)
            {
                url = string.Format(StaticData.list4, 1);
                this.DefaultViewModel["Title"] = "漫画小说";
            }
            else if (flag == 3)
            {
                url = string.Format(StaticData.list3, 1);
                this.DefaultViewModel["Title"] = "动漫文化";
            }
            else if (flag == 2)
            {
                url = string.Format(StaticData.list2, 1);
                this.DefaultViewModel["Title"] = "工作情感";
            }
            else
            {
                url = string.Format(StaticData.list1, 1);
                this.DefaultViewModel["Title"] = "综合";
            }
            (this.DefaultViewModel["ListData"] as ObservableCollection<ACListItem>).Clear();
            this.DefaultViewModel["Busy"] = true;
            using (var stream = await new HttpHelper().Get(url))
            {
                var doc = new HtmlDocument();
                doc.Load(stream);
                var node = HtmlHelper.getListNode(doc);
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

                            ACListItem acitem = new ACListItem();
                            acitem.title = listtemp[1].InnerText;
                            acitem.href = listtemp[0].Attributes[2].Value;
                            acitem.dis = listtemp[2].ChildNodes[3].InnerText;
                            acitem.time = listtemp[1].Attributes[3].Value;
                            acitem.name = HtmlHelper.NoHTML(listtemp[2].ChildNodes[1].InnerText);
                            acitem.beizhu = HtmlHelper.NoHTML(listtemp[3].InnerText);
                            (this.DefaultViewModel["ListData"] as ObservableCollection<ACListItem>).Add(acitem);
                        }
                    }
                }
            }
            nextPageCount = 2;
            this.DefaultViewModel["Busy"] = false;
        }
        private async Task GetListMore(int flag)
        {
            string url = "";
            if (flag == 4)
            {
                url = string.Format(StaticData.list4, nextPageCount);
                this.DefaultViewModel["Title"] = "漫画小说";
            }
            else if (flag == 3)
            {
                url = string.Format(StaticData.list3, nextPageCount);
                this.DefaultViewModel["Title"] = "动漫文化";
            }
            else if (flag == 2)
            {
                url = string.Format(StaticData.list2, nextPageCount);
                this.DefaultViewModel["Title"] = "工作情感";
            }
            else
            {
                url = string.Format(StaticData.list1, nextPageCount);
                this.DefaultViewModel["Title"] = "综合";
            }
            await StatusBar.GetForCurrentView().ProgressIndicator.ShowAsync();
            using (var stream = await new HttpHelper().Get(url))
            {
                var doc = new HtmlDocument();
                doc.Load(stream);
                var node = HtmlHelper.getListNode(doc);
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

                            ACListItem acitem = new ACListItem();
                            acitem.title = listtemp[1].InnerText;
                            acitem.href = listtemp[0].Attributes[2].Value;
                            acitem.dis = listtemp[2].ChildNodes[3].InnerText;
                            acitem.time = listtemp[1].Attributes[3].Value;
                            acitem.name = HtmlHelper.NoHTML(listtemp[2].ChildNodes[1].InnerText);
                            acitem.beizhu = HtmlHelper.NoHTML(listtemp[3].InnerText);
                            (this.DefaultViewModel["ListData"] as ObservableCollection<ACListItem>).Add(acitem);
                        }
                    }
                }
            }
            nextPageCount++;
            await StatusBar.GetForCurrentView().ProgressIndicator.HideAsync();
        }
        #endregion

        private void bt_about_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(About));
        }

        private void Up_AppBt_Click(object sender, RoutedEventArgs e)
        {
            ScrollViewer scrollviewer = ListViewHelper.FindVisualChildByName<ScrollViewer>(listView, "ScrollViewer");
            scrollviewer.ChangeView(null, 0, null);
        }

        private void Refresh_AppBt_Click(object sender, RoutedEventArgs e)
        {
            GetListNew(Settings.Get<int>("Sort", 1));
        }

        private async void List_AppBt_Click(object sender, RoutedEventArgs e)
        {
            //var result = await MessageBox.ShowListSelect((uint)Settings.Get<int>("Sort", 1));
            //if (result != (uint)Settings.Get<int>("Sort", 1))
            //{
            //    Settings.Set("Sort", result);
            //    GetListNew(result);
            //}
        }
    }
}
