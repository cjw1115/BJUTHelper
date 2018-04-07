using BJUTDUHelperXamarin.Models.MyBJUT;
using BJUTDUHelperXamarin.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Windows.Input;

namespace BJUTDUHelperXamarin.ViewModels.MyBJUT
{

    public class UserPageViewModel : BindableBase, INavigatedAware
    {
        private DbService _dbService;
        private INavigationService _navigationService;
        public UserPageViewModel(INavigationService navigationService)
        {
            _dbService = new DbService();
            _navigationService = navigationService;
            LoadedCommand = new DelegateCommand(Loaded);

            TappedCommand = new DelegateCommand(Tapped);
            //ExperimentalTappedCommand = new DelegateCommand(ExperimentalTapped);

            ItemClickCommand=new DelegateCommand<object>(ItemClick);

            LoginCommand = new DelegateCommand(Login);
        }

        public DelegateCommand LoadedCommand { get; set; }
        public  void Loaded()
        {
            LoadUserinfo();

            if (null == Models.Settings.EduProxySetting)
            {
                Models.Settings.EduProxySetting = IsProxy = true;
            }
            else
            {
                IsProxy = Models.Settings.EduProxySetting.Value;
            }

            //if (null == Models.Settings.EduExperimentalSetting)
            //{
            //    Models.Settings.EduExperimentalSetting = IsExperimental = false;
            //}
            //else
            //{
            //    IsExperimental = Models.Settings.EduExperimentalSetting.Value;
            //}
        }
        public void OnNavigatedFrom(NavigationParameters parameters)
        {
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {


        }

        private bool _isProxy=true;
        public bool IsProxy
        {
            get { return _isProxy; }
            set {
                SetProperty(ref _isProxy, value);
            }
        }

        
        public DelegateCommand TappedCommand { get; set; }
        public void Tapped()
        {
            var old = Models.Settings.EduProxySetting;
            if (old != IsProxy)
            {

                Models.Settings.EduProxySetting = IsProxy;

                if (IsProxy == true)
                {
                    Services.NotityService.Notify("已开启 随时随地上教务!在家也能查成绩O(∩_∩)O");
                }
                else
                {
                    Services.NotityService.Notify("关闭成功，教务系统功能需要校园网支持");
                }
            }
        }

        //private bool _isExperimental= true;
        //public bool IsExperimental
        //{
        //    get { return _isExperimental; }
        //    set
        //    {
        //        SetProperty(ref _isExperimental, value);
        //    }
        //}
        //public DelegateCommand ExperimentalTappedCommand { get; set; }
        //public void ExperimentalTapped()
        //{
        //    var old = Models.Settings.EduExperimentalSetting;
        //    if (old != IsExperimental)
        //    {

        //        Models.Settings.EduExperimentalSetting = IsExperimental;

        //        if (IsExperimental == true)
        //        {
        //            Services.NotityService.Notify("已切换到实验学院教务系统O(∩_∩)O");
        //        }
        //        else
        //        {
        //            Services.NotityService.Notify("已切换到本校区教务系统O(∩_∩)O");
        //        }
        //    }
        //}

        public DelegateCommand<Object> ItemClickCommand
        {
            get;set;
        }
        public async void ItemClick(object param)
        {
            var page = param as string;
            switch (page)
            {
                case "编辑资料":
                    NavigateToUserinfo();
                    break;
                case "账号管理":
                    await _navigationService.NavigateAsync(typeof(Views.AccountPage).Name);
                    break;
                default:
                    break;
            }
        }
        public async void NavigateToUserinfo()
        {
            if (Services.UserService.Instance.Token == null)
            {
                Services.NotityService.Notify("请先登录");
                return;
            }
            NavigationParameters parameters = new NavigationParameters();
            parameters.Add("IsLoad", true);
            await _navigationService.NavigateAsync(typeof(Views.MyBJUT.UserEditPage).Name, parameters);
        }
        #region Userinfo Region

        private BJUTHelperUserInfo _bjutHelperUserInfo;
        public BJUTHelperUserInfo BJUTHelperUserInfo
        {
            get => _bjutHelperUserInfo;
            set => SetProperty(ref _bjutHelperUserInfo, value);
        }

        private bool _isSignedIn;
        public bool IsSignedIn
        {
            get => _isSignedIn;
            set => SetProperty(ref _isSignedIn, value);
        }
        public DelegateCommand LoginCommand { get; set; }
        public async void Login()
        {
            await _navigationService.NavigateAsync(typeof(Views.MyBJUT.LoginPage).Name);
        }
        public void LoadUserinfo()
        {
            BJUTHelperUserInfo = UserService.Instance.UserInfo;
            if (BJUTHelperUserInfo == null)
            {
                IsSignedIn = false;
            }
            else
            {
                if (string.IsNullOrEmpty(BJUTHelperUserInfo.Token))
                    IsSignedIn = false;
                else
                    IsSignedIn = true;
            }
        }
        public ICommand LogoutCommand { get; set; }
        public void Logout()
        {
            Services.UserService.Instance.Logout();
            IsSignedIn = false;
        }

        #endregion
    }
}
