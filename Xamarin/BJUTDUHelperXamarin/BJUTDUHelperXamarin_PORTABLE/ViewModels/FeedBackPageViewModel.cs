using Plugin.DeviceInfo;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace BJUTDUHelperXamarin.ViewModels
{
    public class FeedBackPageViewModel : BindableBase,INavigationAware
    {
        private Services.ToolService _toolService;
        private Services.HttpBaseService _httpService;
        private INavigationService _navigationService;
        private Models.FeedModel _feedModel;
        public Models.FeedModel FeedModel
        {
            get { return _feedModel; }
            set { SetProperty(ref _feedModel, value); }
        }

        private string version = null;
        public FeedBackPageViewModel(INavigationService navigationService)
        {
            _toolService = new Services.ToolService();
            _httpService = new Services.HttpBaseService();
            _navigationService = navigationService;


            SubmitCommand = new DelegateCommand(Submit);

            FeedModel = new Models.FeedModel();
        }
        public DelegateCommand SubmitCommand { get; set; }
        public async void Submit()
        {

            string modelVersion = $"Model:{CrossDeviceInfo.Current.Model}{Environment.NewLine}DeviceVersion:{CrossDeviceInfo.Current?.Version}Version:{version}";

            if (string.IsNullOrWhiteSpace(FeedModel.Content))
            {
                Services.NotityService.Notify("至少写点什么吧O(∩_∩)O");
                return;
            }
            try
            {
                FeedModel.Version = modelVersion;
                var re=await _toolService.Feed(_httpService, FeedModel);

                Services.NotityService.Notify(re);

                await _navigationService.GoBackAsync();
            }
            catch(HttpRequestException)
            {
                Services.NotityService.Notify("发送失败/(ㄒoㄒ)/~~");

            }
            catch
            {
                Services.NotityService.Notify("网络错误");
            }
           
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
            
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            version = (string)parameters["version"];
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
            
        }
    }
}
