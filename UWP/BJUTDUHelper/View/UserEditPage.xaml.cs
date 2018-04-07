using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace BJUTDUHelper.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class UserEditPage : Page
    {
        public ViewModel.UserEditVM UserEditVM { get; set; }
        public UserEditPage()
        {
            var locator = Application.Current.Resources["Locator"] as ViewModel.ViewModelLocator;
            UserEditVM = locator.UserEditVM;
            this.InitializeComponent();
            this.Loaded += UserEditPage_Loaded;
            
            
        }

        private UserEditNaviParam naviParam;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            naviParam = (UserEditNaviParam)e.Parameter;
        }
        private void UserEditPage_Loaded(object sender, RoutedEventArgs e)
        {
            UserEditVM.Loaded(naviParam);
        }
    }
    public class UserEditNaviParam
    {
        public string UserType { get; set; }
        public Model.UserBase User{get;set;}

        public ObservableCollection<Model.BJUTInfoCenterUserinfo> BJUTInfoCenterUserinfos { get; set; }
        public ObservableCollection<Model.BJUTEduCenterUserinfo> BJUTEduCenterUserinfos { get; set; }
        public ObservableCollection<Model.BJUTLibCenterUserinfo> BJUTLibCenterUserinfos { get; set; }
    }
}
