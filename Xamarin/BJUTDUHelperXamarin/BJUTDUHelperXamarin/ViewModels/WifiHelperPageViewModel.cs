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
using BJUTDUHelperXamarin.Models.NetModels;

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
            set { SetProperty(ref _BJUTInfoCenterUserinfo , value); }
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

        private OnlineAccountModel _selectedAccount;

        public OnlineAccountModel SelectedAccount
        {
            get { return _selectedAccount; }
            set { SetProperty(ref _selectedAccount , value); }
        }

        private ObservableCollection<OnlineAccountModel> _onlineAccounts = new ObservableCollection<OnlineAccountModel>();
        public ObservableCollection<OnlineAccountModel> OnlineAccounts
        {
            get { return _onlineAccounts; }
            set { SetProperty(ref _onlineAccounts , value); }
        }
        public bool _canOffline;
        public bool CanOffline
        {
            get { return _canOffline; }
            set { SetProperty(ref _canOffline, value); }
        }
        public DelegateCommand OfflineCommand { get; set; }



        public WifiHelperPageViewModel(INavigationService navigationService)
        {
            _httpService = new HttpBaseService();
            _wifiService = new WifiService(_httpService);
            _navigationService = navigationService;
            _dbService = new DbService();

            LoginCommand = new DelegateCommand(Login);
            LoadedCommand = new DelegateCommand(Loaded);
            LogoutCommand = new DelegateCommand(Logout);
            OfflineCommand = new DelegateCommand(Offline);
        }

       

        public DelegateCommand LoadedCommand { get; set; }
        public async void Loaded()
        {
            
            IsLoading = true;

            var infoUser = _dbService.GetAll<Models.BJUTInfoCenterUserinfo>().FirstOrDefault();
            if (infoUser == null)
            {
                await GetUserInfo();
                return;
            }
            BJUTInfoCenterUserinfo = infoUser;

            if (!NetworkHelper.IsConected)
            {
                IsNoneConnection = true;
            }
            else
            {
                IsNoneConnection = false;
            }
            var connectionType = NetworkHelper.GetConnectType();
            if (connectionType == ConnectionType.Cellular)
            {
                IsNoneConnection = true;
            }
            else if(connectionType== ConnectionType.None)
            {
                IsNoneConnection = true;
            }
            else
            {
                var ssid=NetworkHelper.GetCurrentSSID();

                //var re=await _wifiService.GetIsBjutWifi();//be sure it will return true or false.
                if ((!string.IsNullOrEmpty(ssid)) && ssid.ToLower().Contains("bjut_wifi") || ssid.ToLower().Contains("tushuguan"))
                {
                    //bjut_wifi
                    IsNoneConnection = false;
                    try
                    {
                        LoadRegistedInfo();

                        GetAccountInfo();

                        //if (IsRegisted)
                        //{
                        //    GetOnlineAccount();
                        //}
                        //else
                        //{
                        //    var isLoginAccountCenter = await _wifiService.LoginAccountCenter(BJUTInfoCenterUserinfo.Username, BJUTInfoCenterUserinfo.Password);
                        //    if (isLoginAccountCenter == true)
                        //    {
                        //        GetOnlineAccount();
                        //    }
                        //    else
                        //    {

                        //    }
                        //}

                    }
                    catch(InvalidUserInfoException)
                    {
#if DEBUG
                        Services.NotityService.Notify($"DEBUG:用户名或密码错误");
#endif
                    }
                    catch (Exception e)
                    {
#if DEBUG
                        Services.NotityService.Notify($"DEBUG {e.Message}");
#endif
                    }
                    finally
                    {
                        IsLoading = false;
                    }

                }
                else
                {
                    IsNoneConnection = true;
                }
            }


            IsLoading = false;
        }
        public async Task GetAccountInfo()
        {
            try
            {
                var re = await _wifiService.GetAccountBasicInfo(BJUTInfoCenterUserinfo.Username, BJUTInfoCenterUserinfo.Password);
                AccountInfo = re;
                GetOnlineAccount();
            }
            catch(InvalidUserInfoException)
            {
#if DEBUG
                Services.NotityService.Notify($"DEBUG:用户名或密码错误");
#endif
            }

        }
        public async void LoadRegistedInfo()
        {
            isAuthencated = await _wifiService.GetAuthenStatus();
            IsRegisted = await _wifiService.GetRegistStatus();
            //if (IsRegisted)
            //{
            //    var re = await _wifiService.GetAccountBasicInfo(BJUTInfoCenterUserinfo.Username, BJUTInfoCenterUserinfo.Password);
            //    AccountInfo = re;
            //}

        }
        
        public void OnNavigatedFrom(NavigationParameters parameters)
        {
            NetworkHelper.ConnectionTypeChanged -= NetworkHelper_ConnectionTypeChanged;

        }
        private bool isLoginError = false;
        public  void OnNavigatedTo(NavigationParameters parameters)
        {
            NetworkHelper.ConnectionTypeChanged -= NetworkHelper_ConnectionTypeChanged;
            NetworkHelper.ConnectionTypeChanged += NetworkHelper_ConnectionTypeChanged;

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



                    GetAccountInfo();
                }
            }
            catch (InvalidUserInfoException)
            {
                Services.NotityService.Notify("用户名或密码错误，请重新输入");

                isLoginError = true;
                await GetUserInfo();
            }
            catch (LoginTipException loginTipException)
            {
                Services.NotityService.Notify(loginTipException.Message);

                var info = await _wifiService.GetAccountBasicInfo(BJUTInfoCenterUserinfo.Username, BJUTInfoCenterUserinfo.Password);
                AccountInfo = info;

                GetOnlineAccount();
            }
            catch(Exception e)
            {
#if DEBUG
                Services.NotityService.Notify($"DEBUG Type:{e.GetType().ToString()} Msg:{e.Message}");
#else
                Services.NotityService.Notify("未知异常(⊙﹏⊙)");
#endif

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

            try
            {
                IsRegisted = await _wifiService.GetRegistStatus();
                if (IsRegisted)
                {
                    await _wifiService.Logout();
                }
                IsRegisted = false;
                
                Services.NotityService.Notify("注销登录成功");
            }
            catch
            {
                Services.NotityService.Notify("注销异常");
            }
            finally
            {
                IsLoading = false;
            }
            try
            {
                GetOnlineAccount();
            }
            catch
            {

            }
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

        public async void GetOnlineAccount()
        {
            var re = await _wifiService.GetOnlineAccountAsync();

            OnlineAccounts.Clear();
            var ip = NetworkHelper.GetIPAddress();
            foreach (var item in re)
            {
                if (item.IPv4 == ip)
                {
                    item.IsCurrentIP = true;
                }
                OnlineAccounts.Add(item);
            }
            if (OnlineAccounts.Count > 0)
            {
                CanOffline = true;
            }
            else
            {
                CanOffline = false;
            }
        }
        public async void Offline()
        {
            try
            {
                if (SelectedAccount != null)
                {
                    if (SelectedAccount.IsCurrentIP)
                    {
                        Logout();
                    }
                    else
                    {
                        var re = await _wifiService.OfflineAsync(SelectedAccount.SessionID);
                        if (re == true)
                        {
                            OnlineAccounts.Remove(SelectedAccount);
                            if (OnlineAccounts.Count <= 0)
                            {
                                CanOffline = false;
                            }
                            Services.NotityService.Notify($"{SelectedAccount.IPv4}已离线");
                        }
                        else
                        {
                            Services.NotityService.Notify($"强制离线失败");
                        }
                    }

                    SelectedAccount = null;
                }
                else
                {
                    Services.NotityService.Notify("选一个IP，让他下线！！");
                }
            }
            catch
            {
                Services.NotityService.Notify("出了点状况(⊙﹏⊙)");
            }
            
        }


        public void OnNavigatingTo(NavigationParameters parameters)
        {
            
        }
        private bool _isNoneConnection;
        public bool IsNoneConnection
        {
            get { return _isNoneConnection; }
            set { SetProperty(ref _isNoneConnection, value); }
        }

        private void NetworkHelper_ConnectionTypeChanged(object sender, EventArgs e)
        {
            Loaded();
        }
    }
}
