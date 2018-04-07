using BJUTDUHelperXamarin.Views.MyBJUT;
using PivotPagePortable;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BJUTDUHelperXamarin.Controls
{
    public partial class PhotoPicker : ContentPage
    {
        UserEditPage _userEditPage;
        public PhotoPicker(UserEditPage userEditPage)
        {
            _userEditPage = userEditPage;
            InitializeComponent();
            
            this.btnBack.Clicked += BtnBack_Clicked;
            this.btnSubmit.Clicked += BtnSubmit_Clicked;
            this.Appearing += PhotoPicker_Appearing;
        }
        public string Path { get; set; }
        private async void BtnSubmit_Clicked(object sender, EventArgs e)
        {
            var re=await this.avartarClipper.ClipImage();

            SetClipImagePath(re);

            await this.Navigation.PopModalAsync(true);
        }
        private void PhotoPicker_Appearing(object sender, EventArgs e)
        {
            this.avartarClipper.Path = Path;
        }

        public async void BtnBack_Clicked(object sender, EventArgs e)
        {
            await this.Navigation.PopModalAsync(true);
        }
        public void SetClipImagePath(string newPath)
        {
            if (!string.IsNullOrEmpty(newPath))
            {
                _userEditPage.AvatarPath = newPath ;
            }
        }
    }
}