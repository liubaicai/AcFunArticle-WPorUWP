using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace ACFUN
{
    public class Json
    {
        /// <summary>
        /// 对象序列化成 Json String
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string Serializer<T>(T t)
        {
            var ser = new DataContractJsonSerializer(typeof(T));
            var ms = new MemoryStream();
            ser.WriteObject(ms, t);
            var array = ms.ToArray();
            var jsonString = Encoding.UTF8.GetString(array, 0, array.Length);
            ms.Close();
            return jsonString;
        }

        /// <summary>
        /// Json String 反序列化成对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string jsonString)
        {
            var ser = new DataContractJsonSerializer(typeof(T));
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            var obj = (T)ser.ReadObject(ms);
            return obj;
        }
    }
}
