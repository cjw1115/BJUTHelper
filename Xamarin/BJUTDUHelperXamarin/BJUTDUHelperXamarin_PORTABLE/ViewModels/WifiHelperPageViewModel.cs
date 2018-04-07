using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using BJUTDUHelperXamarin.Services;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Net.Http;
using System.Collections.ObjectModel;
using BJUTDUHelperXamarin.Models;
using Xamarin.Forms;

namespace BJUTDUHelperXamarin.ViewModels
{
    public class WifiHelperPageViewModel : BindableBase, INavigationAware
    {
        private Services.WifiService _wifiService;
        private DbService _dbService;
        private INavigationService _navigationService;
        private HttpBaseService _httpService;

        private BJUTInfoCenterUserinfo _BJUTInfoCenterUserinfo;
        public BJUTInfoCenterUserinfo BJUTInfoCenterUserinfo
        {
            get { return _BJUTInfoCenterUserinfo; }
            set { _BJUTInfoCenterUserinfo = value; }
        }

        private InfoCenterAccountInfo _accountInfo;
        public InfoCenterAccountInfo AccountInfo
        {
            get { return _accountInfo; }
            set { SetProperty(ref _accountInfo, value); }
        }

        private bool isAuthencated = false;
        private bool _isRegisted = false;
        public bool IsRegisted
        {
            get { return _isRegisted; }
            set { SetProperty(ref _isRegisted, value); }
        }

        private bool _isLoading = false;

        public bool IsLoading
        {
            get { return _isLoading; }
            set {SetProperty(ref  _isLoading , value); }
        }

        private bool _isOnlyWlan = false;

        public bool IsOnlyWlan
        {
            get { return _isOnlyWlan; }
            set { SetProperty(ref _isOnlyWlan , value); }
        }


        public WifiHelperPageViewModel(INavigationService navigationService)
        {
            _httpService = new HttpBaseService();
            _wifiService = new WifiService(_httpService);
            _navigationService = navigationService;
            _dbService = new DbService();

            LoginCommand = new DelegateCommand(Login);
            LoadedCommand = new DelegateCommand(Loaded);
            LogoutCommand = new DelegateCommand(Logout);
        }
        public DelegateCommand LoadedCommand { get; set; }
        public async void Loaded()
        {
            var eduuser = _dbService.GetAll<Models.BJUTInfoCenterUserinfo>().FirstOrDefault();
            if (eduuser == null)
            {
                await GetUserInfo();
                return;
            }
            BJUTInfoCenterUserinfo = eduuser;

            try
            {
                isAuthencated = await _wifiService.GetAuthenStatus();
                IsRegisted = await _wifiService.GetRegistStatus();
                if (IsRegisted)
                {
                    AccountInfo = await _wifiService.GetAccountBasicInfo(BJUTInfoCenterUserinfo.Username, BJUTInfoCenterUserinfo.Password);
                }
            }
            catch
            {
                Services.NotityService.Notify("尝试获取基础信息失败");
            }

        }

        
        
        public void OnNavigatedFrom(NavigationParameters parameters)
        {

        }
        private bool isLoginError = false;
        public async void OnNavigatedTo(NavigationParameters parameters)
        {
            if (parameters != null)
            {
                var from = (Type)parameters["from"];
                if (from == typeof(Views.UserInfoDetailPage) && isLoginError == true)
                {
                    var infouser = _dbService.GetAll<Models.BJUTInfoCenterUserinfo>().FirstOrDefault();
                    BJUTInfoCenterUserinfo = infouser;
                    Login();
                }
            }
        }

        public DelegateCommand LoginCommand { get; set; }
        public async void Login()
        {
            IsLoading = true;
           
            try
            {
                string ssid = "bjut_wifi";

                if (!isAuthencated)
                {
                    await _wifiService.Authenticate(BJUTInfoCenterUserinfo.Username, BJUTInfoCenterUserinfo.Password, ssid);

                    Services.NotityService.Notify("认证成功");
                }
                if (!IsOnlyWlan)
                {
                    IsRegisted = await _wifiService.GetRegistStatus();
                    if (!IsRegisted)
                    {
                        await _wifiService.Register(BJUTInfoCenterUserinfo.Username, BJUTInfoCenterUserinfo.Password);
                    }
                    IsRegisted = true;
                    Services.NotityService.Notify("网关注册成功");

                    var info = await _wifiService.GetAccountBasicInfo(BJUTInfoCenterUserinfo.Username, BJUTInfoCenterUserinfo.Password);
                    AccountInfo = info;
                }
                
                
            }
            catch (InvalidUserInfoException invalidUserInfoException)
            {
                Services.NotityService.Notify("用户名或密码错误，请重新输入");

                isLoginError = true;
                await GetUserInfo();
            }
            catch (LoginTipException loginTipException)
            {
                Services.NotityService.Notify(loginTipException.Message);
            }
            catch(HttpRequestException httpRequestException)
            {
                Services.NotityService.Notify("网络异常，请检查是否连接到校内WIFI：bjut_wifi");
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


        public DelegateCommand LogoutCommand { get; set; }
        public async void Logout()
        {
            IsLoading = true;

            IsRegisted = await _wifiService.GetRegistStatus();
            if (IsRegisted)
            {
                await _wifiService.Logout();
            }
            IsRegisted = false;

            IsLoading = false;

            Services.NotityService.Notify("注销登录成功");
        }
        /// <summary>
        /// 打开账号添加页面
        /// </summary>
        /// <returns></returns>
        public async Task GetUserInfo()
        {
            BJUTInfoCenterUserinfo = null;

            NavigationParameters naviParam = new NavigationParameters();
            naviParam.Add("title", "信息门户");
            naviParam.Add("model", BJUTInfoCenterUserinfo);
            await _navigationService.NavigateAsync("UserInfoDetailPage", naviParam);//提示重新输入账号
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
            
        }
    }
}
