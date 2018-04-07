using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BJUTDUHelperXamarin.iOS.Services;
using Xamarin.Forms;
using BJUTDUHelperXamarin.Models;
using Foundation;

[assembly: Dependency(typeof(ApplicationInfo))]
namespace BJUTDUHelperXamarin.iOS.Services
{
    public class ApplicationInfo: IApplicationInfo
    {
        public string GetVersion()
        {
            var name= NSBundle.MainBundle.ObjectForInfoDictionary("CFBundleShortVersionString").ToString();
            return name;

           
        }
    }
}