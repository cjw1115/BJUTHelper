using System;
using System.Collections.Generic;
using System.Text;
using BJUTDUHelperXamarin.Models;
using Xamarin.Forms;
using Foundation;
using BJUTDUHelperXamarin.iOS.Services;

[assembly: Dependency(typeof(ResourceIos))]
namespace BJUTDUHelperXamarin.iOS.Services
{
    public class ResourceIos : IResourceUri
    {
        public string GetUri()
        {
            return NSBundle.MainBundle.BundlePath;
        }
    }
}
