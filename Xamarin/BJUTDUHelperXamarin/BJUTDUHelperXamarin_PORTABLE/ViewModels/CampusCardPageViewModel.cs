using BJUTDUHelperXamarin.Models;
using BJUTDUHelperXamarin.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BJUTDUHelperXamarin.ViewModels
{
    public class CampusCardPageViewModel : BindableBase
    {
        private INavigationService _navigationService;
        private Services.HttpBaseService _httpService;
        private Services.DbService _dbService;
        private Services.CampusCardService _campusCardSercvice;

        private Models.BJUTInfoCenterUserinfo _infoCenterUserInfo;
        public Models.BJUTInfoCenterUserinfo InfoCenterUserInfo
        {
            get { return _infoCenterUserInfo; }
            set { SetProperty(ref _infoCenterUserInfo, value); }
        }

        private CampusCardInfoModel _campusCardInfo;
        public CampusCardInfoModel CampusCardInfo
        {
            get { return _campusCardInfo; }
            set { SetProperty(ref _campusCardInfo, value); }
        }

        private bool _isLoading;

        public bool IsLoading
        {
            get { return _isLoading; }
            set { SetProperty(ref _isLoading , value); }
        }

        public CampusCardPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            _httpService = new HttpBaseService();
            //_dbService = new Services.DbService();
            _dbService = new DbService();
            _campusCardSercvice = new CampusCardService();

            LoadedCommand = new DelegateCommand(Loaded);
            ItemClickedCommand = new DelegateCommand<object>(ItemClicked);
            LostCommand = new DelegateCommand(LostCampusCard);
            FoundCommand = new DelegateCommand(FoundCampusCard);
        }

        private bool _isLogined = false;
        public DelegateCommand LoadedCommand { get; set; }
        public async void Loaded()
        {
            if (_isLogined == true)
                return;
            await Services.NotityService.DisplayAlert("工大助手", "此功能目前需要连接校园网使用（iOS暂时无法使用）");

            IsLoading = true;

            var user = _dbService.GetAll<Models.BJUTInfoCenterUserinfo>().FirstOrDefault();
            if (user == null)
            {
                return;
            }

            InfoCenterUserInfo = user;
            LoadLocalCampascadInfo();

            IsLoading = true;
            try
            {
                var re = await LoginInfoCenter();
                if (re == true)
                {
                    _isLogined = true;
                    await GetTransactionInfo();
                }
            }
            catch
            {

            }
            finally
            {
                IsLoading = false;

            }
            
            
        }

        
        public  void SaveCampascadInfo()
        {
            _dbService.DeleteAll<Models.CampusCardInfoModel>();
            _dbService.Insert<Models.CampusCardInfoModel>(CampusCardInfo);

            //await SaveImageToFile("Campuscard" + BJUTInfoCenterUserinfo.Username + "Image", CampusCardInfoModel.PersonImage, ApplicationData.Current.LocalFolder);
        }

        public  void LoadLocalCampascadInfo()
        {
            if (InfoCenterUserInfo == null)
            {
                return;
            }
            var model = _dbService.GetAll<Models.CampusCardInfoModel>().FirstOrDefault();

            CampusCardInfo = model;
        }

        public async Task<bool> LoginInfoCenter()
        {
            try
            {
                if (InfoCenterUserInfo == null)
                {
                    throw new NullRefUserinfoException("请输入用户名和密码");
                }
                await _campusCardSercvice.LoginInfoCenter(_httpService, InfoCenterUserInfo.Username, InfoCenterUserInfo.Password);

                //ProgressMessage = "获取信息中···";
                await _campusCardSercvice.GetCardCenterClient(_httpService);
                //获取一卡通个人信息
                var re = await _campusCardSercvice.GetCampusCardBasicInfo(_httpService);
                CampusCardInfo = re;
                SaveCampascadInfo();
                //获取一卡通个人信息
                LoadCampusCardSalaryInfo();

                return true;

            }
            catch (NullReferenceException)
            {
                
                //GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("请输入用户名和密码", messageToken);
               
            }
            catch (InvalidUserInfoException userinfoException)
            {
                Services.NotityService.Notify("用户名或密码无效");
                //GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("", messageToken);

            }
            catch (HttpRequestException)
            {
                Services.NotityService.Notify("网络连接走丢了/(ㄒoㄒ)/~~");
                //GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("请检查网络连接", messageToken);
            }
            catch
            {
                Services.NotityService.Notify("未知异常/(ㄒoㄒ)/~~");
                //GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("其他错误", messageToken);
            }
            return false;
        }
        public async void LoadCampusCardSalaryInfo()
        {
            try
            {
                if (CampusCardInfo == null)
                {
                    CampusCardInfo = new Models.CampusCardInfoModel();
                }
                var info = await _campusCardSercvice.GetCampusCardSalaryInfo(_httpService);

                CampusCardInfo.balance = info.balance;
                CampusCardInfo.smtAccounts = info.smtAccounts;
                CampusCardInfo.smtEndcodeTxt = info.smtEndcodeTxt;
                CampusCardInfo.smtCardid = info.smtCardid;
            }
            catch (HttpRequestException requestException)
            {
                Services.NotityService.Notify("请检查网络连接");
                //GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("请检查网络连接", messageToken);
            }
            catch (System.Net.WebException)
            {
                Services.NotityService.Notify("检查是否连接到校园网");
            }
            catch
            {
                Services.NotityService.Notify("其他错误");
            }
        }

        public DelegateCommand LostCommand { get; set; }
        public async void LostCampusCard()
        {
            var queryResult=await Services.NotityService.DisplayQueryAlert("工大助手", "确定挂失？(T_T)",cancel:"手抖了");
            if (queryResult != true)
                return;

            IsLoading = true;
            //ProgressMessage = "挂失中···";
            try
            {
                var re = await _campusCardSercvice.LostCampusCard(_httpService, CampusCardInfo.smtCardid);
                LoadCampusCardSalaryInfo();//刷新信息
                Services.NotityService.Notify(re);
                //GalaSoft.MvvmLight.Messaging.Messenger.Default.Send(re, messageToken);
            }
            catch (HttpRequestException requestException)
            {
                Services.NotityService.Notify("请检查网络连接");
                //GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("请检查网络连接", messageToken);
            }
            catch (System.Net.WebException)
            {
                Services.NotityService.Notify("检查是否连接到校园网");
            }
            catch
            {
                Services.NotityService.Notify("未知错误");
                //GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("未知错误", messageToken);
            }
            finally
            {
                IsLoading = false;
            }

        }

        public DelegateCommand FoundCommand { get; set; }
        public async void FoundCampusCard()
        {
            IsLoading = true;
            //ProgressMessage = "解除挂失中···";
            try
            {
                var re = await _campusCardSercvice.FundCampusCard(_httpService, CampusCardInfo.smtCardid);
                LoadCampusCardSalaryInfo();//刷新信息
                Services.NotityService.Notify(re);
                //GalaSoft.MvvmLight.Messaging.Messenger.Default.Send(re, messageToken);
            }
            catch (HttpRequestException requestException)
            {
                Services.NotityService.Notify("请检查网络连接");
                //GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("请检查网络连接", messageToken);
            }
            catch (System.Net.WebException)
            {
                Services.NotityService.Notify("检查是否连接到校园网");
            }
            catch
            {
                Services.NotityService.Notify("未知错误");
                //GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("未知错误", messageToken);
            }
            finally
            {
                IsLoading = false;
            }
        }



        public DelegateCommand<object> ItemClickedCommand { get; set; }
        public async void ItemClicked(object param)
        {
            var naviParam = new NavigationParameters();
            naviParam.Add("campuscardtransactionitemmodel", param);
            await _navigationService.NavigateAsync(typeof(Views.CampusCardTransactionDeatilPage).Name, naviParam);
        }
        private IList<Models.CampusCardTransactionItemModel> _transacitonList;
        public IList<Models.CampusCardTransactionItemModel> TransactionList
        {
            get { return _transacitonList; }
            set { SetProperty(ref _transacitonList, value); }
        }
        public async Task GetTransactionInfo()
        {
            var re = await _campusCardSercvice.GetTransactionInfo(_httpService);
            TransactionList = re;
        }


    }
}
