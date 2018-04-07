using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Prism.Unity;
using BJUTDUHelperXamarin.Services;
using Xamarin.Forms;
using Android.Support.Design.Widget;
using BJUTDUHelperXamarin.Droid.Services;
using Microsoft.Practices.Unity;
using Plugin.Permissions;

namespace BJUTDUHelperXamarin.Droid
{
    [Activity(Theme = "@style/BJUTHelperTheme", Label = "工大助手", Icon = "@drawable/icon",  ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public MainActivity()
        {

        }
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.tabs;
            ToolbarResource = Resource.Layout.toolbar;
    
                 base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);

            
            LoadApplication(new App(new AndroidInitializer()));

            DependencyService.Register<ToastNotificator>(); // Register your dependency
            ToastNotificator.Init(this);

            Services.NotificationHubDroid.InitNotification();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

    }

    public class AndroidInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IUnityContainer container)
        {
            
        }
    }
}

