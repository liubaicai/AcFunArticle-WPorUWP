using System.Linq;
using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Windows.Storage;
using HttpExtensions;
using Microsoft.Phone.Shell;
using Newtonsoft.Json.Linq;

namespace ACFUN
{
    public class ShellTileHelper
    {
        public static void ToTileHelper(bool isTileColor)
        {
            var tileToFind = ShellTile.ActiveTiles.First();
            if (tileToFind != null)
            {
                var flipTile = new FlipTileData
                {
                    SmallBackgroundImage =
                        new Uri("/Assets/Tiles/FlipCycleTileSmall" + (isTileColor ? "" : "-W") + ".png", UriKind.Relative),
                    BackgroundImage = new Uri("/Assets/Tiles/FlipCycleTileMedium" + (isTileColor ? "" : "-W") + ".png", UriKind.Relative),

                };
                tileToFind.Update(flipTile);
            }
        }
    }

    public interface INavigationBase
    {
    }
    public interface INavigatedViewModel : INavigationBase
    {
        void OnNavigatedTo(NavigationEventArgs e);
        void OnNavigatedFrom(NavigationEventArgs e);
    }
    public interface INavigatingViewModel : INavigationBase
    {
        void OnNavigatingFrom(NavigatingCancelEventArgs e);
    }
    public class Navi
    {
        private INavigationBase _currentVM;
        private static PhoneApplicationFrame frame;
        public static event System.Windows.Navigation.NavigatingCancelEventHandler Navigating;
        public Navi(PhoneApplicationFrame frame)
        {
            frame.Navigated += onRootFrameNavigated;
            frame.Navigating += onRootFrameNavigating;
            //frame.NavigationFailed += onRootFrameNavigationFailed;
            //frame.NavigationStopped += onRootFrameNavigationStopped;
        }

        public static bool CanGoBack
        {
            get
            {
                return EnsureMainFrame() && frame.CanGoBack;
            }
        }

        public static void GoBack()
        {
            if (EnsureMainFrame() && frame.CanGoBack)
            {
                frame.GoBack();
            }
        }

        private static bool EnsureMainFrame()
        {
            if (frame != null)
            {
                return true;
            }

            frame = Application.Current.RootVisual as PhoneApplicationFrame;
            if (frame != null)
            {
                frame.Navigating += (s, e) =>
                {
                    if (Navigating != null)
                    {
                        Navigating(s, e);
                    }
                };
            }
            return false;
        }

        private void onRootFrameNavigated(object sender, NavigationEventArgs e)
        {
            try
            {
                if (_currentVM != null)
                {
                    if (_currentVM is INavigatedViewModel)
                    {
                        (_currentVM as INavigatedViewModel).OnNavigatedFrom(e);
                    }
                    _currentVM = null;
                }

                var page = e.Content as PhoneApplicationPage;
                if (page != null && page.DataContext is INavigationBase)
                {
                    _currentVM = page.DataContext as INavigationBase;

                    if (_currentVM is INavigatedViewModel)
                    {
                        (_currentVM as INavigatedViewModel).OnNavigatedTo(e);
                    }
                }
            }
            catch { }

        }

        private void onRootFrameNavigating(object sender, NavigatingCancelEventArgs e)
        {
            try
            {
                if (_currentVM is INavigatingViewModel)
                {
                    (_currentVM as INavigatingViewModel).OnNavigatingFrom(e);
                }
            }
            catch { }

        }

        private static void GetPhoneFrameRoot()
        {
            if (frame == null)
            {
                frame = Application.Current.RootVisual as PhoneApplicationFrame;
                if (frame == null)
                {
                    throw new Exception("获取 ApplicationRootVisual 失败！");
                }
            }
        }
        public static void NavigationTo(string url)
        {
            if (frame == null)
                GetPhoneFrameRoot();
            if (frame != null)
            {
                var pageUri = new Uri(url, UriKind.Relative);
                frame.Navigate(pageUri);
            }
        }

        public static void NavigationTo(Uri pageUri)
        {
            if (frame == null)
                GetPhoneFrameRoot();
            if (frame != null)
            {
                frame.Navigate(pageUri);
            }
        }

    }

    public class HtmlHelp
    {
        /// <summary>
        /// 用正则表达式去掉Html中的script脚本和html标签
        /// </summary>
        /// <param name="Htmlstring"></param>
        /// <returns></returns>
        /// 
        /// NoHTMLContent正文用
        public static string NoHTMLContent(string Htmlstring)
        {
            //删除脚本   
            Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);

            Htmlstring = HttpUtility.HtmlDecode(Htmlstring).Replace("<br/>", "\r\n").Replace("<br>", "\r\n").Replace("</p>", "\r\n").Trim();

            //删除HTML   
            Htmlstring = Regex.Replace(Htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            //Htmlstring = Regex.Replace(Htmlstring, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"([\t])[\s]+", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"-->", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(nbsp|#160);", "   ", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);

            //Htmlstring = Htmlstring.Replace("<", "");
            //Htmlstring = Htmlstring.Replace(">", "");
            //Htmlstring = Htmlstring.Replace("\r\n", "");
            Htmlstring = Regex.Replace(Htmlstring, @"\r\r", Environment.NewLine, RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"\r\n\r\n\r\n", Environment.NewLine, RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"\r\n\r\n", Environment.NewLine, RegexOptions.IgnoreCase);

            return Htmlstring;
        }
        public static string NoHTML(string Htmlstring)
        {
            //删除脚本   
            Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            //删除HTML   
            Htmlstring = Regex.Replace(Htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            //Htmlstring = Regex.Replace(Htmlstring, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"([\t])[\s]+", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"-->", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(nbsp|#160);", "   ", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);

            //Htmlstring = Htmlstring.Replace("<", "");
            //Htmlstring = Htmlstring.Replace(">", "");
            //Htmlstring = Htmlstring.Replace("\r\n", "");
            Htmlstring = HttpUtility.HtmlDecode(Htmlstring).Replace("<br/>", "").Replace("<br>", "").Trim();
            Htmlstring = Regex.Replace(Htmlstring, @"\r\r", Environment.NewLine, RegexOptions.IgnoreCase);

            return Htmlstring;
        }

        public static string NoTag(string Htmlstring)
        {
            Htmlstring = Regex.Replace(Htmlstring, @"\[color=(.*?)\]", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"\[size=(.*?)\]", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"\[back=(.*?)\]", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"\[font=(.*?)\]", "", RegexOptions.IgnoreCase);

            Htmlstring = Htmlstring.Replace("[/u]", "");
            Htmlstring = Htmlstring.Replace("[u]", "");
            Htmlstring = Htmlstring.Replace("[/b]", "");
            Htmlstring = Htmlstring.Replace("[b]", "");
            Htmlstring = Htmlstring.Replace("[/i]", "");
            Htmlstring = Htmlstring.Replace("[i]", "");
            Htmlstring = Htmlstring.Replace("[/s]", "");
            Htmlstring = Htmlstring.Replace("[s]", "");
            Htmlstring = Htmlstring.Replace("[/color]", "");
            Htmlstring = Htmlstring.Replace("[/size]", "");
            Htmlstring = Htmlstring.Replace("[/back]", "");
            Htmlstring = Htmlstring.Replace("[/font]", "");

            return Htmlstring;
        }
    }

    public class HttpHelp
    {
        string boundary = "----------" + DateTime.Now.Ticks.ToString();
        public Dictionary<string, object> parameters = new Dictionary<string, object>();

        public async Task<Stream> Get(string url)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(new Uri(url));
                request.Method = "GET";
                if (StaticData.Cookie != null && StaticData.Cookie != "")
                {
#if DEBUG
                    Debug.WriteLine("GetCookie:" + StaticData.Cookie);
#endif
                    request.Headers["Cookie"] = StaticData.Cookie;
                }
                request.UserAgent = @"Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/36.0.1985.143 Safari/537.36";
                var response = await request.GetResponseAsync();
                var result = response.GetResponseStream();
                return CheckDebug(url, ref result);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<Stream> PostSetCookie(string url)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(new Uri(url));
                request.Method = "POST";
                if (StaticData.Cookie != null && StaticData.Cookie != "")
                {
#if DEBUG
                    Debug.WriteLine("GetCookie:" + StaticData.Cookie);
#endif
                    request.Headers["Cookie"] = StaticData.Cookie;
                }
                request.UserAgent = @"Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/36.0.1985.143 Safari/537.36";
                request.ContentType = string.Format("multipart/form-data; boundary={0}", boundary);
                var stream = await request.GetRequestStreamAsync();

                writeMultipartObject(stream, parameters);

                stream.Close();
                var response = await request.GetResponseAsync();
                foreach (var key in response.Headers.AllKeys)
                {
                    if (key == "Set-Cookie")
                    {
                        StaticData.Cookie = SetCookie(response.Headers[key]);
#if DEBUG
                        Debug.WriteLine("SetCookie:" + StaticData.Cookie);
#endif
                    }
                }
                var result = response.GetResponseStream();
                return CheckDebug(url, ref result);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<Stream> Post(string url)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(new Uri(url));
                request.Method = "POST";
                if (StaticData.Cookie != null && StaticData.Cookie != "")
                {
#if DEBUG
                    Debug.WriteLine("GetCookie:" + StaticData.Cookie);
#endif
                    request.Headers["Cookie"] = StaticData.Cookie;
                }
                request.UserAgent = @"Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/36.0.1985.143 Safari/537.36";
                request.ContentType = string.Format("multipart/form-data; boundary={0}", boundary);
                var stream = await request.GetRequestStreamAsync();

                writeMultipartObject(stream, parameters);

                stream.Close();
                var response = await request.GetResponseAsync();
                var result = response.GetResponseStream();
                return CheckDebug(url, ref result);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }
        public async Task<Stream> PostUrl(string url)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(new Uri(url));
                request.Method = "POST";
                if (StaticData.Cookie != null && StaticData.Cookie != "")
                {
#if DEBUG
                    Debug.WriteLine("GetCookie:" + StaticData.Cookie);
#endif
                    request.Headers["Cookie"] = StaticData.Cookie;
                }
                request.UserAgent = @"Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/36.0.1985.143 Safari/537.36";
                request.ContentType = string.Format("application/x-www-form-urlencoded; charset=UTF-8");
                var stream = await request.GetRequestStreamAsync();

                writeMultipartObject(stream, parameters);

                stream.Close();
                var response = await request.GetResponseAsync();
                var result = response.GetResponseStream();
                return CheckDebug(url, ref result);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        private string SetCookie(string old)
        {
            try
            {
                string newstr = "clientlanguage=zh_CN";
                var cookies = old.Split(',');
                foreach (var cookie in cookies)
                {
                    try
                    {
                        newstr += ";" + cookie.Split(';')[0];
                    }
                    catch (Exception)
                    {
                        newstr += ";" + cookie;
                    }
                }
                return newstr;
            }
            catch (Exception)
            {
                return "";
            }
        }

        private Stream CheckDebug(string url, ref Stream result)
        {
            try
            {
                if (result != null)
                {
#if DEBUG
                    StreamReader sr = new StreamReader(result);
                    Debug.WriteLine(url + "\r\n======================\r\n" + sr.ReadToEnd());
                    result.Position = 0;
#endif
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("CheckDebug:" + ex.Message);
                throw ex;
            }
            return result;
        }

        public void writeMultipartObject(Stream stream, object data)
        {
            try
            {
                var writer = new StreamWriter(stream);
                if (data != null)
                {
                    foreach (var entry in data as Dictionary<string, object>)
                    {
                        WriteEntry(writer, entry.Key, entry.Value);
                    }
                }
                writer.Write("--");
                writer.Write(boundary);
                writer.WriteLine("--");
                writer.Flush();
            }
            catch { }
        }

        private void WriteEntry(StreamWriter writer, string key, object value)
        {
            try
            {
                if (value != null)
                {
                    writer.Write("--");
                    writer.WriteLine(boundary);
                    if (value is byte[])
                    {
                        var ba = value as byte[];

                        writer.WriteLine(@"Content-Disposition: form-data; name=""{0}""; filename=""{1}""", key, Guid.NewGuid().ToString() + ".jpg");
                        writer.WriteLine(@"Content-Type: image/jpeg");
                        writer.WriteLine(@"Content-Length: " + ba.Length);
                        writer.WriteLine();
                        writer.Flush();
                        var output = writer.BaseStream;

                        output.Write(ba, 0, ba.Length);
                        output.Flush();
                        writer.WriteLine();
                    }
                    else
                    {
                        writer.WriteLine(@"Content-Disposition: form-data; name=""{0}""", key);
                        writer.WriteLine();
                        writer.WriteLine(value.ToString());
                    }
                }
            }
            catch { }
        }
    }

    public class LoadMoreHelper
    {
        public static readonly DependencyProperty LoadMoreCommandProperty;
        public static bool alreadyHookedScrollEvents = false;
        public static ListBox lbx;
        //private static double scrolledOffset;
        //private static double startY, endY; 
        static LoadMoreHelper()
        {
            LoadMoreCommandProperty = DependencyProperty.RegisterAttached("LoadMoreCommand", typeof(ICommand), typeof(ListBox), new PropertyMetadata(delegate(DependencyObject dp, DependencyPropertyChangedEventArgs ea)
            {
                lbx = dp as ListBox;
                if ((lbx != null) && (ea.NewValue != null))
                {
                    lbx.Unloaded -= new RoutedEventHandler(LoadMoreHelper.sv_Unloaded);
                    lbx.Loaded -= new RoutedEventHandler(LoadMoreHelper.sv_Loaded);

                    lbx.Loaded += new RoutedEventHandler(LoadMoreHelper.sv_Loaded);
                    lbx.LayoutUpdated += new EventHandler(lbx_LayoutUpdated);
                    lbx.Unloaded += new RoutedEventHandler(LoadMoreHelper.sv_Unloaded);

                }
            }));
        }



        public static void SetLoadMoreCommand(ListBox element, ICommand value)
        {
            element.SetValue(LoadMoreCommandProperty, value);
        }

        public static ICommand GetLoadMoreCommand(ListBox element)
        {
            return (ICommand)element.GetValue(LoadMoreCommandProperty);
        }

        public static UIElement FindElementRecursive(FrameworkElement parent, Type targetType)
        {
            var childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            UIElement element = null;
            if (childrenCount > 0)
            {
                for (var i = 0; i < childrenCount; i++)
                {
                    object child = VisualTreeHelper.GetChild(parent, i);
                    if (child.GetType() == targetType)
                    {
                        return (child as UIElement);
                    }
                    element = FindElementRecursive(VisualTreeHelper.GetChild(parent, i) as FrameworkElement, targetType);
                }
            }
            return element;
        }






        public static void sv_Loaded(object sender, RoutedEventArgs e)
        {
            lbx = sender as ListBox;

            var scrollBarList = GetVisualChildCollection<ScrollBar>(lbx);
            foreach (var scrollBar in scrollBarList)
            {
                if (scrollBar.Orientation == System.Windows.Controls.Orientation.Vertical)
                {
                    scrollBar.ValueChanged += new RoutedPropertyChangedEventHandler<double>(verticalScrollBar_ValueChanged);
                }
            }

        }

        static void lbx_LayoutUpdated(object sender, EventArgs e)
        {

        }

        public static void sv_Unloaded(object sender, RoutedEventArgs e)
        {
            lbx = sender as ListBox;
            lbx.Unloaded -= new RoutedEventHandler(LoadMoreHelper.sv_Unloaded);
            lbx.Loaded -= new RoutedEventHandler(LoadMoreHelper.sv_Loaded);
        }

        private static void verticalScrollBar_ValueChanged(object sender, RoutedEventArgs e)
        {
            var scrollBar = (ScrollBar)sender;
            var valueObj = scrollBar.GetValue(ScrollBar.ValueProperty);
            var maxObj = scrollBar.GetValue(ScrollBar.MaximumProperty);
            if (valueObj != null && maxObj != null)
            {
                var value = (double)valueObj;
                var max = (double)maxObj - 3;
                if (value >= max)
                {
                    var loadMoreCommand = GetLoadMoreCommand(lbx);
                    if (loadMoreCommand != null)
                    {
                        loadMoreCommand.Execute(null);
                    }
                }
            }
        }

        public static List<T> GetVisualChildCollection<T>(object parent) where T : UIElement
        {
            var visualCollection = new List<T>();
            GetVisualChildCollection(parent as DependencyObject, visualCollection);
            return visualCollection;
        }
        public static void GetVisualChildCollection<T>(DependencyObject parent, List<T> visualCollection) where T : UIElement
        {
            try
            {
                var count = VisualTreeHelper.GetChildrenCount(parent);
                for (var i = 0; i < count; i++)
                {
                    var child = VisualTreeHelper.GetChild(parent, i);
                    if (child is T)
                    {
                        visualCollection.Add(child as T);
                    }
                    else if (child != null)
                    {
                        GetVisualChildCollection(child, visualCollection);
                    }
                }
            }
            catch { }

        }
    }

    /// <summary>
    /// 独立存储缓存的图片源
    /// </summary>
    public sealed class StorageCachedImage : BitmapSource
    {
        private readonly Uri uriSource;
        private readonly string filePath;
        private const string CacheDirectory = "CacheHtmlFolder";

        static StorageCachedImage()
        {
            //创建缓存目录
            using (var isolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!isolatedStorageFile.DirectoryExists(CacheDirectory))
                {
                    isolatedStorageFile.CreateDirectory(CacheDirectory);
                }
            }
        }

        /// <summary>
        /// 创建一个独立存储缓存的图片源
        /// </summary>
        /// <param name="uriSource"></param>
        public StorageCachedImage(Uri uriSource)
        {
            this.uriSource = uriSource;

            //文件路径
            filePath = Path.Combine(CacheDirectory, uriSource.AbsolutePath.TrimStart('/').Replace('/', '_'));
            OpenCatchSource();
        }

        /// <summary>
        /// 打开缓存源
        /// </summary>
        private void OpenCatchSource()
        {
            bool exist;
            using (var isolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication())
            {
                exist = isolatedStorageFile.FileExists(filePath);
            }
            if (exist)
            {
                SetCacheStreamSource();
            }
            else
            {
                SetWebStreamSource();
            }
        }

        /// <summary>
        /// 设置缓存流到图片
        /// </summary>
        private void SetCacheStreamSource()
        {
            try
            {
                using (var isolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication())
                using (var stream = isolatedStorageFile.OpenFile(filePath, FileMode.Open, FileAccess.Read))
                {
                    SetSource(stream);
                }
            }
            catch { }
        }

        /// <summary>
        /// 下载Uri中的图片
        /// </summary>
        private void SetWebStreamSource()
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(uriSource);
            httpWebRequest.AllowReadStreamBuffering = true;
            httpWebRequest.BeginGetResponse(ResponseCallBack, httpWebRequest);
        }

        /// <summary>
        /// 下载回调
        /// </summary>
        /// <param name="asyncResult"></param>
        private void ResponseCallBack(IAsyncResult asyncResult)
        {
            var httpWebRequest = asyncResult.AsyncState as HttpWebRequest;
            if (httpWebRequest == null) return;
            try
            {
                var response = httpWebRequest.EndGetResponse(asyncResult);
                using (var stream = response.GetResponseStream())
                using (var isolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication())
                using (var fileStream = isolatedStorageFile.OpenFile
                    (filePath, FileMode.Create, FileAccess.Write))
                {
                    stream.CopyTo(fileStream);
                }
                Dispatcher.BeginInvoke(SetCacheStreamSource);
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
            }
        }
    }
    
    /// <summary>
    /// 保存数据到本地
    /// </summary>
    public class Settings
    {
        private static readonly IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
        /// <summary>
        /// 添加或更新数据
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">值</param>
        public static void AddOrUpdateValue(string key, object value)
        {
            var valueChanged = false;
            if (settings.Contains(key))
            {
                if (settings[key] != null)
                {
                    settings[key] = value;
                    valueChanged = true;
                }
            }
            else
            {
                settings.Add(key, value);
                valueChanged = true;
            }
            if (valueChanged)
            {
                Save();
            }
        }
        /// <summary>
        /// 获取指定的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">Key</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>返回指定类型的值</returns>
        public static T GetValueOrDefault<T>(string key, T defaultValue)
        {
            T value;
            try
            {
                if (settings.Contains(key))
                {
                    value = (T)settings[key];
                }
                else
                {
                    value = defaultValue;
                }

                return value;
            }
            catch
            {
                return defaultValue;
            }
        }
        private static void Save()
        {
            settings.Save();
        }

        public static void Clear()
        {
            settings.Clear();
        }
    }

    /// <summary>
    /// 缓存文件到本地
    /// </summary>
    public class CacheFile
    {
        public async static void ClearCache()
        {
            try
            {
                var local = Windows.Storage.ApplicationData.Current.LocalFolder;
                if (local != null)
                {
                    var dataFolder = await local.GetFolderAsync("CacheHtmlFolder");
                    {
                        if (dataFolder != null)
                        {
                            foreach (var file in await dataFolder.GetFilesAsync())
                            {
                                await file.DeleteAsync();
                            }
                        }
                    }
                }
            }
            catch { }
        }

        public async static Task<long> GetCacheSize()
        {
            var local = Windows.Storage.ApplicationData.Current.LocalFolder;
            long len = 0;
            if (local != null)
            {
                try
                {
                    var dataFolder = await local.GetFolderAsync("CacheHtmlFolder");
                    if (dataFolder != null)
                    {
                        foreach (var file in await dataFolder.GetFilesAsync())
                        {
                            len += (await file.OpenStreamForReadAsync()).Length;
                        }
                    }
                }
                catch { }
            }
            return len;
        }

        public async Task<Stream> GetCachedArticle(string acname)
        {
            Stream result;

            try
            {
                var acid = acname.Remove(0, 5);
                var local =
                    IsolatedStorageFile.GetUserStoreForApplication();

                if (local.FileExists("CacheHtmlFolder\\" + acid))
                {
                    result = await ReadFile(acid);
                }
                else
                {
                    var request = (HttpWebRequest)WebRequest.Create(new Uri("http://api.acfun.tv/videos/" + acid + "/Article"));
                    request.UserAgent = @"Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; Trident/6.0)";
                    var response = await request.GetResponseAsync();
                    var sr = new StreamReader(response.GetResponseStream());
                    await WriteToFile(sr.ReadToEnd(), acid);
                    result = await ReadFile(acid);
                }

                if (result == null)
                {
                    var request = (HttpWebRequest)WebRequest.Create(new Uri("http://api.acfun.tv/videos/" + acid + "/Article"));
                    request.UserAgent = @"Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; Trident/6.0)";
                    var response = await request.GetResponseAsync();
                    result = response.GetResponseStream();
                }

                return result;
            }
            catch { return null; }
        }

        public async Task<Stream> GetCachedHtml(string acname)
        {
            Stream result;

            try
            {
                var filename = acname.Remove(0, 3);
                var local =
                    IsolatedStorageFile.GetUserStoreForApplication();

                if (local.FileExists("CacheHtmlFolder\\" + filename))
                {
                    result = await ReadFile(filename);
                }
                else
                {
                    var request = (HttpWebRequest)WebRequest.Create(new Uri("http://api.acfun.tv/apiserver/content/info?contentId=" + StaticData.acitem.href.Remove(0, 5)));
                    request.UserAgent = @"Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; Trident/6.0)";
                    request.Headers["deviceType"] = "1";
                    var response = await request.GetResponseAsync();
                    var sr = new StreamReader(response.GetResponseStream());
                    var obj = JObject.Parse(sr.ReadToEnd());
                    await WriteToFile(obj["data"]["fullArticle"]["txt"].ToString(), filename);
                    result = await ReadFile(filename);
                }

                return result;
            }
            catch { return null; }
        }

        private async Task WriteToFile(string content, string filename)
        {
            // Get the text data from the textbox. 
            var fileBytes = System.Text.Encoding.UTF8.GetBytes(content);

            // Get the local folder.
            var local = Windows.Storage.ApplicationData.Current.LocalFolder;

            // Create a new folder name DataFolder.
            var dataFolder = await local.CreateFolderAsync("CacheHtmlFolder",
                CreationCollisionOption.OpenIfExists);

            // Create a new file named DataFile.txt.
            var file = await dataFolder.CreateFileAsync(filename,
            CreationCollisionOption.ReplaceExisting);

            // Write the data from the textbox.
            using (var s = await file.OpenStreamForWriteAsync())
            {
                s.Write(fileBytes, 0, fileBytes.Length);
            }
        }

        private async Task<Stream> ReadFile(string filename)
        {
            // Get the local folder.
            var local = Windows.Storage.ApplicationData.Current.LocalFolder;

            if (local != null)
            {
                // Get the DataFolder folder.
                var dataFolder = await local.GetFolderAsync("CacheHtmlFolder");

                // Get the file.
                var file = await dataFolder.OpenStreamForReadAsync(filename);

                // Read the data.
                return file;
            }
            else
            {
                return null;
            }
        }

        public async static Task DataWriter(string content, string filename)
        {
            // Get the text data from the textbox. 
            var fileBytes = System.Text.Encoding.UTF8.GetBytes(content);

            // Get the local folder.
            var local = Windows.Storage.ApplicationData.Current.LocalFolder;

            // Create a new folder name DataFolder.
            var dataFolder = await local.CreateFolderAsync("Data",
                CreationCollisionOption.OpenIfExists);

            // Create a new file named DataFile.txt.
            var file = await dataFolder.CreateFileAsync(filename,
            CreationCollisionOption.ReplaceExisting);

            // Write the data from the textbox.
            using (var s = await file.OpenStreamForWriteAsync())
            {
                s.Write(fileBytes, 0, fileBytes.Length);
            }
        }

        public async static Task<Stream> DataReader(string filename)
        {
            // Get the local folder.
            var local = Windows.Storage.ApplicationData.Current.LocalFolder;

            if (local != null)
            {
                // Get the DataFolder folder.
                var dataFolder = await local.GetFolderAsync("Data");

                // Get the file.
                var file = await dataFolder.OpenStreamForReadAsync(filename);

                // Read the data.
                return file;
            }
            else
            {
                return null;
            }
        }

        public static async Task<bool> IsAppUpdate()
        {
            try
            {
                // Get the local folder.
                var local = Windows.Storage.ApplicationData.Current.LocalFolder;

                if (local != null)
                {
                    // Get the DataFolder folder.
                    var dataFolder = await local.GetFolderAsync("Data");

                    if (dataFolder == null)
                        return true;

                    // Get the file.
                    var file = await dataFolder.GetFileAsync("bdapps");

                    if (file == null)
                        return true;

                    if (file.DateCreated.AddDays(1).CompareTo(DateTimeOffset.Now) > 0)
                        return true;

                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception)
            {
                return true;
            }
        }
    }

    public class TimeFuc
    {
        public static int getTimeSpan()
        {
            var startTime = new DateTime(1970, 1, 1);
            var time = (int)(DateTime.Now - startTime).TotalSeconds;
            return time;
        }

        public static int getTimeSpan(DateTime dateTime)
        {
            var startTime = new DateTime(1970, 1, 1);
            var time = (int)(dateTime - startTime).TotalSeconds;
            return time;
        }

        public static DateTime getTime(long timeStamp)
        {
            var dtStart = new DateTime(1970, 1, 1).ToLocalTime();
            var lTime = long.Parse(timeStamp.ToString() + "0000");
            var toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }
    }


    public static class PhoneNameResolver
    {
        public static CanonicalPhoneName Resolve(string manufacturer, string model)
        {
            var manufacturerNormalized = manufacturer.Trim().ToUpper();


            switch (manufacturerNormalized)
            {
                case "NOKIA":
                    return ResolveNokia(manufacturer, model);
                case "HTC":
                    return ResolveHtc(manufacturer, model);
                case "SAMSUNG":
                    return ResolveSamsung(manufacturer, model);
                case "LG":
                    return ResolveLg(manufacturer, model);
                case "HUAWEI":
                    return ResolveHuawei(manufacturer, model);
                default:
                    return new CanonicalPhoneName()
                    {
                        ReportedManufacturer = manufacturer,
                        ReportedModel = model,
                        CanonicalManufacturer = manufacturer,
                        CanonicalModel = model,
                        IsResolved = false
                    };


            }
        }





        private static CanonicalPhoneName ResolveHuawei(string manufacturer, string model)
        {
            var modelNormalized = model.Trim().ToUpper();


            var result = new CanonicalPhoneName()
            {
                ReportedManufacturer = manufacturer,
                ReportedModel = model,
                CanonicalManufacturer = "HUAWEI",
                CanonicalModel = model,
                IsResolved = false
            };



            var lookupValue = modelNormalized;


            if (lookupValue.StartsWith("HUAWEI H883G"))
            {
                lookupValue = "HUAWEI H883G";
            }


            if (lookupValue.StartsWith("HUAWEI W1"))
            {
                lookupValue = "HUAWEI W1";
            }


            if (modelNormalized.StartsWith("HUAWEI W2"))
            {
                lookupValue = "HUAWEI W2";
            }


            if (huaweiLookupTable.ContainsKey(lookupValue))
            {
                var modelMetadata = huaweiLookupTable[lookupValue];
                result.CanonicalModel = modelMetadata.CanonicalModel;
                result.Comments = modelMetadata.Comments;
                result.IsResolved = true;
            }


            return result;
        }






        private static CanonicalPhoneName ResolveLg(string manufacturer, string model)
        {
            var modelNormalized = model.Trim().ToUpper();


            var result = new CanonicalPhoneName()
            {
                ReportedManufacturer = manufacturer,
                ReportedModel = model,
                CanonicalManufacturer = "LG",
                CanonicalModel = model,
                IsResolved = false
            };




            var lookupValue = modelNormalized;


            if (lookupValue.StartsWith("LG-C900"))
            {
                lookupValue = "LG-C900";
            }


            if (lookupValue.StartsWith("LG-E900"))
            {
                lookupValue = "LG-E900";
            }


            if (lgLookupTable.ContainsKey(lookupValue))
            {
                var modelMetadata = lgLookupTable[lookupValue];
                result.CanonicalModel = modelMetadata.CanonicalModel;
                result.Comments = modelMetadata.Comments;
                result.IsResolved = true;
            }


            return result;
        }


        private static CanonicalPhoneName ResolveSamsung(string manufacturer, string model)
        {
            var modelNormalized = model.Trim().ToUpper();


            var result = new CanonicalPhoneName()
            {
                ReportedManufacturer = manufacturer,
                ReportedModel = model,
                CanonicalManufacturer = "SAMSUNG",
                CanonicalModel = model,
                IsResolved = false
            };




            var lookupValue = modelNormalized;


            if (lookupValue.StartsWith("GT-S7530"))
            {
                lookupValue = "GT-S7530";
            }


            if (lookupValue.StartsWith("SGH-I917"))
            {
                lookupValue = "SGH-I917";
            }


            if (samsungLookupTable.ContainsKey(lookupValue))
            {
                var modelMetadata = samsungLookupTable[lookupValue];
                result.CanonicalModel = modelMetadata.CanonicalModel;
                result.Comments = modelMetadata.Comments;
                result.IsResolved = true;
            }


            return result;
        }


        private static CanonicalPhoneName ResolveHtc(string manufacturer, string model)
        {
            var modelNormalized = model.Trim().ToUpper();


            var result = new CanonicalPhoneName()
            {
                ReportedManufacturer = manufacturer,
                ReportedModel = model,
                CanonicalManufacturer = "HTC",
                CanonicalModel = model,
                IsResolved = false
            };




            var lookupValue = modelNormalized;


            if (lookupValue.StartsWith("A620"))
            {
                lookupValue = "A620";
            }


            if (lookupValue.StartsWith("C625"))
            {
                lookupValue = "C625";
            }


            if (lookupValue.StartsWith("C620"))
            {
                lookupValue = "C625";
            }


            if (htcLookupTable.ContainsKey(lookupValue))
            {
                var modelMetadata = htcLookupTable[lookupValue];
                result.CanonicalModel = modelMetadata.CanonicalModel;
                result.Comments = modelMetadata.Comments;
                result.IsResolved = true;
            }


            return result;
        }


        private static CanonicalPhoneName ResolveNokia(string manufacturer, string model)
        {
            var modelNormalized = model.Trim().ToUpper();


            var result = new CanonicalPhoneName()
            {
                ReportedManufacturer = manufacturer,
                ReportedModel = model,
                CanonicalManufacturer = "NOKIA",
                CanonicalModel = model,
                IsResolved = false
            };


            var lookupValue = modelNormalized;
            if (modelNormalized.StartsWith("RM-"))
            {
                lookupValue = modelNormalized.Substring(0, 6);
            }


            if (nokiaLookupTable.ContainsKey(lookupValue))
            {
                var modelMetadata = nokiaLookupTable[lookupValue];
                result.CanonicalModel = modelMetadata.CanonicalModel;
                result.Comments = modelMetadata.Comments;
                result.IsResolved = true;
            }


            return result;
        }




        private static Dictionary<string, CanonicalPhoneName> huaweiLookupTable = new Dictionary<string, CanonicalPhoneName>()
        {
            // Huawei W1
            { "HUAWEI H883G", new CanonicalPhoneName() { CanonicalModel = "Ascend W1" } },
            { "HUAWEI W1", new CanonicalPhoneName() { CanonicalModel = "Ascend W1" } },
            
            // Huawei Ascend W2
            { "HUAWEI W2", new CanonicalPhoneName() { CanonicalModel = "Ascend W2" } },
        };




        private static Dictionary<string, CanonicalPhoneName> lgLookupTable = new Dictionary<string, CanonicalPhoneName>()
        {
            // Optimus 7Q/Quantum
            { "LG-C900", new CanonicalPhoneName() { CanonicalModel = "Optimus 7Q/Quantum" } },


            // Optimus 7
            { "LG-E900", new CanonicalPhoneName() { CanonicalModel = "Optimus 7" } },


            // Jil Sander
            { "LG-E906", new CanonicalPhoneName() { CanonicalModel = "Jil Sander" } },
        };


        private static Dictionary<string, CanonicalPhoneName> samsungLookupTable = new Dictionary<string, CanonicalPhoneName>()
        {
            // OMNIA W
            { "GT-I8350", new CanonicalPhoneName() { CanonicalModel = "Omnia W" } },
            { "GT-I8350T", new CanonicalPhoneName() { CanonicalModel = "Omnia W" } },
            { "OMNIA W", new CanonicalPhoneName() { CanonicalModel = "Omnia W" } },


            // OMNIA 7
            { "GT-I8700", new CanonicalPhoneName() { CanonicalModel = "Omnia 7" } },
            { "OMNIA7", new CanonicalPhoneName() { CanonicalModel = "Omnia 7" } },


            // OMNIA M
            { "GT-S7530", new CanonicalPhoneName() { CanonicalModel = "Omnia 7" } },


            // Focus
            { "I917", new CanonicalPhoneName() { CanonicalModel = "Focus" } },
            { "SGH-I917", new CanonicalPhoneName() { CanonicalModel = "Focus" } },


            // Focus 2
            { "SGH-I667", new CanonicalPhoneName() { CanonicalModel = "Focus 2" } },


            // Focus Flash
            { "SGH-I677", new CanonicalPhoneName() { CanonicalModel = "Focus Flash" } },


            // Focus S
            { "HADEN", new CanonicalPhoneName() { CanonicalModel = "Focus S" } },
            { "SGH-I937", new CanonicalPhoneName() { CanonicalModel = "Focus S" } },


            // ATIV S
            { "GT-I8750", new CanonicalPhoneName() { CanonicalModel = "ATIV S" } },
            { "SGH-T899M", new CanonicalPhoneName() { CanonicalModel = "ATIV S" } },


            // ATIV Odyssey
            { "SCH-I930", new CanonicalPhoneName() { CanonicalModel = "ATIV Odyssey" } },
            { "SCH-R860U", new CanonicalPhoneName() { CanonicalModel = "ATIV Odyssey", Comments="US Cellular" } },


            // ATIV S Neo
            { "SPH-I800", new CanonicalPhoneName() { CanonicalModel = "ATIV S Neo", Comments="Sprint" } },
            { "SGH-I187", new CanonicalPhoneName() { CanonicalModel = "ATIV S Neo", Comments="AT&T" } },
        };


        private static Dictionary<string, CanonicalPhoneName> htcLookupTable = new Dictionary<string, CanonicalPhoneName>()
        {
            // Surround
            { "7 MONDRIAN T8788", new CanonicalPhoneName() { CanonicalModel = "Surround" } },
            { "T8788", new CanonicalPhoneName() { CanonicalModel = "Surround" } },
            { "SURROUND", new CanonicalPhoneName() { CanonicalModel = "Surround" } },
            { "SURROUND T8788", new CanonicalPhoneName() { CanonicalModel = "Surround" } },


            // Mozart
            { "7 MOZART", new CanonicalPhoneName() { CanonicalModel = "Mozart" } },
            { "7 MOZART T8698", new CanonicalPhoneName() { CanonicalModel = "Mozart" } },
            { "HTC MOZART", new CanonicalPhoneName() { CanonicalModel = "Mozart" } },
            { "MERSAD 7 MOZART T8698", new CanonicalPhoneName() { CanonicalModel = "Mozart" } },
            { "MOZART", new CanonicalPhoneName() { CanonicalModel = "Mozart" } },
            { "MOZART T8698", new CanonicalPhoneName() { CanonicalModel = "Mozart" } },
            { "PD67100", new CanonicalPhoneName() { CanonicalModel = "Mozart" } },
            { "T8697", new CanonicalPhoneName() { CanonicalModel = "Mozart" } },


            // Pro
            { "7 PRO T7576", new CanonicalPhoneName() { CanonicalModel = "7 Pro" } },
            { "MWP6885", new CanonicalPhoneName() { CanonicalModel = "7 Pro" } },
            { "USCCHTC-PC93100", new CanonicalPhoneName() { CanonicalModel = "7 Pro" } },


            // Arrive
            { "PC93100", new CanonicalPhoneName() { CanonicalModel = "Arrive", Comments = "Sprint" } },
            { "T7575", new CanonicalPhoneName() { CanonicalModel = "Arrive", Comments = "Sprint" } },


            // HD2
            { "HD2", new CanonicalPhoneName() { CanonicalModel = "HD2" } },
            { "HD2 LEO", new CanonicalPhoneName() { CanonicalModel = "HD2" } },
            { "LEO", new CanonicalPhoneName() { CanonicalModel = "HD2" } },


            // HD7
            { "7 SCHUBERT T9292", new CanonicalPhoneName() { CanonicalModel = "HD7" } },
            { "GOLD", new CanonicalPhoneName() { CanonicalModel = "HD7" } },
            { "HD7", new CanonicalPhoneName() { CanonicalModel = "HD7" } },
            { "HD7 T9292", new CanonicalPhoneName() { CanonicalModel = "HD7" } },
            { "MONDRIAN", new CanonicalPhoneName() { CanonicalModel = "HD7" } },
            { "SCHUBERT", new CanonicalPhoneName() { CanonicalModel = "HD7" } },
            { "Schubert T9292", new CanonicalPhoneName() { CanonicalModel = "HD7" } },
            { "T9296", new CanonicalPhoneName() { CanonicalModel = "HD7", Comments = "Telstra, AU" } },
            { "TOUCH-IT HD7", new CanonicalPhoneName() { CanonicalModel = "HD7" } },


            // HD7S
            { "T9295", new CanonicalPhoneName() { CanonicalModel = "HD7S" } },


            // Trophy
            { "7 TROPHY", new CanonicalPhoneName() { CanonicalModel = "Trophy" } },
            { "7 TROPHY T8686", new CanonicalPhoneName() { CanonicalModel = "Trophy" } },
            { "PC40100", new CanonicalPhoneName() { CanonicalModel = "Trophy", Comments = "Verizon" } },
            { "SPARK", new CanonicalPhoneName() { CanonicalModel = "Trophy" } },
            { "TOUCH-IT TROPHY", new CanonicalPhoneName() { CanonicalModel = "Trophy" } },
            { "MWP6985", new CanonicalPhoneName() { CanonicalModel = "Trophy" } },


            // 8S
            { "A620", new CanonicalPhoneName() { CanonicalModel = "8S" } },
            { "WINDOWS PHONE 8S BY HTC", new CanonicalPhoneName() { CanonicalModel = "8S" } },


            // 8X
            { "C620", new CanonicalPhoneName() { CanonicalModel = "8X" } },
            { "C625", new CanonicalPhoneName() { CanonicalModel = "8X" } },
            { "HTC6990LVW", new CanonicalPhoneName() { CanonicalModel = "8X", Comments="Verizon" } },
            { "PM23300", new CanonicalPhoneName() { CanonicalModel = "8X", Comments="AT&T" } },
            { "WINDOWS PHONE 8X BY HTC", new CanonicalPhoneName() { CanonicalModel = "8X" } },


            // 8XT
            { "HTCPO881 SPRINT", new CanonicalPhoneName() { CanonicalModel = "8XT", Comments="Sprint" } },


            // Titan
            { "ETERNITY", new CanonicalPhoneName() { CanonicalModel = "Titan", Comments = "China" } },
            { "PI39100", new CanonicalPhoneName() { CanonicalModel = "Titan", Comments = "AT&T" } },
            { "TITAN X310E", new CanonicalPhoneName() { CanonicalModel = "Titan" } },
            { "ULTIMATE", new CanonicalPhoneName() { CanonicalModel = "Titan" } },
            { "X310E", new CanonicalPhoneName() { CanonicalModel = "Titan" } },
            { "X310E TITAN", new CanonicalPhoneName() { CanonicalModel = "Titan" } },
            
            // Titan II
            { "PI86100", new CanonicalPhoneName() { CanonicalModel = "Titan II", Comments = "AT&T" } },
            { "RADIANT", new CanonicalPhoneName() { CanonicalModel = "Titan II" } },


            // Radar
            { "RADAR", new CanonicalPhoneName() { CanonicalModel = "Radar" } },
            { "RADAR 4G", new CanonicalPhoneName() { CanonicalModel = "Radar", Comments = "T-Mobile USA" } },
            { "RADAR C110E", new CanonicalPhoneName() { CanonicalModel = "Radar" } },
            
        };


        private static Dictionary<string, CanonicalPhoneName> nokiaLookupTable = new Dictionary<string, CanonicalPhoneName>()
        {
            // Lumia 505
            { "LUMIA 505", new CanonicalPhoneName() { CanonicalModel = "Lumia 505" } },
            // Lumia 510
            { "LUMIA 510", new CanonicalPhoneName() { CanonicalModel = "Lumia 510" } },
            { "NOKIA 510", new CanonicalPhoneName() { CanonicalModel = "Lumia 510" } },
            // Lumia 610
            { "LUMIA 610", new CanonicalPhoneName() { CanonicalModel = "Lumia 610" } },
            { "LUMIA 610 NFC", new CanonicalPhoneName() { CanonicalModel = "Lumia 610", Comments = "NFC" } },
            { "NOKIA 610", new CanonicalPhoneName() { CanonicalModel = "Lumia 610" } },
            { "NOKIA 610C", new CanonicalPhoneName() { CanonicalModel = "Lumia 610" } },
            // Lumia 620
            { "LUMIA 620", new CanonicalPhoneName() { CanonicalModel = "Lumia 620" } },
            { "RM-846", new CanonicalPhoneName() { CanonicalModel = "Lumia 620" } },
            // Lumia 710
            { "LUMIA 710", new CanonicalPhoneName() { CanonicalModel = "Lumia 710" } },
            { "NOKIA 710", new CanonicalPhoneName() { CanonicalModel = "Lumia 710" } },
            // Lumia 800
            { "LUMIA 800", new CanonicalPhoneName() { CanonicalModel = "Lumia 800" } },
            { "LUMIA 800C", new CanonicalPhoneName() { CanonicalModel = "Lumia 800" } },
            { "NOKIA 800", new CanonicalPhoneName() { CanonicalModel = "Lumia 800" } },
            { "NOKIA 800C", new CanonicalPhoneName() { CanonicalModel = "Lumia 800", Comments = "China" } },
            // Lumia 810
            { "RM-878", new CanonicalPhoneName() { CanonicalModel = "Lumia 810" } },
            // Lumia 820
            { "RM-824", new CanonicalPhoneName() { CanonicalModel = "Lumia 820" } },
            { "RM-825", new CanonicalPhoneName() { CanonicalModel = "Lumia 820" } },
            { "RM-826", new CanonicalPhoneName() { CanonicalModel = "Lumia 820" } },
            // Lumia 822
            { "RM-845", new CanonicalPhoneName() { CanonicalModel = "Lumia 822", Comments = "Verizon" } },
            // Lumia 900
            { "LUMIA 900", new CanonicalPhoneName() { CanonicalModel = "Lumia 900" } },
            { "NOKIA 900", new CanonicalPhoneName() { CanonicalModel = "Lumia 900" } },
            // Lumia 920
            { "RM-820", new CanonicalPhoneName() { CanonicalModel = "Lumia 920" } },
            { "RM-821", new CanonicalPhoneName() { CanonicalModel = "Lumia 920" } },
            { "RM-822", new CanonicalPhoneName() { CanonicalModel = "Lumia 920" } },
            { "RM-867", new CanonicalPhoneName() { CanonicalModel = "Lumia 920", Comments = "920T" } },
            { "NOKIA 920", new CanonicalPhoneName() { CanonicalModel = "Lumia 920" } },
            { "LUMIA 920", new CanonicalPhoneName() { CanonicalModel = "Lumia 920" } },
            // Lumia 520
            { "RM-914", new CanonicalPhoneName() { CanonicalModel = "Lumia 520" } },
            { "RM-915", new CanonicalPhoneName() { CanonicalModel = "Lumia 520" } },
            { "RM-913", new CanonicalPhoneName() { CanonicalModel = "Lumia 520", Comments="520T" } },
            // Lumia 521?
            { "RM-917", new CanonicalPhoneName() { CanonicalModel = "Lumia 521", Comments="T-Mobile 520" } },
            // Lumia 720
            { "RM-885", new CanonicalPhoneName() { CanonicalModel = "Lumia 720" } },
            { "RM-887", new CanonicalPhoneName() { CanonicalModel = "Lumia 720", Comments="China 720T" } },
            // Lumia 928
            { "RM-860", new CanonicalPhoneName() { CanonicalModel = "Lumia 928" } },
            // Lumia 925
            { "RM-892", new CanonicalPhoneName() { CanonicalModel = "Lumia 925" } },
            { "RM-893", new CanonicalPhoneName() { CanonicalModel = "Lumia 925" } },
            { "RM-910", new CanonicalPhoneName() { CanonicalModel = "Lumia 925" } },
            { "RM-955", new CanonicalPhoneName() { CanonicalModel = "Lumia 925", Comments="China 925T" } },
            // Lumia 1020
            { "RM-875", new CanonicalPhoneName() { CanonicalModel = "Lumia 1020" } },
            { "RM-876", new CanonicalPhoneName() { CanonicalModel = "Lumia 1020" } },
            { "RM-877", new CanonicalPhoneName() { CanonicalModel = "Lumia 1020" } },
            // Lumia 625
            { "RM-941", new CanonicalPhoneName() { CanonicalModel = "Lumia 625" } },
            { "RM-942", new CanonicalPhoneName() { CanonicalModel = "Lumia 625" } },
            { "RM-943", new CanonicalPhoneName() { CanonicalModel = "Lumia 625" } },
            // Lumia 1520
            { "RM-937", new CanonicalPhoneName() { CanonicalModel = "Lumia 1520" } },
            { "RM-938", new CanonicalPhoneName() { CanonicalModel = "Lumia 1520", Comments="AT&T" } },
            { "RM-939", new CanonicalPhoneName() { CanonicalModel = "Lumia 1520" } },
            { "RM-940", new CanonicalPhoneName() { CanonicalModel = "Lumia 1520", Comments="AT&T" } },


            // Lumia 525
            { "RM-998", new CanonicalPhoneName() { CanonicalModel = "Lumia 525" } },
        };
    }
    public class CanonicalPhoneName
    {
        public string ReportedManufacturer { get; set; }
        public string ReportedModel { get; set; }
        public string CanonicalManufacturer { get; set; }
        public string CanonicalModel { get; set; }
        public string Comments { get; set; }
        public bool IsResolved { get; set; }


        public string FullCanonicalName
        {
            get { return CanonicalManufacturer + " " + CanonicalModel; }
        }
    }
}
