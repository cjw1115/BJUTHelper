using BJUTDUHelperXamarin.Controls;
using BJUTDUHelperXamarin.Models.MyBJUT;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Globalization;
using System.Windows.Input;
using System.Collections;

namespace BJUTDUHelperXamarin.Views.MyBJUT
{
    public partial class MomentsViewPage : ContentPage
    {
        public MomentsViewPage()
        {
            InitializeComponent();
        }
       

        //private void LikeButton_Clicked(object sender, object e)
        //{
        //    MomentItem item = e as MomentItem;
        //    if (item.IsLike)
        //    {
        //        item.IsLike = false;
        //        item.LikeTimes--;
        //    }
        //    else
        //    {
        //        item.IsLike = true;
        //        item.LikeTimes++;
        //    }
        //}

        private void UniformImagePanel_ItemClicked(object sender, UniformImagePaneItemClickedEventArgs e)
        {
            var iamgePanel = sender as UniformImagePanel;
            PhotoViewer photoViewer = new PhotoViewer();
            photoViewer.Photos = iamgePanel.ItemsSource;
            photoViewer.SelectedIndex = e.Index;
            if (photoViewer.Photos == null || photoViewer.Photos.Count <= 0)
            {
                return;
            }
            App.Current.MainPage.Navigation.PushModalAsync(photoViewer);

        }
    }
    public class MomentsListView : ListView
    {
        public static readonly BindableProperty LoadMoreCommandProperty = BindableProperty.Create("LoadMoreCommand", typeof(ICommand), typeof(MomentsListView), defaultValue: null);
        public ICommand LoadMoreCommand
        {
            get => this.GetValue(LoadMoreCommandProperty) as ICommand;
            set => SetValue(LoadMoreCommandProperty, value);
        }
        public static readonly BindableProperty IsLoadingMoreProperty = BindableProperty.Create("IsLoadingMore", typeof(bool), typeof(MomentsListView), defaultValue: false,defaultBindingMode: BindingMode.TwoWay,propertyChanged: IsLoadingMoreChangedCallback);
        public bool IsLoadingMore
        {
            get => (bool)this.GetValue(IsLoadingMoreProperty);
            set => SetValue(IsLoadingMoreProperty, value);
        }
        public static void IsLoadingMoreChangedCallback(BindableObject o,object oldValue,object newValue)
        {
            var listView = o as MomentsListView;
            listView.OnIsLoadingPropertyChanged();
        }

        public static readonly BindableProperty CanLoadMoreProperty = BindableProperty.Create("CanLoadMore", typeof(bool), typeof(MomentsListView), defaultValue: true, defaultBindingMode: BindingMode.TwoWay, propertyChanged: CanLoadMoreChangedCallback);
        public bool CanLoadMore
        {
            get => (bool)this.GetValue(CanLoadMoreProperty);
            set => SetValue(CanLoadMoreProperty, value);
        }
        public static void CanLoadMoreChangedCallback(BindableObject o, object oldValue, object newValue)
        {
            var list = o as MomentsListView;
            if (!list.CanLoadMore)
            {
                list.Footer = null;
            }
        }

        ProgressRing _indicatorLoadingMore;
        Button _btnLoadMore;
        private void InitLoadMoreControls()
        {
            if (_btnLoadMore == null)
            {
                _btnLoadMore = new Button();
                _btnLoadMore.Text = "更多";
                _btnLoadMore.HorizontalOptions = new LayoutOptions(LayoutAlignment.Fill,false);
                _btnLoadMore.Clicked += _btnLoadMore_Clicked;
            }
            if (_indicatorLoadingMore == null)
            {
                _indicatorLoadingMore = new ProgressRing();
                _indicatorLoadingMore.IsRunning = true;
                _indicatorLoadingMore.Color = (Color)App.Current.Resources["BJUTDUHelperMainBackground"];
            }
        }

        private void _btnLoadMore_Clicked(object sender, EventArgs e)
        {
            InvokeLoadMore();
        }
        private void InvokeLoadMore()
        {
            if (CanLoadMore == false)
            {
                return;
            }
            if (!IsLoadingMore)
            {
                IsLoadingMore = true;
                LoadMoreCommand?.Execute(null);

            }
        }

        public void OnIsLoadingPropertyChanged()
        {
            if (CanLoadMore == false)
            {
                this.Footer = null;
                return;
            }
            if (IsLoadingMore)
            {
                this.Footer = _indicatorLoadingMore;
            }
            else
            {
                this.Footer = _btnLoadMore ;
            }
        }

        public MomentsListView()
        {
            InitLoadMoreControls();
            this.ItemAppearing += MomentsListView_ItemAppearing;
            this.ItemTapped += MomentsListView_ItemTapped;
        }

        private void MomentsListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            TappedCommand?.Execute(e.Item);
        }

        private void MomentsListView_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            var list = this.ItemsSource as IList;
            var itemIndex=list.IndexOf(e.Item);
            if (itemIndex == list.Count-1)
            {
                InvokeLoadMore();
            }
        }

        public static readonly BindableProperty TappedCommandProperty = BindableProperty.Create("TappedCommand", typeof(ICommand), typeof(MomentsListView), defaultValue: null, defaultBindingMode: BindingMode.TwoWay, propertyChanged: CanLoadMoreChangedCallback);
        public ICommand TappedCommand
        {
            get => (ICommand)this.GetValue(TappedCommandProperty);
            set => SetValue(TappedCommandProperty, value);
        }
    }
    public class DateToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dateTime = (DateTime)value;
            return dateTime.ToString("yyyy-MM-dd hh:mm");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class NicknameConverter : IValueConverter
    {
        public bool HasSemicolon { get; set; } = false;
        public bool HasAt { get; set; } = false;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var nickname = value as string;
                if (!string.IsNullOrWhiteSpace(nickname))
                {

                    if (HasSemicolon)
                    {
                        nickname += ":";
                    }
                    if (HasAt)
                    {
                        nickname = "@" + nickname;
                    }
                    return nickname;

                }
                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
