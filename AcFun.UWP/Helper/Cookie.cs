using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AcFun.UWP.Helper
{
    public static class Cookie
    {
        public static Dictionary<string, string> Cookies { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// auth_key 用户id
        /// </summary>
        public static string Uid { get; set; }

        public static void SetCookie(this HttpRequestHeaders headers)
        {
            if (Cookies != null && Cookies.Count > 0)
            {
                foreach (var cookie in Cookies)
                {
                    headers.Add("Cookie", $"{cookie.Key}={cookie.Value};");
                }
            }
        }

        public static void GetCookie(this HttpResponseHeaders headers)
        {
            foreach (var cookie in headers.ToImmutableDictionary()["Set-Cookie"])
            {
                var pair = cookie.Split(';')[0].Split('=');
                var key = pair[0];
                var value = pair[1].Replace("\"","");
                if (!Cookies.ContainsKey(key))
                {
                    Cookies.Add(key, value);
                }
            }
            if (Cookies.ContainsKey("auth_key"))
            {
                Uid = Cookies["auth_key"];
            }
        }
    }
}
