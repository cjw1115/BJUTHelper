using BJUTDUHelperXamarin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BJUTDUHelperXamarin.Services
{
    public class NotityService
    {
        private static IToastNotificator _toastNotificator = DependencyService.Get<IToastNotificator>();
        public static void Notify(string message)
        {
            if (_toastNotificator == null)
            {
                _toastNotificator = DependencyService.Get<IToastNotificator>();
            }
            _toastNotificator?.Notify(message) ;
            //NotificationOptions option = new NotificationOptions();

            //option.Description=message;
            //if (_toastNotificator == null)
            //{
            //    _toastNotificator = DependencyService.Get<IToastNotificator>();
            //}
            //_toastNotificator?.Notify(option);

        }

        public static async Task DisplayAlert(string Title, string Message,string ok = "好")
        {
            await Xamarin.Forms.Application.Current.MainPage.DisplayAlert(Title, Message, ok);
        }
        public static async Task<bool> DisplayQueryAlert(string Title, string Message,string accept="确定",string cancel="取消")
        {
            var re=await Xamarin.Forms.Application.Current.MainPage.DisplayAlert(Title, Message, accept, cancel);
            return re;
        }

    }
}
