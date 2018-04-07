using BJUTDUHelperXamarin.Models;
using BJUTDUHelperXamarin.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;


namespace BJUTDUHelperXamarin.ViewModels
{
   
    public class UserPageViewModel : BindableBase, INavigationAware
    {
        private DbService _dbService;
        private INavigationService _navigationService;
        public UserPageViewModel(INavigationService navigationService)
        {
            _dbService = new DbService();
            _navigationService = navigationService;

            ItemClickCommand = new DelegateCommand<string>(ItemClick);
            LoadedCommand = new DelegateCommand(Loaded);

            TappedCommand = new DelegateCommand(Tapped);
            ExperimentalTappedCommand = new DelegateCommand(ExperimentalTapped);


        }

        public DelegateCommand LoadedCommand { get; set; }
        public  void Loaded()
        {
            var infouser = _dbService.GetAll<BJUTInfoCenterUserinfo>().FirstOrDefault();
            if (infouser != null)
            {
                var user = new BJUTInfoCenterUserinfo();
                user.InfoCenterAccountInfo = infouser.InfoCenterAccountInfo;
                user.Username = infouser.Username;
                user.Password = infouser.Password;

                InfoUser = user;
            }

            var eduuser = _dbService.GetAll<BJUTEduCenterUserinfo>().FirstOrDefault();
            if (eduuser != null)
            {
               var user= new BJUTEduCenterUserinfo();
                user.EduSystemType = eduuser.EduSystemType;
                user.Username = eduuser.Username;
                user.Password = eduuser.Password;

                EduUser = user;
            }

            var ibuser = _dbService.GetAll<BJUTLibCenterUserinfo>().FirstOrDefault();
            if (ibuser != null)
            {
               var user = new BJUTLibCenterUserinfo();
                user.Username = ibuser.Username;
                user.Password = ibuser.Password;

                LibUser = user;
            }

            if (null == Models.Settings.EduProxySetting)
            {
                Models.Settings.EduProxySetting = IsProxy = true;
            }
            else
            {
                IsProxy = Models.Settings.EduProxySetting.Value;
            }

            if (null == Models.Settings.EduExperimentalSetting)
            {
                Models.Settings.EduExperimentalSetting = IsExperimental = false;
            }
            else
            {
                IsExperimental = Models.Settings.EduExperimentalSetting.Value;
            }


        }
        public void OnNavigatedFrom(NavigationParameters parameters)
        {
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            

        }

        private BJUTInfoCenterUserinfo _infoUser = new BJUTInfoCenterUserinfo();
        public BJUTInfoCenterUserinfo InfoUser
        {
            get { return _infoUser; }
            set { SetProperty(ref _infoUser, value); }
        }
        private BJUTEduCenterUserinfo _eduUser = new BJUTEduCenterUserinfo();
        public BJUTEduCenterUserinfo EduUser
        {
            get { return _eduUser; }
            set { SetProperty(ref _eduUser, value); }
        }
        private BJUTLibCenterUserinfo _libUser = new BJUTLibCenterUserinfo();
        public BJUTLibCenterUserinfo LibUser
        {
            get { return _libUser; }
            set { SetProperty(ref _libUser, value); }
        }

        public DelegateCommand<string> ItemClickCommand { get; set; }
        public void ItemClick(string param)
        {
            string title = "";
            object model = null;
            switch (param)
            {
                case "信息门户":
                    title = "信息门户";
                    model = InfoUser;
                    break;
                case "教务管理":
                    title = "教务管理";
                    model = EduUser;
                    break;
                case "图书馆":
                    title = "图书馆";
                    model = LibUser;
                    break;
                default:
                    break;
            }

            NavigationParameters naviParam = new NavigationParameters();
            naviParam.Add("title", title);
            naviParam.Add("model", model);
            _navigationService.NavigateAsync("UserInfoDetailPage",naviParam);
        }


        private bool _isProxy=true;
        public bool IsProxy
        {
            get { return _isProxy; }
            set {
                SetProperty(ref _isProxy, value);
            }
        }

        private bool _isExperimental= true;
        public bool IsExperimental
        {
            get { return _isExperimental; }
            set
            {
                SetProperty(ref _isExperimental, value);
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

        public DelegateCommand ExperimentalTappedCommand { get; set; }
        public void ExperimentalTapped()
        {
            var old = Models.Settings.EduExperimentalSetting;
            if (old != IsExperimental)
            {

                Models.Settings.EduExperimentalSetting = IsExperimental;

                if (IsExperimental == true)
                {
                    Services.NotityService.Notify("已切换到实验学院教务系统O(∩_∩)O");
                }
                else
                {
                    Services.NotityService.Notify("已切换到本校区教务系统O(∩_∩)O");
                }
            }
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
            
        }
    }
}
