using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace BJUTDUHelperXamarin.ViewModels
{
    public class AboutPageViewModel : BindableBase,INavigationAware
    {
        private INavigationService _navigationService;
        private Services.HttpBaseService _httpService;
        public AboutPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            _httpService = new Services.HttpBaseService();
            FeedCommand = new DelegateCommand(Feed);
            UpdateCommand = new DelegateCommand(Update);

            Version = Xamarin.Forms.DependencyService.Get<Models.IApplicationInfo>().GetVersion();
        }
        public DelegateCommand FeedCommand { get; set; }
        public async void Feed()
        {
            NavigationParameters naviParam = new NavigationParameters();
            naviParam.Add("version", Version);
            await _navigationService.NavigateAsync(typeof(Views.FeedBackPage).Name,naviParam);
        }
        
        public DelegateCommand UpdateCommand { get; set; }
        public async void Update()
        {
            //var updateModel=await Services.ToolService.GetLatestVersionCode(_httpService);
            //if(updateModel != null&& Version!=null&& updateModel.LatestVersion != Version)
            //{
            //    //非最新版本，开启最新更新版本

            //    if(Device.OS== TargetPlatform.Android)
            //    {
            //        Services.NotityService.Notify($"检测到新版本{updateModel.LatestVersion},开始下载...");
            //        Device.OpenUri(new Uri(updateModel.DownloadUri));

            //    }
            //    else if(Device.OS== TargetPlatform.iOS)
            //    {
            //        Services.NotityService.Notify($"检测到新版本{updateModel.LatestVersion},请打开AppStore更新");
            //    }
            //}
            //else
            //{
            //    Services.NotityService.Notify("已是最新版本O(∩_∩)O");
            //}

            var re=await Services.ToolService.CheckUpdate(_httpService);

            if (re == false)
            {
                Services.NotityService.Notify("已是最新版本O(∩_∩)O");
            }
            
        }


        public void OnNavigatedFrom(NavigationParameters parameters)
        {
            
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
            
        }

        private string _version;

        public string Version
        {
            get { return _version; }
            set { _version = value; }
        }


    }
}
