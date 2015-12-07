using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.Storage;

namespace AcFunBlue.Common
{
    public class Settings
    {
        private static readonly IPropertySet settings = ApplicationData.Current.LocalSettings.Values;
        /// <summary>
        /// 添加或更新数据
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">值</param>
        public static void Set(string key, object value)
        {
            try
            {
                settings[key] = value;
            }
            catch { }
        }
        /// <summary>
        /// 获取指定的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">Key</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>返回指定类型的值</returns>
        public static T Get<T>(string key, T defaultValue)
        {
            T value;
            try
            {
                if (settings.ContainsKey(key))
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
        private static void Remove(string key)
        {
            try
            {
                if (settings.ContainsKey(key))
                {
                    settings.Remove(key);
                }
            }
            catch { }
        }

        public static void Clear()
        {
            settings.Clear();
        }
    }
}
