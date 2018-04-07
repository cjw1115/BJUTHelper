using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace BJUTDUHelperXamarin.Helpers
{
    public class Utility
    {
        public static string GetMD5(string str)
        {
            byte[] data = Encoding.UTF8.GetBytes(str);
            
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] OutBytes = md5.ComputeHash(data);

            string OutString = "";
            for (int i = 0; i < OutBytes.Length; i++)
            {
                OutString += OutBytes[i].ToString("x2");
            }
            return OutString.ToLower();
        }
    }
}
