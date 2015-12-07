using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AcFun.UWP.Helper
{
    public class Http
    {
        public static HttpClient Instance
        {
            get
            {
                var http = new HttpClient();
                http.DefaultRequestHeaders.SetCookie();
                http.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/45.0.2454.101 Safari/537.36");
                http.DefaultRequestHeaders.Add("Accept-Encoding", "deflate, sdch");
                http.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.8");
                return http;
            }
        }

        public static HttpClient GzipInstance
        {
            get
            {
                var http = new HttpClient();
                http.DefaultRequestHeaders.SetCookie();
                http.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/45.0.2454.101 Safari/537.36");
                http.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, sdch");
                http.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.8");
                return http;
            }
        }
    }
}
