using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace BJUTHelper
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Setting : Page
    {
        public Setting()
        {
            this.InitializeComponent();
            var notification = Windows.Storage.ApplicationData.Current.LocalSettings.Values["Notification"];
            if (notification != null)
            {
                notificationSwitch.IsOn=(bool)notification;
            }
        }

        //推送通知开关，Notification
        private void notificationSwitch_Tapped(object sender, TappedRoutedEventArgs e)
        {
            notificationSwitch.IsOn = !notificationSwitch.IsOn;
            if (notificationSwitch.IsOn)
            {

            }
        }
        //public async void Access()
        //{
        //    try
        //    {
        //        BackgroundExecutionManager.RemoveAccess();
        //        var access = await BackgroundExecutionManager.RequestAccessAsync();
        //        if (access == BackgroundAccessStatus.Denied || access == BackgroundAccessStatus.Unspecified)
        //        {
        //            await new MessageDialog("系统关闭了后台运行，请前往‘系统设置’进行设置").ShowAsync();
        //            return;

        //        }
        //        var task = BackgroundTaskRegistration.AllTasks.Values.FirstOrDefault(m => m.Name == "bjutNotification");
        //        if (task == null)
        //        {
        //            BackgroundTaskBuilder builder = new BackgroundTaskBuilder();
        //            builder.Name = "bjutNotification";
        //            builder.TaskEntryPoint = typeof(UpdateLeastInfoBackground).FullName;
        //            var triger = new SystemTrigger(SystemTriggerType.UserAway, false);

        //            builder.SetTrigger(triger);
        //            task = builder.Register();
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }

        //}

    }
}
