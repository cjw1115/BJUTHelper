using BJUTDUHelperXamarin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTGSnackBar;
using UIKit;

namespace BJUTDUHelperXamarin.iOS.Services
{
    public class ToastNotificator : IToastNotificator
    {
        public void Notify(string message)
        {
            var snackbar = new TTGSnackbar(message);
            snackbar.ActionText = "OK";
            snackbar.ActionBlock = (o) => { };
            snackbar.ActionTextColor = UIColor.FromRGB(29,145,255);
            snackbar.Duration = TimeSpan.FromSeconds(3);

            snackbar.AnimationType = TTGSnackbarAnimationType.SlideFromBottomBackToBottom;
            snackbar.SeperateView.Alpha = 0;

            snackbar.LeftMargin = 0;
            snackbar.RightMargin = 0;
            snackbar.BottomMargin = 0;
            snackbar.CornerRadius = 0;
            snackbar.Show();
        }
    }
}