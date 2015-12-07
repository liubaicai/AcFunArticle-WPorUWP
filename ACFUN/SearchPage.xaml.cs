using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using System.Windows.Input;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.IO;
using Newtonsoft.Json.Linq;

namespace ACFUN
{
    public partial class SearchPage : PhoneApplicationPage, INotifyPropertyChanged
    {
        public ObservableCollection<ACItem> listdata { get; set; }
        int nextcount = 1;

        public SearchPage()
        {
            InitializeComponent();
            listdata = new ObservableCollection<ACItem>();
            DataContext = this;
            this.Loaded += SearchPage_Loaded;
        }

        async void SearchPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (NavigationContext.QueryString.ContainsKey("query") && listdata.Count<=0)
            {
                searchbox.Text = HttpUtility.UrlDecode(NavigationContext.QueryString["query"]);
                await Search(searchbox.Text,1);
            }
        }

        private async void searchbox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                if (searchbox.Text != "")
                {
                    await Search(searchbox.Text, 1);
                }
            }
        }

        private async void search_Click(object sender, RoutedEventArgs e)
        {
            if (searchbox.Text != "")
            {
                await Search(searchbox.Text, 1);
            }
        }

        private async Task Search(string query,int count)
        {
            var page =1;
            if (count == 1)
                listdata.Clear();
            else
                page = count;

            try
            {
                indicator.IsVisible = true;
                using (var stream = await new HttpHelp().Get(string.Format(StaticData.search,query,page)))
                {
                    var json = JObject.Parse(await new StreamReader(stream).ReadToEndAsync());
                    if (nextcount < Int32.Parse(json["totalpage"].ToString()))
                    {
                        nextcount++;
                    }
                    var array = json["contents"] as JArray;
                    foreach (var item in array)
                    {//ACItem(string title, string url, string dis, string time, string name, string beizhu)
                        listdata.Add(new ACItem(
                            item["title"].ToString(),
                            "/a/ac" + item["id"].ToString(),
                            item["comments"].ToString(),
                            TimeFuc.getTime(long.Parse(item["releaseDate"].ToString())).ToString(),
                            item["username"].ToString(),
                            item["sign"].ToString()));
                    }
                }
            }
            catch { }
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

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var lb = sender as ListBox;
            if (lb.SelectedIndex != -1)
            {
                var item = lb.SelectedItem as ACItem;
                StaticData.acitem = item;
                NavigationService.Navigate(new Uri("/DetailPage.xaml", UriKind.RelativeOrAbsolute));
            }
            lb.SelectedIndex = -1;
        }

        bool islistbusy = false;
        private async void ListBox_StretchingBottom(object sender, EventArgs e)
        {
            try
            {
                if (!islistbusy && listbox.Items.Count > 0 && nextcount > 1)
                {
                    islistbusy = true;
                    await Search(searchbox.Text, nextcount);
                    islistbusy = false;
                }
            }
            catch { }
            islistbusy = false;
        }
    }
}