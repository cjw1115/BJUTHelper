using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

namespace BJUTHelper
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Grade : Page
    {
        GDJWGL gd;
        public Grade()
        {
            //if ("Windows.Mobile" == Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily)
            //    Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            this.NavigationCacheMode = NavigationCacheMode.Disabled;
            this.InitializeComponent();
            
        }

        /// <summary>
        /// 在此页将要在 Frame 中显示时进行调用。
        /// </summary>
        /// <param name="e">描述如何访问此页的事件数据。
        /// 此参数通常用于配置页。</param>

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            gd = (GDJWGL)e.Parameter;
            if (gd == null)
            {
                await new MessageDialog("获取成绩错误！请返回重试").ShowAsync();
            }
            lvGrade.ItemsSource = gd.gc;
            cbSchoolYear.ItemsSource = gd.gc.schoolYearList;
            cbTerm.ItemsSource = gd.gc.termList;
        }

        //private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        //{
        //    e.Handled = true;

        //    if (Frame.CanGoBack)
        //        Frame.GoBack();
        //}

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            gd.gc.Clear();
            //HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
            if (Frame.CanGoBack)
                Frame.GoBack( );
        }

        private void cbSchoolYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if ((string)cbSchoolYear.SelectedValue == "历年成绩")
                cbTerm.SelectedIndex = -1;
            gd.gc.GetSpecificGradeChart((string)cbSchoolYear.SelectedValue, (string)cbTerm.SelectedValue);
            //lvGrade.ItemsSource = gd.gc;
        }

        private void cbTerm_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            gd.gc.GetSpecificGradeChart((string)cbSchoolYear.SelectedValue, (string)cbTerm.SelectedValue);
            //lvGrade.ItemsSource = gd.gc;
            tbkWeightScore.Text = gd.gc.GetWeightAvarageScore();
        }

        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            
            ListViewItem listViewItem = (ListViewItem)(lvGrade.ContainerFromItem((sender as Panel).DataContext));
            ContentPresenter contentPresenter = FindVisualChild<ContentPresenter>(listViewItem);

            // ListBoxItem myListBoxItem = (ListBoxItem)(listbox.ItemContainerGenerator.ContainerFromItem((sender as Panel).DataContext));

            // ContentPresenter contentPresenter = (ContentPresenter)lvGrade.ItemContainerGenerator.ContainerFromItem((sender as Panel).DataContext);

            if (contentPresenter.ContentTemplate.Equals(dataTemplateSelectName))
            {
                contentPresenter.ContentTemplate = dataTemplateDefaultName;
            }
            else
            {
                contentPresenter.ContentTemplate = dataTemplateSelectName;
            }
        }

        private childItem FindVisualChild<childItem>(DependencyObject obj) where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem)
                    return (childItem)child;
                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            OnNavigatedFrom(null);
        }

       
    }
}
