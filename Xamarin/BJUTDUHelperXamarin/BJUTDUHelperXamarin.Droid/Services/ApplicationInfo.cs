using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BJUTDUHelperXamarin.Models;
using Xamarin.Forms;
using BJUTDUHelperXamarin.Droid.Services;

[assembly: Dependency(typeof(ApplicationInfo))]
namespace BJUTDUHelperXamarin.Droid.Services
{
    public class ApplicationInfo: IApplicationInfo
    {
        public string GetVersion()
        {
            var name = Forms.Context.PackageManager.GetPackageInfo(Forms.Context.PackageName, 0).VersionName;
            return name;

           
        }
    }
}