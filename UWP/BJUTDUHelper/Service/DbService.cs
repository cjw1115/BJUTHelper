using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BJUTDUHelper.Service
{
    public class DbService
    {
        public static BJUTDUHelper.Model.BJUTDUHelperDbContext BJUEDUHelperDbContext { get; set; } = new Model.BJUTDUHelperDbContext();
        public static void SaveInfoCenterUserinfo<T>(T userinfo)where T:Model.UserBase,new()
        {
            var user = BJUEDUHelperDbContext.Set<T>().Where(m => m.Username == userinfo.Username).FirstOrDefault();
            if (user == null)
            {
                user = new T();
                user.Username = userinfo.Username;
                user.Password = userinfo.Password;

                BJUEDUHelperDbContext.Set<T>().Add(user);
            }
            else
            {
                user.Password = userinfo.Password;
                BJUEDUHelperDbContext.Set<T>().Update(user);
            }
            BJUEDUHelperDbContext.SaveChanges();
        }
        public static void RemoveInfoCenterUserinfo<T>(string username) where T : Model.UserBase, new()
        {
            var user = BJUEDUHelperDbContext.Set<T>().Where(m => m.Username == username).FirstOrDefault();
            if (user != null)
            {
                BJUEDUHelperDbContext.Set<T>().Remove(user);
            }
            BJUEDUHelperDbContext.SaveChanges();
        }
        public static List<T> GetInfoCenterUserinfo<T>() where T : Model.UserBase
        {
            List<T> list = new List<T>(0);
            var user = BJUEDUHelperDbContext.Set<T>().ToList();
            return user?? list;
        }
        public static T GetInfoCenterUserinfo<T>(string username) where T : Model.UserBase
        {
            var user = BJUEDUHelperDbContext.Set<T>().Where(m => m.Username == username).FirstOrDefault();
            return user;
        }

    }
}
