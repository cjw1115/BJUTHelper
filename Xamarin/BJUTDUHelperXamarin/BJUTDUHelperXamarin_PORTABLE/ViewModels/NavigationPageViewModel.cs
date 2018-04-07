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

namespace BJUTDUHelperXamarin.ViewModels
{
    public class NavigationPageViewModel : BindableBase//, IConfirmNavigation, IConfirmNavigationAsync
    {
        public NavigationPageViewModel()
        {
            
            NavigationItems = new List<NavigationItemModel>
            {
                //new NavigationItemModel { Title="工大生活",PageType=typeof(Views.MyBjutPage),Icon=Device.OnPlatform<string>(iOS:"mybjut.png",Android:"mybjut.png",WinPhone:"img/mybjut.png")},
                new NavigationItemModel { Title="网络助手",PageType=typeof(Views.WifiHelperPage),Icon='\uf1eb'.ToString()},
                new NavigationItemModel { Title="教务中心",PageType=typeof(Views.EduPage),Icon='\uf19d'.ToString()},
                new NavigationItemModel { Title="图书馆", Icon='\uf02d'.ToString()},
                new NavigationItemModel { Title="一卡通助手",PageType=typeof(Views.CampusCardPage),Icon='\uf2c3'.ToString()},
                new NavigationItemModel { Title="个人中心",PageType=typeof(Views.UserPage),Icon='\uf2be'.ToString()},
                 
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
    }
}
