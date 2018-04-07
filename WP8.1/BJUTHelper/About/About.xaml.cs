using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace BJUTHelper.About
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class About : Page
    {
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (Frame.CanGoBack)
                Frame.GoBack();
        }
        //private void HardwareButtons_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        //{
        //    e.Handled = true;
        //    if(Frame.CanGoBack)
        //    {
        //        Frame.GoBack();
        //    }
        //}

        public About()
        {
            //if ("Windows.Mobile" == Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily)
            //    Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            this.InitializeComponent();
            
            storyBoard.Begin();
            Task.Run (async () => 
            {
                await this.Dispatcher.RunAsync( Windows.UI.Core.CoreDispatcherPriority.Normal,() => { BottomAppBar.Focus(FocusState.Programmatic); });
            });
           
        }

       
        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            OnNavigatedTo(null);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            storyBoardText.Begin();
        }
    }
}
