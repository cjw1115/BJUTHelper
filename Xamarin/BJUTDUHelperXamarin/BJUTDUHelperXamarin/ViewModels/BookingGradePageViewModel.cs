using BJUTDUHelperXamarin.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BJUTDUHelperXamarin.ViewModels
{
    public class BookingGradePageViewModel : BindableBase,INavigationAware
    {
        public  Models.BJUTEduCenterUserinfo BJUTEduCenterUserinfo { get; set; }
        public  Services.HttpBaseService _httpService { get; set; } = new Services.HttpBaseService(true);
        public Services.MyBjutService bjutService { get; set; } 

        private INavigationService _navigationService;
        private DbService _dbService;

        private Services.INotificationHub _notifyHub;
        public BookingGradePageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

            _dbService = new DbService();

            _notifyHub = Services.NotificationHub.GetNotificationHub();

            UnBookingGradeCommand = new DelegateCommand(UnBookingGrade);
            BookingGradeCommand = new DelegateCommand(BookingGrade);
        }

        public async void OnNavigatedTo(NavigationParameters parameters)
        {

            //var frompage = parameters["from"] as Type;
            //if (frompage == null)
            //{
            //    Loaded();
            //}
            //if (frompage == typeof(Views.UserInfoDetailPage))
            //{
            //    var eduuser = _dbService.GetAll<Models.BJUTEduCenterUserinfo>().FirstOrDefault();
            //    BJUTEduCenterUserinfo = eduuser;
            //}
            Loaded();
        }
        public void OnNavigatedFrom(NavigationParameters parameters)
        {
        }

        public void Loaded()
        {
            var eduuser = _dbService.GetAll<Models.BJUTEduCenterUserinfo>().FirstOrDefault();
            BJUTEduCenterUserinfo = eduuser;

            GetServerStatus();
            GetIsBookingGrade();
            
        }
        private bool _isLoading;

        public bool IsLoading
        {
            get { return _isLoading; }
            set { SetProperty(ref _isLoading , value); }
        }

        private bool hasBookingGrade = false;

        public bool HasBookingGrade 
        {
            get { return hasBookingGrade; }
            set { SetProperty(ref hasBookingGrade , value); }
        }

        public async void GetIsBookingGrade()
        {
            if (BJUTEduCenterUserinfo == null || string.IsNullOrWhiteSpace(BJUTEduCenterUserinfo.Username) || string.IsNullOrWhiteSpace(BJUTEduCenterUserinfo.Username))

            {
                Services.NotityService.Notify("教务管理账号信息不全");
                return;
            }
            IsLoading = true;
            try
            {
                var status = await MyBjutService.GetIsBookingGrade(_httpService, BJUTEduCenterUserinfo.Username);
                HasBookingGrade = status;
            }
            catch (System.Net.Http.HttpRequestException)
            {
                Services.NotityService.Notify("网络异常");
            }
            finally
            {
                IsLoading = false;
            }
        }
        public async void GetServerStatus()
        {
            try
            {
                var status = await MyBjutService.GetBookingGradeServerStatus(_httpService);
                ServerStatus = status;
            }
            catch (System.Net.Http.HttpRequestException)
            {
                Services.NotityService.Notify("网络异常");
            }
            finally
            {
            }
        }
        private string _serverStatus;

        public string ServerStatus
        {
            get { return _serverStatus; }
            set { SetProperty(ref _serverStatus , value); }
        }


        public DelegateCommand BookingGradeCommand { get; set; }
        public async void BookingGrade()
        {
            if (BJUTEduCenterUserinfo == null || string.IsNullOrWhiteSpace(BJUTEduCenterUserinfo.Username) || string.IsNullOrWhiteSpace(BJUTEduCenterUserinfo.Username))
            {
                await EduPageViewModel.GetUserInfo(_navigationService);
                return;
            }

            IsLoading = true;

            try
            {

                var subResult = await _notifyHub.InitNotificationHubAsync(Services.ConfigurationSettings.HUB_NAME, Services.ConfigurationSettings.HUB_CONNEXTION_STRING, new[] { BJUTEduCenterUserinfo.Username });
                if (_notifyHub == null)
                {
                    Services.NotityService.Notify("服务初始化异常");
                    return;
                }
                if(subResult==false)
                {
                    Services.NotityService.Notify("手机推送服务开启失败");
                    return;
                }


                var re = await Services.MyBjutService.BookingGrade(_httpService, BJUTEduCenterUserinfo.Username, BJUTEduCenterUserinfo.Password);
                switch (re.Code)
                {
                    case 200:
                        
                        HasBookingGrade = true;

                        Services.NotityService.Notify("订阅成功");
                        GetServerStatus();
                        break;
                    case 300:
                        Services.NotityService.Notify("账户检查不通过，请重新录入学号密码");
                        await EduPageViewModel.GetUserInfo(_navigationService);
                        break;
                    case 400:
                    case 401:
                        Services.NotityService.Notify("提交的参数错误");
                        break;
                    
                    case 500:
                        Services.NotityService.Notify("服务异常");
                        break;
                    default:
                        Services.NotityService.Notify("未知错误");
                        break;
                }
            }
            catch (System.Net.Http.HttpRequestException)
            {
                Services.NotityService.Notify("网络异常");
            }
            catch
            {
                Services.NotityService.Notify("其他异常");
            }
            finally
            {
                IsLoading = false;
            }
            
        }

        public DelegateCommand UnBookingGradeCommand { get; set; }
        public async void UnBookingGrade()
        {
            if (BJUTEduCenterUserinfo == null || string.IsNullOrWhiteSpace(BJUTEduCenterUserinfo.Username) || string.IsNullOrWhiteSpace(BJUTEduCenterUserinfo.Username))
            {
                await EduPageViewModel.GetUserInfo(_navigationService);
                return;
            }
            IsLoading = true;
            try
            {
                var re = await Services.MyBjutService.UnBookingGrade(_httpService, BJUTEduCenterUserinfo.Username, BJUTEduCenterUserinfo.Password);
                switch (re.Code)
                {
                    case 200:
                        Services.NotityService.Notify("取消订阅成功");
                        HasBookingGrade = false;
                        GetServerStatus();
                        break;
                    case 400:
                    case 401:
                        Services.NotityService.Notify("提交的参数错误");
                        break;

                    case 301:
                        Services.NotityService.Notify("提交的信息和订阅时不一致");
                        await EduPageViewModel.GetUserInfo(_navigationService);
                        break;
                    case 500:
                        Services.NotityService.Notify("服务出错，稍后尝试");
                        break;
                    default:
                        Services.NotityService.Notify("未知错误");
                        break;
                }
            }
            catch (System.Net.Http.HttpRequestException)
            {
                Services.NotityService.Notify("网络异常");
            }
            catch
            {
                Services.NotityService.Notify("其他异常");
            }

            finally
            {
                IsLoading = false;
            }

        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
            
        }
    }
}
