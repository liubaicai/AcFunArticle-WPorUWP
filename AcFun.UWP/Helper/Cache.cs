using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Newtonsoft.Json.Linq;

namespace AcFun.UWP.Helper
{
    public class Cache
    {
        public static async Task<string> GetCachedHtml(int contentId)

        {
            try
            {
                var http = Http.Instance;
                var result = await http.GetStringAsync(string.Format(AppData.ContentInfoUrl, contentId));
                await WriteToFile("cached", contentId.ToString());
                return result;
            }
            catch { return string.Empty; }
        }

        private static async Task WriteToFile(string content, string filename)
        {
            // Get the text data from the textbox. 
            var fileBytes = Encoding.UTF8.GetBytes(content);

            // Get the local folder.
            var local = ApplicationData.Current.LocalFolder;

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

        private static async Task<string> ReadFile(string filename)
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
                var str = await new StreamReader(file).ReadToEndAsync();

                return str;
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
