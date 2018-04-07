using System;
using System.Collections.Generic;
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
    public sealed partial class BJUTEduCenterView : Page
    {
        public ViewModel.BJUTEduCenterVM BJUTEduCenterVM { get; set; } 
        public BJUTEduCenterView()
        {
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
            this.InitializeComponent();
            this.Loaded += BJUTEduCenterView_Loaded;

            var locator = Application.Current.Resources["Locator"] as ViewModel.ViewModelLocator;
            BJUTEduCenterVM = locator.BJUTEduCenterVM;
            
        }

        private void BJUTEduCenterView_Loaded(object sender, RoutedEventArgs e)
        {
            BJUTEduCenterVM.Loaded();
        }
    }

    public class EduCenterViewParam
    {
        public Service.HttpBaseService HttpService { get; set; }
        public Model.BJUTEduCenterUserinfo BJUTEduCenterUserinfo { get; set; }
        public object Other { get; set; }
    }
}
