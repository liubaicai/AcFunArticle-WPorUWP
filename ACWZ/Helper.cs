using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ACWZ
{
    public class HtmlHelp
    {
        /// <summary>
        /// 用正则表达式去掉Html中的script脚本和html标签
        /// </summary>
        /// <param name="Htmlstring"></param>
        /// <returns></returns>
        public static string NoHTML(string Htmlstring)
        {
            //删除脚本   
            Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            //删除HTML   
            Htmlstring = Regex.Replace(Htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
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
                    request.Headers["Cookie"] = StaticData.Cookie;
                }
                request.UserAgent = @"Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; Trident/6.0)";
                var response = await request.GetResponseAsync();
                return response.GetResponseStream();
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
                    request.Headers["Cookie"] = StaticData.Cookie;
                }
                request.UserAgent = @"Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; Trident/6.0)";
                request.ContentType = string.Format("multipart/form-data; boundary={0}", boundary);
                var stream = await request.GetRequestStreamAsync();

                writeMultipartObject(stream, parameters);

                stream.Close();
                var response = await request.GetResponseAsync();
                foreach (var key in response.Headers.AllKeys)
                {
                    if (key == "Set-Cookie")
                    {
                        StaticData.Cookie = response.Headers[key];
                    }
                }
                return response.GetResponseStream();
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
                    request.Headers["Cookie"] = StaticData.Cookie;
                }
                request.UserAgent = @"Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; Trident/6.0)";
                request.ContentType = string.Format("multipart/form-data; boundary={0}", boundary);
                var stream = await request.GetRequestStreamAsync();

                writeMultipartObject(stream, parameters);

                stream.Close();
                var response = await request.GetResponseAsync();
                return response.GetResponseStream();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        public void writeMultipartObject(Stream stream, object data)
        {
            try
            {
                StreamWriter writer = new StreamWriter(stream);
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
                        byte[] ba = value as byte[];

                        writer.WriteLine(@"Content-Disposition: form-data; name=""{0}""; filename=""{1}""", key, Guid.NewGuid().ToString() + ".jpg");
                        writer.WriteLine(@"Content-Type: image/jpeg");
                        writer.WriteLine(@"Content-Length: " + ba.Length);
                        writer.WriteLine();
                        writer.Flush();
                        Stream output = writer.BaseStream;

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
            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            UIElement element = null;
            if (childrenCount > 0)
            {
                for (int i = 0; i < childrenCount; i++)
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

            List<ScrollBar> scrollBarList = GetVisualChildCollection<ScrollBar>(lbx);
            foreach (ScrollBar scrollBar in scrollBarList)
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
            ScrollBar scrollBar = (ScrollBar)sender;
            object valueObj = scrollBar.GetValue(ScrollBar.ValueProperty);
            object maxObj = scrollBar.GetValue(ScrollBar.MaximumProperty);
            if (valueObj != null && maxObj != null)
            {
                double value = (double)valueObj;
                double max = (double)maxObj - 3;
                if (value >= max)
                {
                    ICommand loadMoreCommand = GetLoadMoreCommand(lbx);
                    if (loadMoreCommand != null)
                    {
                        loadMoreCommand.Execute(null);
                    }
                }
            }
        }

        public static List<T> GetVisualChildCollection<T>(object parent) where T : UIElement
        {
            List<T> visualCollection = new List<T>();
            GetVisualChildCollection(parent as DependencyObject, visualCollection);
            return visualCollection;
        }
        public static void GetVisualChildCollection<T>(DependencyObject parent, List<T> visualCollection) where T : UIElement
        {
            try
            {
                int count = VisualTreeHelper.GetChildrenCount(parent);
                for (int i = 0; i < count; i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(parent, i);
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
        private const string CacheDirectory = "CacheFolder";

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
            bool valueChanged = false;
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
#if WP8
        public async static void ClearCache()
        {
            try
            {
                StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
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
            StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
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

        public async Task<Stream> GetCachedHtml(string acname)
        {
            Stream result;

            try
            {
                string filename = acname.Remove(0, 3);
                IsolatedStorageFile local =
                    IsolatedStorageFile.GetUserStoreForApplication();

                if (local.FileExists("CacheHtmlFolder\\" + filename))
                {
                    result = await ReadFile(filename);
                }
                else
                {
                    var request = (HttpWebRequest)WebRequest.Create(new Uri("http://www.acfun.com" + StaticData.acitem.href));
                    request.UserAgent = @"Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; Trident/6.0)";
                    var response = await request.GetResponseAsync();
                    StreamReader sr = new StreamReader(response.GetResponseStream());
                    await WriteToFile(sr.ReadToEnd(), filename);
                    result = await ReadFile(filename);
                }

                if (result == null)
                {
                    var request = (HttpWebRequest)WebRequest.Create(new Uri("http://www.acfun.com" + StaticData.acitem.href));
                    request.UserAgent = @"Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; Trident/6.0)";
                    var response = await request.GetResponseAsync();
                    result = response.GetResponseStream();
                }

                return result;
            }
            catch { return null; }
        }

        private async Task WriteToFile(string content, string filename)
        {
            // Get the text data from the textbox. 
            byte[] fileBytes = System.Text.Encoding.UTF8.GetBytes(content);

            // Get the local folder.
            StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;

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
            StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;

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
#endif
    }

    public class TimeFuc
    {
        public static int getTimeSpan()
        {
            DateTime startTime = new DateTime(1970, 1, 1);
            int time = (int)(DateTime.Now - startTime).TotalSeconds;
            return time;
        }

        public static int getTimeSpan(DateTime dateTime)
        {
            DateTime startTime = new DateTime(1970, 1, 1);
            int time = (int)(dateTime - startTime).TotalSeconds;
            return time;
        }

        public static DateTime getTime(long timeStamp)
        {
            DateTime dtStart = new DateTime(1970, 1, 1).ToLocalTime();
            long lTime = long.Parse(timeStamp.ToString() + "0000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }
    }
}
