using AcFunBlue.DataModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AcFunBlue.Common
{
    public class HttpHelper
    {
        public async Task<Stream> Get(string url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(url));
                request.Method = "GET";
                request.Headers["User-Agent"] = @"Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; Trident/6.0)";
                if (StaticData.Cookie != null && StaticData.Cookie != "")
                {
                    request.Headers["Cookie"] = StaticData.Cookie;
                }
                var response = await request.GetResponseAsync();
                return response.GetResponseStream();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Stream> Post(string url, string post)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(url));
                request.Method = "POST";
                request.Headers["User-Agent"] = @"Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; Trident/6.0)";
                if (StaticData.Cookie != null && StaticData.Cookie != "")
                {
                    request.Headers["Cookie"] = StaticData.Cookie;
                }
                request.ContentType = "application/json; charset=utf-8";
                using (var stream = await request.GetRequestStreamAsync())
                {
                    stream.Position = 0;
                    byte[] buffer = Encoding.UTF8.GetBytes(post);
                    stream.Write(buffer, 0, buffer.Length);
                    await stream.FlushAsync();
                }
                var response = await request.GetResponseAsync();
                return response.GetResponseStream();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        string boundary = "----------" + DateTime.Now.Ticks.ToString();
        public Dictionary<string, object> parameters = new Dictionary<string, object>();

        public async Task<Stream> PostSetCookie(string url)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(new Uri(url));
                request.Method = "POST";
                request.Headers["User-Agent"] = @"Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; Trident/6.0)";
                if (StaticData.Cookie != null && StaticData.Cookie != "")
                {
                    request.Headers["Cookie"] = StaticData.Cookie;
                }
                request.ContentType = string.Format("multipart/form-data; boundary={0}", boundary);
                using (var stream = await request.GetRequestStreamAsync())
                {
                    writeMultipartObject(stream, parameters);
                }
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
                request.Headers["User-Agent"] = @"Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; Trident/6.0)";
                if (StaticData.Cookie != null && StaticData.Cookie != "")
                {
                    request.Headers["Cookie"] = StaticData.Cookie;
                }
                request.ContentType = string.Format("multipart/form-data; boundary={0}", boundary);
                using (var stream = await request.GetRequestStreamAsync())
                {
                    writeMultipartObject(stream, parameters);
                }
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
    public class Result
    {
        public int code { set; get; }
        public string msg { set; get; }
    }
}
