using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BJUTDUHelperXamarin.Models
{
    public class Settings
    {
        public static void InitSetting()
        {
            if (null == EduProxySetting)
            {
                EduProxySetting = true;
            }
        }

        //null:0 true:1 false:2
        public static bool? EduProxySetting
        {
            get
            {
                var re = Plugin.Settings.CrossSettings.Current.GetValueOrDefault<int>("eduproxy", 0);
                if (re == 0)
                    return null;
                else if (re == 1)
                    return true;
                else
                    return false ;
            }
            set
            {
                if (value == null)
                    Plugin.Settings.CrossSettings.Current.AddOrUpdateValue<int>("eduproxy", 0);
                else if (value == true)
                    Plugin.Settings.CrossSettings.Current.AddOrUpdateValue<int>("eduproxy", 1);
                else
                    Plugin.Settings.CrossSettings.Current.AddOrUpdateValue<int>("eduproxy", 2);

            }
        }

        public static bool? EduExperimentalSetting
        {
            get
            {
                var re = Plugin.Settings.CrossSettings.Current.GetValueOrDefault<int>("eduxxperimental", 0);
                if (re == 0)
                    return null;
                else if (re == 1)
                    return true;
                else
                    return false;
            }
            set
            {
                if (value == null)
                    Plugin.Settings.CrossSettings.Current.AddOrUpdateValue<int>("eduxxperimental", 0);
                else if (value == true)
                    Plugin.Settings.CrossSettings.Current.AddOrUpdateValue<int>("eduxxperimental", 1);
                else
                    Plugin.Settings.CrossSettings.Current.AddOrUpdateValue<int>("eduxxperimental", 2);

            }
        }
    }
}
