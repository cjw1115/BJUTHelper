using Xamarin.Forms;

namespace BJUTDUHelperXamarin.Views.MyBJUT
{
    public partial class UserEditPage : ContentPage
    {
        TapGestureRecognizer _tapGesture = new TapGestureRecognizer();
        public UserEditPage()
        {
            InitializeComponent();

            _tapGesture.Tapped += _tapGesture_Tapped;
            this.imgAvatar.GestureRecognizers.Add(_tapGesture);
        }

        private void _tapGesture_Tapped(object sender, System.EventArgs e)
        {
            BtnAddImage_Clicked();
        }

        public static readonly BindableProperty AvatarPathProperty = BindableProperty.Create("AvatarPath", typeof(string), typeof(UserEditPage), null,propertyChanged: AvatarPathPropertyChanged);
        public string AvatarPath
        {
            get => GetValue(AvatarPathProperty) as string;
            set => SetValue(AvatarPathProperty, value);
        }
        public static void AvatarPathPropertyChanged(BindableObject o, object oldValue, object newValue)
        {
            var editPage = o as UserEditPage;
            if (!string.IsNullOrEmpty(editPage.AvatarPath))
            {
                editPage.imgAvatar.Source = FileImageSource.FromFile(editPage.AvatarPath);
            }
        }
        private Controls.PhotoPicker _photoPicker;
        private async void BtnAddImage_Clicked()
        {
            _photoPicker = new Controls.PhotoPicker(this);
            var re = await Plugin.Media.CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions() { CompressionQuality = 92, PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium });
            if (re != null && !string.IsNullOrEmpty(re.Path))
            {
                _photoPicker.Path = re.Path;
                this.AvatarPath = string.Empty;
                await this.Navigation.PushModalAsync(_photoPicker,false);
                
            }

        }
    }
}
