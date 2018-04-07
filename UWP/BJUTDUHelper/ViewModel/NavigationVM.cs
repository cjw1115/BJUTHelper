using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BJUTDUHelper.Model;
using Windows.UI.Xaml.Controls;

namespace BJUTDUHelper.ViewModel
{
    public class NavigationVM:ViewModelBase
    {
        private readonly string messageToken = "1";
        public List<NavigationListItem> NavigationList { get; set; } = new List<NavigationListItem>
        {
            new NavigationListItem { Title="网络助手",PageType=typeof(View.WIFIHelperView),Icon="\xE701"},
            new NavigationListItem { Title="教务中心",PageType=typeof(View.BJUTEduCenterView),Icon="\xE7BE" },
            new NavigationListItem { Title="图书馆",Icon="\xE8F1" },
            new NavigationListItem { Title="一卡通助手",PageType=typeof(View.BJUTCampusCardView),Icon="\xE8F9"},
            new NavigationListItem { Title="个人中心",PageType=typeof(View.UserManagerView),Icon="\xE77B" }
        };

        public static Frame FuncFrame { get; set; }
        public static Frame DetailFrame { get; set; }

        private string _mainTitle;
        public string MainTitle
        {
            get { return _mainTitle; }
            set { Set(ref _mainTitle, value); }
        }
        
        public void NavigationItemClick(object sender,ItemClickEventArgs e)
        {
            Model.NavigationListItem item = e.ClickedItem as Model.NavigationListItem;
            if (item != null && item.PageType != null)
            {
                MainTitle = item.Title;
                FuncFrame.Navigate(item.PageType);
            }
            else
            {
                GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("这只是个饼，而且还没画完O(∩_∩)O", messageToken);
            }
        }
    }
}
