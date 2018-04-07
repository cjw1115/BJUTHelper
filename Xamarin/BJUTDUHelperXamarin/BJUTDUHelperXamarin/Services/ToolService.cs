
using BJUTDUHelperXamarin.Models;
using Plugin.DeviceInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
namespace BJUTDUHelperXamarin.Services
{
    public class ToolService
    {
        private readonly string feedUri = "https://www.cjw1115.com/bjutduhelper/feed";
        private static readonly string pantUri = "https://www.cjw1115.com/bjutduhelper/pant";
        private  static readonly string latestVersionCodeUri= "https://www.cjw1115.com/bjutduhelper/latestversion";
        private class ResualtModel
        {
            public string message { get; set; }
            public int code { get; set; }
        }
        
        public ToolService()
        {

        }
        public async Task<string> Feed(Services.HttpBaseService httpService, Models.FeedModel model)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("Content", model.Content);
            dic.Add("Contact", model.Contact);
            dic.Add("FeedTime", DateTime.Now.ToString());
            dic.Add("Version", model.Version);
            string re = null;
            try
            {
                re = await httpService.SendRequst(feedUri, HttpMethod.Post, dic);
                var resultModel = Newtonsoft.Json.JsonConvert.DeserializeObject<ResualtModel>(re);
                return resultModel.message;
            }
            catch
            {
                throw;
            }
        }

        public async static Task<UpdateModel> GetLatestVersionCode(Services.HttpBaseService httpService)
        {
            UpdateModel updateModel = null;
            try
            {
                var re = await httpService.SendRequst(latestVersionCodeUri, HttpMethod.Get);
                updateModel = Newtonsoft.Json.JsonConvert.DeserializeObject<UpdateModel>(re);
                return updateModel;
            }
            catch
            {
                throw;
            }
        }
        
        public static async Task<bool> CheckUpdate(Services.HttpBaseService _httpService)
        {
            try
            {
                var version = Xamarin.Forms.DependencyService.Get<Models.IApplicationInfo>().GetVersion();
                
                var updateModel = await Services.ToolService.GetLatestVersionCode(_httpService);
                if (updateModel != null && version != null)
                {
                    //if(updateModel.LatestVersion != version)
                    var newVerisons=updateModel.LatestVersion.Split('.');

                    var nowVersions = version.Split('.');
                    var hasNew = false;
                    for (int i = 0; i < newVerisons.Length; i++)
                    {
                        var newNum = int.Parse(newVerisons[i]);
                        var nowNum = int.Parse(nowVersions[i]);
                        if (newNum > nowNum)
                        {
                            hasNew = true;
                            break;
                        }
                        else if(newNum<nowNum)
                        {
                            break;
                        }
                    }
                    if (hasNew)
                    {
                        if (Device.OS == TargetPlatform.Android)
                        {
                            Services.NotityService.Notify($"检测到新版本{updateModel.LatestVersion},开始下载...");
                            Device.OpenUri(new Uri(updateModel.DownloadUri));

                        }
                        else if (Device.OS == TargetPlatform.iOS)
                        {
                            Services.NotityService.Notify($"检测到新版本{updateModel.LatestVersion},请在应用商店检查更新");
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch
            {
                
            }
            return false;
        }

        public static async Task Pant(HttpBaseService _httpService)
        {
            var version = Xamarin.Forms.DependencyService.Get<Models.IApplicationInfo>().GetVersion();
            await _httpService.GetResponseCode($"{pantUri}?device={CrossDeviceInfo.Current.Model}&version={version}");
        }
    }
}
