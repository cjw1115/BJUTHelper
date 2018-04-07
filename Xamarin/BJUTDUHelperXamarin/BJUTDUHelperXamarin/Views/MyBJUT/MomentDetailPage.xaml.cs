using System;
using System.Globalization;
using BJUTDUHelperXamarin.Controls;
using Xamarin.Forms;

namespace BJUTDUHelperXamarin.Views.MyBJUT
{
    public partial class MomentDetailPage : ContentPage
    {
        public MomentDetailPage()
        {
            InitializeComponent();
            Entry entry = new Entry();
        }

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
    public class PlaceholderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var nickname = value as string;
            if (string.IsNullOrEmpty(nickname))
            {
                return "留下你的评论";
            }
            else
            {
                return "回复@" + nickname;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
