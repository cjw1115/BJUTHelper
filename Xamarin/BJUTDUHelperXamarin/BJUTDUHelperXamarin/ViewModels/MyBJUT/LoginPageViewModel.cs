using BJUTDUHelperXamarin.Models.MyBJUT;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace BJUTDUHelperXamarin.ViewModels.MyBJUT
{
    public class LoginPageViewModel : BindableBase, INavigatedAware
    {
        public void OnNavigatedFrom(NavigationParameters parameters)
        {

        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {

        }
        private Services.UserService _userService = Services.UserService.Instance;
        private Services.HttpBaseService _httpService;
        private INavigationService _navigationService;
        public LoginPageViewModel(INavigationService navigationService)
        {
            LoginCommand = new DelegateCommand(Login);
            RegistCommand = new DelegateCommand(Regist);
            RetrieveCommand = new DelegateCommand(Retrieve);

            _httpService = new Services.HttpBaseService();
            _navigationService = navigationService;

            InitCollege();
            InitGender();
        }



        public void Load()
        {
        }

#region Login Region
        public ICommand LoginCommand { get; set; }
        public async void Login()
        {
            if (string.IsNullOrWhiteSpace(LoginModel.Username))
            {
                Services.NotityService.Notify("用户名不能空");
                return;
            }
            if (string.IsNullOrWhiteSpace(LoginModel.Password))
            {
                Services.NotityService.Notify("密码不能空");
                return;
            }

            IsLoading = true;
            try
            {


                var user = await _userService.LoginAsync(_httpService, LoginModel);
                #region DEBUG
                System.Diagnostics.Debug.WriteLine($"Token:{user.Token}");
                #endregion
                Services.UserService.Instance.SetLocalUserinfo(user);
                Services.NotityService.Notify("登录成功");
                await _navigationService.GoBackAsync();
            }

            catch (LoginTipException tip)
            {
                Services.NotityService.Notify(tip.Message);
            }
            catch
            {
                Services.NotityService.Notify("遇到未知错误");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private LoginModel _loginModel = new LoginModel();
        public LoginModel LoginModel
        {
            get => _loginModel;
            set => SetProperty(ref _loginModel, value);
        }

#endregion

#region Regist Region
        public ICommand RegistCommand { get; set; }
        public async void Regist()
        {
            if (string.IsNullOrWhiteSpace(RegistModel.Username))
            {
                Services.NotityService.Notify("用户名不能空");
                return;
            }
            if (string.IsNullOrWhiteSpace(RegistModel.Password))
            {
                Services.NotityService.Notify("密码不能空");
                return;
            }
            if (string.IsNullOrWhiteSpace(RegistModel.PasswordConfirm))
            {
                Services.NotityService.Notify("验证密码不能空");
                return;
            }
            if (RegistModel.Password != RegistModel.PasswordConfirm)
            {
                Services.NotityService.Notify("两次密码不一样");
                return;
            }
            IsLoading = true;
            try
            {
                await _userService.RegistAsync(_httpService, RegistModel);

                Services.NotityService.Notify("注册成功");

                Views.MyBJUT.LoginPage.Instance.SetPageType(Views.MyBJUT.LoginPage.PageType.登录);
            }

            catch (LoginTipException tip)
            {
                Services.NotityService.Notify(tip.Message);
            }
            catch
            {
                Services.NotityService.Notify("遇到未知错误");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private RegistModel _registModel = new RegistModel();
        public  RegistModel RegistModel
        {
            get => _registModel;
            set => SetProperty(ref _registModel, value);
        }


        #endregion

        #region Retrieview Region
        public ICommand RetrieveCommand { get; set; }
        public async void Retrieve()
        {
            if (string.IsNullOrWhiteSpace(RetrieveModel.Username))
            {
                Services.NotityService.Notify("用户名不能空");
                return;
            }
            if (string.IsNullOrWhiteSpace(RetrieveModel.Password))
            {
                Services.NotityService.Notify("密码不能空");
                return;
            }
            if (string.IsNullOrWhiteSpace(RetrieveModel.VarifyPassword))
            {
                Services.NotityService.Notify("教务系统密码");
                return;
            }
            IsLoading = true;
            try
            {
                var user=await _userService.RetrieveAsync(_httpService, RetrieveModel);

                Services.UserService.Instance.SetLocalUserinfo(user);
                Services.NotityService.Notify("重置密码成功");

                await _navigationService.GoBackAsync();
            }

            catch (LoginTipException tip)
            {
                Services.NotityService.Notify(tip.Message);
            }
            catch
            {
                Services.NotityService.Notify("遇到未知错误");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private RetrieveModel _retrieveModel = new RetrieveModel();
        public RetrieveModel RetrieveModel
        {
            get => _retrieveModel;
            set => SetProperty(ref _retrieveModel, value);
        }


        #endregion

        private string[] _collegeList;
        public string[] CollegeList
        {
            get => _collegeList;
            set => SetProperty(ref _collegeList, value);
        }
        public void InitCollege()
        {
            var names=Enum.GetNames(typeof(BJUTCollege));
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
        private bool _isLoading = false;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { SetProperty(ref _isLoading, value); }
        }
    }
}
