using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace BJUTDUHelperXamarin.Util
{
    public class Sha1
    {
        public static string Hmac(string value, string key)
        {
            byte[] bytes;
            using (var hmac = new HMACMD5(Encoding.UTF8.GetBytes(key)))
            {
                bytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(value));
            }
            StringBuilder result = new StringBuilder();
            foreach (byte t in bytes)
            {
                result.Append(t.ToString("X2"));
            }
            return result.ToString();
        }

    }
   

}
