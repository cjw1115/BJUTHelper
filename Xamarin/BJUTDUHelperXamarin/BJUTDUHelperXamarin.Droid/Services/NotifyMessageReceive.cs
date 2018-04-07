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
using Com.Baidu.Android.Pushservice;
using WindowsAzure.Messaging;
using System.Threading.Tasks;

namespace BJUTDUHelperXamarin.Droid.Services
{
    public class BindedEventArgs:EventArgs
    {
        public string UserID { get; set; }
        public string ChannelID { get; set; }
    }
    public class NotifyMessageReceiver : PushMessageReceiver
    {
        public static event EventHandler<BindedEventArgs> Binded;
        public override void OnBind(Context p0, int p1, string p2, string p3, string p4, string p5)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"OnBind {p1} {p2} {p3}");
#endif



            BindedEventArgs args = new BindedEventArgs { UserID = p3, ChannelID = p4};
            Binded?.Invoke(this, args);
            
        }

        public override void OnDelTags(Context p0, int p1, IList<string> p2, IList<string> p3, string p4)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("OnDelTags");
#endif

        }

        public override void OnListTags(Context p0, int p1, IList<string> p2, string p3)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("OnListTags");
#endif

        }

        public override void OnMessage(Context p0, string p1, string p2)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("OnMessage");
#endif

        }

        public override void OnNotificationArrived(Context p0, string p1, string p2, string p3)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("OnNotificationArrived");
#endif

        }

        public override void OnNotificationClicked(Context p0, string p1, string p2, string p3)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("OnNotificationClicked");
#endif

        }



        public override void OnSetTags(Context p0, int p1, IList<string> p2, IList<string> p3, string p4)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("OnSetTags");
#endif

        }

        public override void OnUnbind(Context p0, int p1, string p2)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("OnUnbind");
#endif

            //hub.Unregister();
        }
    }
}