using Android.App;
using Android.Support.Design.Widget;
using Android.Text;
using Android.Views;
using BJUTDUHelperXamarin.Models;
using System;
using System.Collections.Generic;
using System.Threading;
namespace BJUTDUHelperXamarin.Droid.Services
{
    public class ToastNotificator: IToastNotificator
    {
        private static Activity _activity;
        public static void Init(Activity activity)
        {
            _activity = activity;
        } 
        public void Notify(string message)
        {
            var view=_activity.FindViewById(Android.Resource.Id.Content);
            Snackbar.Make(view, message, Snackbar.LengthLong).SetAction("Ok", (call) => { }).Show() ;
        }
    }
}