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

    public class AccountPageViewModel : BindableBase, INavigatedAware
    {
        private DbService _dbService;
        private INavigationService _navigationService;
        public AccountPageViewModel(INavigationService navigationService)
        {
            _dbService = new DbService();
            _navigationService = navigationService;

            ItemClickCommand = new DelegateCommand<string>(ItemClick);
            LoadedCommand = new DelegateCommand(Loaded);
        }

        public DelegateCommand LoadedCommand { get; set; }
        public void Loaded()
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
                var user = new BJUTEduCenterUserinfo();
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
            _navigationService.NavigateAsync("UserInfoDetailPage", naviParam);
        }
    }
}
