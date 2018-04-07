using Xamarin.Forms;

namespace BJUTDUHelperXamarin.Views.MyBJUT
{
    public partial class MomentPublishPage : ContentPage
    {
        public static MomentPublishPage Instance;
        public MomentPublishPage()
        {
            Instance = this;
            InitializeComponent();

            this.btnAdd.Clicked += BtnAddImage_Clicked;
            this.btnDel.Clicked += BtnDel_Clicked; ;

            this.tbContent.Focused += MomentPublishPage_Focused;
            this.tbContent.Unfocused += MomentPublishPage_Unfocused;
        }

        private void MomentPublishPage_Unfocused(object sender, FocusEventArgs e)
        {
            if (string.IsNullOrEmpty(this.tbContent.Text))
            {
                this.labelPlaceholder.IsVisible = true;
            }
        }

        private void MomentPublishPage_Focused(object sender, FocusEventArgs e)
        {
            this.labelPlaceholder.IsVisible = false;
        }

        public void ShowMsg(string msg)
        {
            DisplayAlert("Congratulations", msg, "OK");
        }
        private void BtnDel_Clicked(object sender, System.EventArgs e)
        {
            if (this.panelImages.Children.Count <= 2)
            {
                return;
            }
            var index = panelImages.Children.Count - 3;
            panelImages.Children.RemoveAt(index);
            ViewModels.MyBJUT.MomentPublishPageViewModel.Images.RemoveAt(index);
        }

        private async void BtnAddImage_Clicked(object sender, System.EventArgs e)
        {
            if (this.panelImages.Children.Count >= 5)
            {
                return;
            }

            var re=await Plugin.Media.CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions() { CompressionQuality=92, PhotoSize= Plugin.Media.Abstractions.PhotoSize.Medium });
            if (re != null && !string.IsNullOrEmpty(re.Path))
            {
                var image = new Image();
                image.Source = FileImageSource.FromFile(re.Path);
                image.Aspect = Aspect.AspectFill;
                image.WidthRequest = 50;
                image.HeightRequest = 50;
                panelImages.Children.Insert(panelImages.Children.Count - 2, image);


                ViewModels.MyBJUT.MomentPublishPageViewModel.Images.Add(re);
            }
           
        }
    }
}
