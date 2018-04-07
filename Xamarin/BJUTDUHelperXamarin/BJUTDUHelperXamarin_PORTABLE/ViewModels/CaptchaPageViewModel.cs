using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Navigation;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace BJUTDUHelperXamarin.ViewModels
{
    public class CaptchaPageViewModel : BindableBase,INavigationAware, IConfirmNavigation, IConfirmNavigationAsync
    {

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
            
        }
        Models.ILogin _login;
        Services.HttpBaseService _httpService;
        NavigationParameters naviParam;
        
        public  void OnNavigatedTo(NavigationParameters parameters)
        {
            naviParam = parameters;
            _httpService = (Services.HttpBaseService)parameters["httpservice"];
            _login= (Models.ILogin)parameters["ILogin"];
            GetCheckCode();
        }
        INavigationService naviService;
        public CaptchaPageViewModel(INavigationService navigationService)
        {
            naviService = navigationService;
            SubmitCommand = new DelegateCommand(Submit);
            RefreshCommand = new DelegateCommand(Refresh);
        }
        public DelegateCommand SubmitCommand { get; set; }
        public async void Submit()
        {
            if (!string.IsNullOrWhiteSpace(CaptchaText))
            {
                naviParam = new NavigationParameters();
                naviParam.Add("from", typeof(Views.CaptchaPage));
                naviParam.Add("captchatext", CaptchaText);

                //await Views.NavigationPage.CurrentNavigationPage.Navigation.PopAsync();
                var re = await naviService.GoBackAsync(naviParam);

            }
        }

        public DelegateCommand RefreshCommand { get; set; }
        public void Refresh()
        {
             GetCheckCode();
        }
        private string _captchaText;
        public string CaptchaText
        {
            get { return _captchaText; }
            set { SetProperty(ref _captchaText , value); }
        }

        private ImageSource _captchaSource;
        public ImageSource CaptchaSource
        {
            get { return _captchaSource; }
            set { SetProperty(ref _captchaSource , value); }
        }

        public async void GetCheckCode()
        {
            var re=await _login.GetCheckCode(_httpService);
            CaptchaSource = re;
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
            
        }

        public bool CanNavigate(NavigationParameters parameters)
        {
            return true;
        }

        public Task<bool> CanNavigateAsync(NavigationParameters parameters)
        {
            return Task.FromResult(true);
        }
    }
}
