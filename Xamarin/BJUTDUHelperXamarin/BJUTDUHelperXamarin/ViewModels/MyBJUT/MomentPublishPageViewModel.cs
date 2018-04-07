using BJUTDUHelperXamarin.Models.MyBJUT;
using Plugin.Media.Abstractions;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Qiniu.IO;
using Qiniu.IO.Model;
using Qiniu.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BJUTDUHelperXamarin.ViewModels.MyBJUT
{
    public class MomentPublishPageViewModel : BindableBase,INavigationAware
    {
        private readonly string momentsUploadUri = "https://bjuthelper.cjw1115.com/api/moments/";
        INavigationService _navigationService;
        Services.HttpBaseService _httpService = new Services.HttpBaseService();
        public MomentPublishPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            PublishCommand = new DelegateCommand(Publish);
        }
        public DelegateCommand PublishCommand { get; set; }
        public async void Publish()
        {
            if (string.IsNullOrWhiteSpace(Content))
            {
                Services.NotityService.Notify("至少写点什么吧");
                return;
            }
            HttpClient client = null;
            IsLoading = true;
            try
            {
                var baseUri = "https://up-z1.qbox.me";
                List<String> imgUris = new List<string>();
                client = new HttpClient();
                for (int i = 0; i < Images.Count; i++)
                {
                    var index = Images[i].Path.LastIndexOf('/');
                    var fileName = Images[i].Path.Substring(index + 1, Images[i].Path.Length - index - 1);
                    fileName = DateTime.UtcNow.ToString("yyyyMMddHHmmssms") + fileName;

                    StreamContent imageContent = null;
                    var stream = Images[i].GetStream();
                    {
                        imageContent = new StreamContent(stream);
                    }
                    var content = new MultipartFormDataContent();
                    content.Add(new StringContent(fileName), "key");
                    content.Add(new StringContent(MakeToken(fileName)), "token");
                    content.Add(imageContent, "file", fileName);

                    var re = await client.PostAsync(baseUri, content);

                    if (re.IsSuccessStatusCode)
                    {
                        imgUris.Add(fileName);
                    }
#if DEBUG
                    var msg = await re.Content.ReadAsStringAsync();
                    Views.MyBJUT.MomentPublishPage.Instance.ShowMsg(msg);
#endif

                    stream.Dispose();
                }


                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("Content", Content);
                for (int i = 0; i < imgUris.Count; i++)
                {
                    dic.Add($"Imgs[{i}]", imgUris[i]);
                }
                var user = Services.UserService.Instance.LoadLocalUserinfo();
                dic.Add("Username", user.Username);
                var resoncseMsg = await _httpService.SendRequst(momentsUploadUri, HttpMethod.Post, dic, Authorization: Services.UserService.Instance.Token);

                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<Result>(resoncseMsg);
                string message = result.Msg;
                Services.NotityService.Notify(message);
                if (result.Code == 200)
                {
                    NavigationParameters param = new NavigationParameters();
                    //param.Add("IsLoad", true);
                    await _navigationService.GoBackAsync(param);
                }

            }
            catch (InvalidUserInfoException)
            {
                await _navigationService.NavigateAsync(typeof(Views.MyBJUT.LoginPage).Name);
            }
            catch (Exception e)
            {
                Services.NotityService.Notify("遇到错误");
#if DEBUG
                Views.MyBJUT.MomentPublishPage.Instance.ShowMsg(e.Message);
#endif

            }
            finally
            {
                client?.Dispose();
                IsLoading = false;
            }

            
        }
        string QINIU_AK = "9qRkG3lAmX4KHuukfL54aixliTHZsKJ0x8VA4XkW";
        string QINIU_SK = "Jn45dN55mDDzc8aOceNHESzPj6R9bMSLQlJsEzCI";
       
        public string MakeToken(string key)
        {
            PutPolicy putPolicy = new PutPolicy();
            putPolicy.Scope = "moments" + ":" + key;
            //putPolicy.Scope = "moments";
            putPolicy.SetExpires(600);

            string token = Auth.CreateUploadToken(new Mac(QINIU_AK,QINIU_SK),putPolicy.ToJsonString());
            return token;
        }
        public void OnNavigatedFrom(NavigationParameters parameters)
        {
            Images.Clear();
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            Images.Clear();
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
            
        }

        public static List<MediaFile> Images { get; set; } = new List<MediaFile>();

        public string _content;
        public string Content
        {
            get => _content;
            set
            {
                SetProperty(ref _content, value);
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { SetProperty(ref _isLoading, value); }
        }
    }
}
