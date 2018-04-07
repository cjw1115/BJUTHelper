using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using BJUTDUHelperXamarin.Models;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Prism.Navigation;
using System.Threading.Tasks;
using BJUTDUHelperXamarin.Models.MyBJUT;
using BJUTDUHelperXamarin.Services;
using System.Windows.Input;
using BJUTDUHelperXamarin.Views.MyBJUT;

namespace BJUTDUHelperXamarin.ViewModels
{
    public class NavigationPageViewModel : BindableBase//, IConfirmNavigation, IConfirmNavigationAsync
    {
        INavigationService _navigationService;
        public NavigationPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

            LogoutCommand = new DelegateCommand(Logout);

            NavigationItems = new List<NavigationItemModel>
            {
                //new NavigationItemModel { Title="工大生活",PageType=typeof(Views.MyBjutPage),Icon=Device.OnPlatform<string>(iOS:"mybjut.png",Android:"mybjut.png",WinPhone:"img/mybjut.png")},
                new NavigationItemModel { Title="网络助手",PageType=typeof(Views.WifiHelperPage),Icon='\uf1eb'.ToString()},
                new NavigationItemModel { Title="教务中心",PageType=typeof(Views.EduPage),Icon='\uf19d'.ToString()},
                new NavigationItemModel { Title="图书馆", Icon='\uf02d'.ToString()},
                new NavigationItemModel { Title="一卡通助手",PageType=typeof(Views.CampusCardPage),Icon='\uf2c3'.ToString()},
                new NavigationItemModel { Title="个人中心",PageType=typeof(UserPage),Icon='\uf2be'.ToString()},
                 
            };
        }
        public List<NavigationItemModel> _navigationItems;
        public List<NavigationItemModel> NavigationItems
        {
            get { return _navigationItems; }
            set { SetProperty(ref _navigationItems, value); }
        }

        public Task<bool> CanNavigateAsync(NavigationParameters parameters)
        {
            return Task.FromResult(true);
        }

        public bool CanNavigate(NavigationParameters parameters)
        {
            return true;
        }

        private  BJUTHelperUserInfo _bjutHelperUserInfo;
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
        public void Load()
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
    }
}
