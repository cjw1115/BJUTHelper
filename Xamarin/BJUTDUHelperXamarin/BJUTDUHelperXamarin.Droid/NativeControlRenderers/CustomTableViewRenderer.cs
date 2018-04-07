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
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using BJUTDUHelperXamarin.Droid.NativeControls;

[assembly: ExportRenderer(typeof(Xamarin.Forms.TableView), typeof(CustomTableViewRenderer))]
namespace BJUTDUHelperXamarin.Droid.NativeControls
{
    class CustomTableViewRenderer : TableViewRenderer
    {
        protected override TableViewModelRenderer GetModelRenderer(Android.Widget.ListView listView, TableView view)
        {
            return new CustomTableViewModelRenderer(Context,listView, view);
        }
    }

    public class CustomTableViewModelRenderer : TableViewModelRenderer
    {
        public CustomTableViewModelRenderer(Context context, Android.Widget.ListView listView, TableView view)
            : base(context, listView, view)
        {
        }
        public override Android.Views.View GetView(int position, Android.Views.View convertView, ViewGroup parent)
        {
            var view = base.GetView(position, convertView, parent);
            if (GetCellForPosition(position).GetType() != typeof(Xamarin.Forms.TextCell)) return view;
            var layout = (LinearLayout)view;
            var linearLayout = (LinearLayout)layout.GetChildAt(0);
            var linearLayout2 = (LinearLayout)linearLayout.GetChildAt(1);
            var text = (TextView)linearLayout2.GetChildAt(0);
            text.SetTextColor(((Color)App.Current.Resources["BJUTDUHelperSecondTextForground"]).ToAndroid());
            
            return view;
        }
    }
}