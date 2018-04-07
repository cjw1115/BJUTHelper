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
using Xamarin.Forms;

using BJUTDUHelperXamarin.Models;
using BJUTDUHelperXamarin.Droid.Services;

[assembly: Dependency(typeof(ResourceDriod))]
namespace BJUTDUHelperXamarin.Droid.Services
{

    public class ResourceDriod : IResourceUri
    {
        public string GetUri()
        {
            return "file:///android_asset/";
        }
    }
}