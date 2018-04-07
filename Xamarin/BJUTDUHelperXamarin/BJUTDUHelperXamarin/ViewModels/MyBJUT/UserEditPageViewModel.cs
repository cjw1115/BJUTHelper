using BJUTDUHelperXamarin.Models.MyBJUT;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BJUTDUHelperXamarin.ViewModels.MyBJUT
{
	public class UserEditPageViewModel : BindableBase,INavigatedAware
	{
        private BJUTHelperUserInfo _userinfo;
        private INavigationService _navigationService;
        private Services.HttpBaseService _httpService = new Services.HttpBaseService();
        public BJUTHelperUserInfo Userinfo
        {
            get => _userinfo;
            set => SetProperty(ref _userinfo, value);
        }

        private string _avatarPath;

        public string AvatarPath
        {
            get { return _avatarPath; }
            set { SetProperty(ref _avatarPath , value); }
        }

        private bool _isLoading = false;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }
        public UserEditPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            SubmitCommand = new DelegateCommand(Submit);

            InitGender();
            InitCollege();
        }
        public void OnNavigatedFrom(NavigationParameters parameters)
        {

        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            var re = parameters.Keys.Contains("IsLoad");
            if (re == true)
            {
                var value = (bool)parameters["IsLoad"];
                if (value == true)
                {
                    LoadUserinfo();
                }
            }
            
        }
        public DelegateCommand SubmitCommand { get; set; }
        public async void Submit()
        {

            if (string.IsNullOrWhiteSpace(Userinfo.NickName))
            {
                Services.NotityService.Notify("昵称不能为空");
                return;
            }
            try
            {
                IsLoading = true;

                var re=await Services.UserService.Instance.EditUserinfoAsync(_httpService, Userinfo,AvatarPath);
                re.Token = Services.UserService.Instance.GetOriginToken();
                Services.UserService.Instance.SetLocalUserinfo(re);

                Services.NotityService.Notify("修改成功");

                await _navigationService.GoBackAsync();
            }
            catch(LoginTipException tip)
            {
                Services.NotityService.Notify(tip.Message);
            }
            catch(InvalidUserInfoException)
            {
                Services.NotityService.Notify("登录状态失效，请重新登录");
            }
            catch(Exception e)
            {
                Services.NotityService.Notify("未知异常");
            }
            finally
            {
                IsLoading = false;
            }
        }
        public void LoadUserinfo()
        {
            Userinfo = Services.UserService.Instance.LoadLocalUserinfo();
        }


        private string[] _collegeList;
        public string[] CollegeList
        {
            get => _collegeList;
            set => SetProperty(ref _collegeList, value);
        }
        public void InitCollege()
        {
            var names = Enum.GetNames(typeof(BJUTCollege));
            CollegeList = names;
        }

        private string[] _genderList;
        public string[] GenderList
        {
            get => _genderList;
            set => SetProperty(ref _genderList, value);
        }
        public void InitGender()
        {
            var names = Enum.GetNames(typeof(Gender));
            GenderList = names;
        }

       
    }
}
