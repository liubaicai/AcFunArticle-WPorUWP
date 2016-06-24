using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AcFun.UWP.Helper
{
    public class Http
    {
        public static HttpClientEx Instance
        {
            get
            {
                var http = new HttpClientEx();
                http.DefaultRequestHeaders.SetCookie();
                http.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/45.0.2454.101 Safari/537.36");
                http.DefaultRequestHeaders.Add("Accept-Encoding", "deflate, sdch");
                http.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.8");
                http.DefaultRequestHeaders.Add("deviceType", "1");
                return http;
            }
        }

        public static HttpClientEx GzipInstance
        {
            get
            {
                var http = new HttpClientEx();
                http.DefaultRequestHeaders.SetCookie();
                http.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/45.0.2454.101 Safari/537.36");
                http.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, sdch");
                http.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.8");
                http.DefaultRequestHeaders.Add("deviceType", "1");
                return http;
            }
        }
    }

    public class HttpClientEx : HttpClient
    {
        public new Task<string> GetStringAsync(string requestUri)
        {
            Debug.WriteLine(requestUri);
            return base.GetStringAsync(requestUri);
        }
    }
}
