using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using BJUTHelper.Model;
namespace BJUTHelper
{
    public class User<T> where T :UserEntity, new()
    {
        public T UserInfo { get; set; }
        
        public User()
        {
            try
            {
                UserInfo = new T();
                string value = string.Empty;
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

                UserInfo.UserName = (string)localSettings.Values[typeof(T).ToString()];
                UserInfo.Password = (string)localSettings.Values[typeof(T).ToString()+"pwd"];
                
            }
            catch { }
            
        }
        public void SaveUserInfo()
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (UserInfo.UserName == null)
                UserInfo.UserName = string.Empty;
            
            if (UserInfo.Password == null)
                UserInfo.Password = string.Empty;
            localSettings.Values[typeof(T).ToString()] = UserInfo.UserName;
            localSettings.Values[typeof(T).ToString()+"pwd"] = UserInfo.Password;
           
        }
       
    }
}
