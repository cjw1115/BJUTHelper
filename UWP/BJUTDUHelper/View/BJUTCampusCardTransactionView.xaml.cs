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
    public sealed partial class BJUTCampusCardTransactionView : Page
    {
        private object navigationParam;
        public ViewModel.BJUTCampusCardTransactionVM BJUTCampusCardTransactionVM { get; set; }
        public BJUTCampusCardTransactionView()
        {
            this.InitializeComponent();
            this.Loaded += BJUTCampusCardTransactionView_Loaded;
            var locator = Application.Current.Resources["Locator"] as ViewModel.ViewModelLocator;
            BJUTCampusCardTransactionVM = locator.BJUTCampusCardTransactionVM;
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationParam = e.Parameter;
        }
        private void BJUTCampusCardTransactionView_Loaded(object sender, RoutedEventArgs e)
        {
            BJUTCampusCardTransactionVM.Loaded(navigationParam);
        }
        private void Panel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var model = (sender as Panel).DataContext;
            ListViewItem selitem = listView.ContainerFromItem(model) as ListViewItem;
            if (selitem.ContentTemplate == DataTemplateNoSelected)
            {
                selitem.ContentTemplate = DataTemplateSelected;
            }
            else
            {
                selitem.ContentTemplate = DataTemplateNoSelected;
            }

            foreach (var item in listView.Items)
            {
                if (item== model)
                    continue;
                var listViewItem = listView.ContainerFromItem(item) as ListViewItem;
                if (listViewItem.ContentTemplate == DataTemplateSelected)
                    listViewItem.ContentTemplate = DataTemplateNoSelected;
            }
        }
    }
}
