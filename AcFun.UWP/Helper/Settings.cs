using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcFun.UWP.Helper
{
    public class Settings
    {
        private static readonly Windows.Storage.ApplicationDataContainer LocalSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        public static void Set(string key, object value)
        {
            LocalSettings.Values[key] = value;
        }

        public static T Get<T>(string key, T defaultValue)
        {
            if (LocalSettings.Values[key] == null)
            {
                return defaultValue;
            }
            else
            {
                return (T)LocalSettings.Values[key];
            }
        }
    }
}
