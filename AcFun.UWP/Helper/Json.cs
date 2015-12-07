using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using Newtonsoft.Json;

namespace BaicaiMobileService.Helper
{
    public static class Json
    {
        /// <summary>
        /// 对象序列化成 Json String
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string ToJsonString<T>(this T t)
        {
            return JsonConvert.SerializeObject(t);
        }

        /// <summary>
        /// Json String 反序列化成对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static T ToJsonObject<T>(this string jsonString)
        {
            Debug.WriteLine("============ToJsonObject");
            Debug.WriteLine(jsonString);
            return JsonConvert.DeserializeObject<T>(jsonString);
        }

        /// <summary>
        /// Json Stream 反序列化成对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonStream"></param>
        /// <returns></returns>
        public static T ToJsonObject<T>(this Stream jsonStream)
        {
            var jsonString = new StreamReader(jsonStream).ReadToEnd();
            Debug.WriteLine("============ToJsonObject");
            Debug.WriteLine(jsonString);
            return JsonConvert.DeserializeObject<T>(jsonString);
        }
    }
}