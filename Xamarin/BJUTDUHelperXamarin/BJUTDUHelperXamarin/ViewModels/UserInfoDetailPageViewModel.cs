using BJUTDUHelperXamarin.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BJUTDUHelperXamarin.ViewModels
{
    public class UserInfoDetailPageViewModel : BindableBase,INavigationAware
    {
        INavigationService _navigationService;
        DbService _debService;
        public UserInfoDetailPageViewModel(INavigationService navigationService)
        {
            _debService = new DbService();
            _navigationService = navigationService;

            SaveCommand = new DelegateCommand(Save);
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {

        }

        private object userModel = null;
        public void OnNavigatedTo(NavigationParameters parameters)
        {
            var title = (string)parameters["title"];
            Title = title;
            userModel= parameters["model"];
            switch (title)
            {
                case "信息门户":
                   var infouser=(Models.BJUTInfoCenterUserinfo)parameters["model"];
                    Username = infouser?.Username;
                    Password = infouser?.Password;
                    break;
                case "教务管理":
                   var eduuser = (Models.BJUTEduCenterUserinfo)parameters["model"];
                    Username = eduuser?.Username;
                    Password = eduuser?.Password;
                    break;
                case "图书馆":
                    var libuser = (Models.BJUTLibCenterUserinfo)parameters["model"];
                    Username = libuser?.Username;
                    Password = libuser?.Password;
                    break;
                default:
                    break;
            }
        }

        private string _username;
        public string Username
        {
            get { return _username; }
            set { SetProperty(ref _username, value); }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }
        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title ,value); }
        }


        public DelegateCommand SaveCommand { get; set; }
        public void Save()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password))
                {
                    switch (Title)
                    {
                        case "信息门户":
                            _debService.DeleteAll<Models.BJUTInfoCenterUserinfo>();

                            var infouser = new Models.BJUTInfoCenterUserinfo { Username = Username, Password = Password };
                            _debService.Insert(infouser);
                            break;
                        case "教务管理":
                            _debService.DeleteAll<Models.BJUTEduCenterUserinfo>();
                            var eduuser = new Models.BJUTEduCenterUserinfo { Username = Username, Password = Password };
                            _debService.Insert(eduuser);

                            //if (Services.EduService.IsExperimental(eduuser.Username))
                            //{
                            //    Models.Settings.EduExperimentalSetting = true;
                            //}
                            //else
                            //{
                            //    Models.Settings.EduExperimentalSetting = false;
                            //}
                            break;
                        case "图书馆":
                            _debService.DeleteAll<Models.BJUTLibCenterUserinfo>();
                            var libuser = new Models.BJUTLibCenterUserinfo { Username = Username, Password = Password };
                            _debService.Insert(libuser);
                            break;
                        default:
                            break;
                    }
                }
                Services.NotityService.Notify("保存成功");

                NavigationParameters naviParam = new NavigationParameters();
                naviParam.Add("from", typeof(Views.UserInfoDetailPage));
                _navigationService.GoBackAsync(naviParam);
               
            }
            catch(Exception e)
            {
                Services.NotityService.Notify("保存失败");
            }
            
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
            
        }
    }
}
