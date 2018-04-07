using BJUTDUHelperXamarin.Models.MyBJUT;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BJUTDUHelperXamarin.ViewModels.MyBJUT
{
    /// <summary>
    /// Navigation to this page,need navigation params ":
    /// "IsLoad",indicate if page need to load content when be opened.
    /// </summary>
	public class MomentsViewPageViewModel : BindableBase, INavigationAware
    {
        INavigationService _navigationService;
        Services.HttpBaseService _httpService;
        public MomentsViewPageViewModel(INavigationService navigationService)
        {
            _httpService = new Services.HttpBaseService();
            _navigationService = navigationService;
            PostCommand = new DelegateCommand(OpenPost);
            RefreshCommand = new DelegateCommand(Refresh);
            ItemTappedCommand = new DelegateCommand<object>(ItemTapped);
            LoadMoreCommand = new DelegateCommand(LoadMore);
        }
        public async void Load()
        {
            try
            {
                IsLoading = true;
                Moments = new ObservableCollection<MomentViewModel>();

                MomentViewModel[] re;
                re = await GetMoments(pageIndex, pageSize);
                
                if (re == null || re.Length == 0)
                {
                    this.CanLoadMore = false;
                }
                else
                {
                    foreach (var item in re)
                    {
                        Moments.Add(item);
                    }
                }

            }
            catch (Exception e)
            {
#if DEBUG
                Services.NotityService.DisplayAlert("Error", e.Message, "OK");
#else
                Services.NotityService.Notify("获取数据遇到错误");
#endif
            }
            finally
            {
                IsLoading = false;
            }

        }
        
        public DelegateCommand PostCommand { get; set; }
        public async void OpenPost()
        {
            if (Services.UserService.Instance.GetIsSignedIn())
            {
                await _navigationService.NavigateAsync(typeof(Views.MyBJUT.MomentPublishPage).Name);
            }
            else
            {
                Services.NotityService.Notify("需要先登录");
                await _navigationService.NavigateAsync(typeof(Views.MyBJUT.LoginPage).Name);
            }
        }
            

        private ObservableCollection<Models.MyBJUT.MomentViewModel> _moments;
        public ObservableCollection<Models.MyBJUT.MomentViewModel> Moments
        {
            get => _moments;
            set => SetProperty(ref _moments, value);
        }
        public void OnNavigatedFrom(NavigationParameters parameters)
        {

        }


        public void OnNavigatedTo(NavigationParameters parameters)
        {
            var re=parameters.Keys.Contains("IsLoad");
            if (re == true)
            {
                var value = (bool)parameters["IsLoad"];
                if (value == true)
                {
                    Load();
                }
            }
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
            
        }

        public ICommand ItemTappedCommand { get; set; }
        public async void ItemTapped(object model)
        {
            NavigationParameters parameters = new NavigationParameters();
            parameters.Add("IsLoad", true);
            parameters.Add("Moment", model);
            await _navigationService.NavigateAsync(typeof(Views.MyBJUT.MomentDetailPage).Name, parameters : parameters);
        }
        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { SetProperty(ref _isLoading, value); }
        }
        private bool _isRefreshing;
        public bool IsRefreshing
        {
            get { return _isRefreshing; }
            set { SetProperty(ref _isRefreshing, value); }
        }
        public DelegateCommand RefreshCommand { get; set; }
        public async void Refresh()
        {
            try
            {
                IsRefreshing = true;
                
                pageIndex = 0;
                var re=await GetMoments(pageIndex, pageSize);
                if (re == null || re.Length == 0)
                {
                    this.CanLoadMore = false;
                }
                else
                {
                    Moments = new ObservableCollection<MomentViewModel>();
                    foreach (var item in re)
                    {
                        Moments.Add(item);
                    }
                }

            }
            catch(Exception e)
            {
#if DEBUG
                Services.NotityService.DisplayAlert("Error", e.Message, "OK");
#else
                Services.NotityService.Notify("获取数据遇到错误");
#endif
            }
            finally
            {
                IsRefreshing = false;
            }

        }

        public DelegateCommand LoadMoreCommand { get; set; }
        public async void LoadMore()
        {
            try
            {
                IsLoadingMore = true;
                pageIndex++;
                var newMoments=await GetMoments(pageIndex, pageSize);
                if (newMoments == null||newMoments.Length==0)
                {
                    CanLoadMore = false;
                }
                else
                {
                    foreach (var item in newMoments)
                    {
                        Moments.Add(item);
                    }
                }
            }
            catch(Exception e)
            {
#if DEBUG
                Services.NotityService.DisplayAlert("Error", e.Message, "OK");
#else
                Services.NotityService.Notify("获取数据遇到错误");
#endif
            }
            finally
            {
                IsLoadingMore = false;
            }
            
        }

        private bool _isLoadingMore = false;
        public bool IsLoadingMore
        {
            get { return _isLoadingMore; }
            set { SetProperty(ref _isLoadingMore, value); }
        }

        private bool _canLoadMore = true;

        public bool CanLoadMore
        {
            get { return _canLoadMore; }
            set { SetProperty(ref _canLoadMore, value); }
        }

        private string _momentsList = "https://bjuthelper.cjw1115.com/api/moments";
        private int pageSize = DefaultPageSize;
        private const int DefaultPageSize = 10;
        private int pageIndex = 0;
        public async Task<MomentViewModel[]> GetMoments(int pageIndex,int pageSize)
        {
            try
            {

                //var uri = $"{_momentsList}?pageIndex={pageIndex}&pageSize={pageSize}&dayOffset={dayOffset}";
                var uri = $"{_momentsList}?pageIndex={pageIndex}&pageSize={pageSize}";
                var msg = await _httpService.SendRequst(uri, HttpMethod.Get);
                if (msg != null)
                {
                    var result = Newtonsoft.Json.JsonConvert.DeserializeObject<MomentResult>(msg);
                    var list = result.Data.ToArray();
                    foreach (var item in list)
                    {
                        item.FilterNicknameAndConent();
                    }

                    return list;
                }
                else
                {
                    Services.NotityService.Notify("请求服务器错误");
                    return null;
                    
                }
            }
            catch (Exception e)
            {
#if DEBUG
                Services.NotityService.DisplayAlert("Error", e.Message, "OK");
#else
                Services.NotityService.Notify("获取数据遇到错误");
#endif
            }
            return null;
        }
    }
    
}
