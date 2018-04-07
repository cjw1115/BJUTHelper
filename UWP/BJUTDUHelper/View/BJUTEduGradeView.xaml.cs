using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
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
    public sealed partial class BJUTEduGradeView : Page
    {
        private object navigationParam;
        public ViewModel.BJUTEduGradeVM BJUTEduGradeVM;
        public BJUTEduGradeView()
        {
            this.InitializeComponent();
            var locator = Application.Current.Resources["Locator"] as ViewModel.ViewModelLocator;
            BJUTEduGradeVM = locator.BJUTEduGradeVM;
            this.Loaded += BJUTEduGradeView_Loaded;


        }

        private void BJUTEduGradeView_Loaded(object sender, RoutedEventArgs e)
        {
            BJUTEduGradeVM.Loaded(navigationParam);
        }

        //public static ICommand GetLoadedCommand(DependencyObject o)
        //{
        //    return (ICommand)o.GetValue(LoadedCommandProperty);
        //}
        //public static void SetLoadedCommand(DependencyObject o, ICommand value)
        //{
        //    o.SetValue(LoadedCommandProperty, value);
        //}
        //public static readonly DependencyProperty LoadedCommandProperty = DependencyProperty.Register("LoadedCommand", typeof(ICommand), typeof(BJUTEduGradeView), new PropertyMetadata(null));

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationParam = e.Parameter;
        }
        //public static ICommand GetNavigateToCommand(DependencyObject o)
        //{
        //    return (ICommand)o.GetValue(NavigateToCommandProperty);
        //}
        //public static void SetNavigateToCommand(DependencyObject o, ICommand value)
        //{
        //    o.SetValue(NavigateToCommandProperty, value);
        //}
      
        //public static readonly DependencyProperty NavigateToCommandProperty = DependencyProperty.Register("NavigateToCommand", typeof(ICommand), typeof(BJUTEduGradeView), new PropertyMetadata(null));



        private void cbSchoolYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void cbTerm_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
        }

        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {

            var model = (sender as Panel).DataContext;
            ListViewItem selitem = lvGrade.ContainerFromItem(model) as ListViewItem;
            if (selitem.ContentTemplate == dataTemplateDefaultName)
            {
                
                selitem.ContentTemplate = dataTemplateSelectName;
            }
            else
            {
                selitem.ContentTemplate = dataTemplateDefaultName;
            }

            foreach (var item in lvGrade.Items)
            {
                if (item == model)
                    continue;
                var listViewItem = lvGrade.ContainerFromItem(item) as ListViewItem;

                if (listViewItem!=null&&listViewItem.ContentTemplate == dataTemplateSelectName)
                    listViewItem.ContentTemplate = dataTemplateDefaultName;
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


    }
    public class PassColorValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var numStr = value as string;
            int num;
            if(int.TryParse(numStr, out num)&&num<60)
            {
                return new SolidColorBrush(Colors.Red);
            }
            
            return new SolidColorBrush(Colors.Black);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
