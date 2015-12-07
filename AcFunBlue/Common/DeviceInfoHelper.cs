using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace AcFunBlue.Common
{
    public class DeviceInfoHelper
    {
        public static string GetDeviceId()
        {
            return GetMD5(GetUniqueId());
        }

        /// <summary>
        /// 获取设备id
        /// </summary>
        /// <returns></returns>
        private static string GetUniqueId()
        {
            var token = Windows.System.Profile.HardwareIdentification.GetPackageSpecificToken(null);
            IBuffer buffer = token.Id;
            using (var dataReader = DataReader.FromBuffer(buffer))
            {
                var bytes = new byte[buffer.Length];
                dataReader.ReadBytes(bytes);
                return BitConverter.ToString(bytes);
            }
        }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetMD5(string str)
        {
            IBuffer mybf = CryptographicBuffer.ConvertStringToBinary(str, BinaryStringEncoding.Utf8);
            string strAgName = HashAlgorithmNames.Md5;

            HashAlgorithmProvider haprovider = HashAlgorithmProvider.OpenAlgorithm(strAgName);
            IBuffer mybfData = haprovider.HashData(mybf);

            if (mybfData.Length != haprovider.HashLength)
            {
                throw new ArgumentNullException("this is null can not create the hash");
            }
            string str64string = CryptographicBuffer.EncodeToHexString(mybfData);
            return str64string;
        }
    }
}
