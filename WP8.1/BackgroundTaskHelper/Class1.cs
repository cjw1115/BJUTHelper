using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace BackgroundTaskHelper
{
    public sealed class ToastNotificationTask : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            throw new NotImplementedException();
        }
        //public async Task GetNetInfo()
        //{
        //    try
        //    {
        //        using (HttpClient client = new HttpClient())
        //        {

        //            string uriStr = "http://www.cjw1115.com/Info?id=" + DateTime.Now;
        //            using (HttpResponseMessage respnese = await client.GetAsync(new Uri(uriStr)))
        //            {
        //                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
        //                {
        //                    if (infoList == null)
        //                    { infoList = new ObservableCollection<InfoEntity>(); }

        //                    //获取json信息
        //                    string json = await respnese.Content.ReadAsStringAsync();
        //                    var list = WebInfoContent.PraseJson(json);
        //                    //添加前清空数据
        //                    infoList.Clear();
        //                    foreach (var item in list)
        //                    {
        //                        infoList.Add(item);
        //                    }
        //                    WriteWebInfoToLocal(infoList);
        //                });

        //            }
        //        }
        //    }
        //    catch (Exception ex) { }
        //}
    }
}
