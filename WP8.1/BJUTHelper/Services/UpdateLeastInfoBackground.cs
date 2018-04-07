using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace BJUTHelper.Services
{
    public class UpdateLeastInfoBackground : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            var deferral = taskInstance.GetDeferral();
            var toast_updater = ToastNotificationManager.CreateToastNotifier();
            string xml = "<toast lang=\"zh-CN\" launch='111' >" +
                         "<visual>" +
                            "<binding template=\"ToastGeneric\">" +
                                "<text>通知</text>" +
                                "<text>你好噻</text>" +
                            "</binding>" +
                         "</visual>" +
                        "</toast>";
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            // 显示通知
            ToastNotification notification = new ToastNotification(doc);
            toast_updater.Show(notification);

            deferral.Complete();
        }
    }
}
