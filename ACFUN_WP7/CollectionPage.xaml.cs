using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using Liubaicai.Toolkit;
using System.Runtime.Serialization.Json;
using ACFUN.Controls;

namespace ACFUN
{
    public partial class CollectionPage : PhoneApplicationPage, INotifyPropertyChanged
    {
        public ObservableCollection<ACItem> collectionlist { get; set; }

        string collectionUrl = StaticData.collection;

        int page = 1;

        public CollectionPage()
        {
            collectionlist = new ObservableCollection<ACItem>();
            InitializeComponent();
            DataContext = this;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.New)
            {
                collectionlist.Clear();
                page = 1;
                await getList();
            }
        }

        private async void delcolbt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("若确信继续当前操作，请点击[确定]按钮，反之则请点击[取消]按钮", "取消收藏", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                {
                    return;
                }
                ButtonEx lb = sender as ButtonEx;
                HttpHelp httpHelp = new HttpHelp();
                httpHelp.parameters.Add("cId", lb.Tag.ToString().Remove(0, 5));
                httpHelp.parameters.Add("operate", "0");
                using (var stream = await httpHelp.Post(StaticData.tocollection))
                {
                    DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(tocollectionresult));
                    var result = json.ReadObject(stream) as tocollectionresult;
                    if (result.success)
                    {
                        int tag = -1;
                        for (int i = 0; i < collectionlist.Count; i++)
                        {
                            if (lb.Tag.ToString() == collectionlist[i].href)
                            {
                                tag = i;
                            }
                        }
                        if (tag != -1)
                        {
                            collectionlist.RemoveAt(tag);
                        }
                    }
                    else
                    {
                        MessageBox.Show("删除收藏失败");
                    }
                }
            }
            catch { }
        }

        private void EasyListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EasyListBox lb = sender as EasyListBox;
            if (lb.SelectedIndex != -1)
            {
                ACItem item = lb.SelectedItem as ACItem;
                StaticData.acitem = item;
                NavigationService.Navigate(new Uri("/DetailPage.xaml", UriKind.RelativeOrAbsolute));
            }
            lb.SelectedIndex = -1;
        }

        bool islistbusy = false;
        private async void EasyListBox_StretchingBottom(object sender, EventArgs e)
        {
            if (!islistbusy && collectionlist.Count > 0 && page > 0)
            {
                await getList();
            }
        }

        private async Task getList()
        {
            islistbusy = true;
            indicator.IsVisible = true;
            try
            {
                using (Stream stream = await new HttpHelp().Get(string.Format(StaticData.collection,page)))
                {
                    StreamReader sr = new StreamReader(stream);
                    string str = sr.ReadToEnd();
                    JObject result = JObject.Parse(str);

                    if (!(bool)result["success"])
                    {
                        if ((string)result["message"] != null)
                        {
                            MessageBox.Show((string)result["message"]);
                        }
                        indicator.IsVisible = false;
                        return;
                    }

                    int totalPage = (int)result["totalpage"];

                    foreach (JObject content in (result["contents"] as JArray))
                    {
                        ACItem item = new ACItem();
                        item.title = (string)content["title"];
                        item.href = (string)content["url"];
                        item.dis = (string)content["comments"];
                        item.time = (string)content["releaseDate"];
                        item.name = (string)content["username"];
                        item.beizhu = (string)content["description"];
                        collectionlist.Add(item);
                    }

                    if (page < totalPage)
                    {
                        page++;
                    }
                    else
                    {
                        page = 0;
                    }
                }
            }
            catch(Exception) { }
            islistbusy = false;
            indicator.IsVisible = false;
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
    }
}