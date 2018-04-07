using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace BJUTDUHelperXamarin.ViewModels
{
    public class NewsPageViewModel : BindableBase,INavigationAware
    {
        private INavigationService _navigationService;
        public NewsPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        private Models.NewsModel _news;

        public Models.NewsModel News
        {
            get { return _news; }
            set { SetProperty(ref _news , value); }
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
        }

        private Models.NewsHeaderModel _newsHeader;
        private Services.HttpBaseService _httpService;
        public void OnNavigatedTo(NavigationParameters parameters)
        {
            _newsHeader = parameters[typeof(Models.NewsHeaderModel).Name] as Models.NewsHeaderModel;
            _httpService = parameters["httpservice"] as Services.HttpBaseService;
            if (_newsHeader != null)
            {
                LoadNews();
            }
        }
        private bool _isLoading = false;
        public bool IsLoading
        {
            get { return IsLoading; }
            set { SetProperty(ref _isLoading, value); }
        }
        public async void LoadNews()
        {
            
            IsLoading = true;
            if (_newsHeader.ContentUri == "https://cjw1115.com/bjutduhelper/appview/")
            {
                try
                {
                    var _news = new Models.NewsModel();
                    _news.Title = "工大助手公告";
                    _news.Author = "工大助手团队";
                    _news.PostTime = DateTime.Now;
                    _news.Content = await _httpService.SendRequst(_newsHeader.ContentUri, HttpMethod.Get);
                    News = _news;
                }
                catch(HttpRequestException)
                {
                    Services.NotityService.Notify("网络不给力！/(ㄒoㄒ)/~~");
                    await _navigationService.GoBackAsync();
                }
                catch
                {
                    Services.NotityService.Notify("未知异常/(ㄒoㄒ)/~~");
                    await _navigationService.GoBackAsync();
                }
            }
            else
            {
                var re = await Services.NewsService.GetNews(_httpService, _newsHeader.NewsID);
                if (re != null)
                {
                    News = re;
                }
                else
                {
                    Services.NotityService.Notify("出现问题！/(ㄒoㄒ)/~~");
                    await _navigationService.GoBackAsync();
                }
            }
            
            IsLoading = false;
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
            
        }
    }
}
