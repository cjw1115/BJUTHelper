using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Navigation;
using Xamarin.Forms;
using BJUTDUHelperXamarin.Services;
using System.Threading.Tasks;
using System.Net.Http;

namespace BJUTDUHelperXamarin.ViewModels
{
    public class EduPageViewModel : BindableBase,INavigationAware
    {
        //保存当前登录账号信息
        public static Models.BJUTEduCenterUserinfo BJUTEduCenterUserinfo { get; set; }
        //保存登录的用户姓名
        public static string Name { get; set; }
        //教务管理系统信息服务类
        public static Services.EduService CoreService { get; set; } = new EduService();
        //教务系统网络请求服务类，保存相关的cookie等
        public static Services.HttpBaseService HttpService { get; set; }= new Services.HttpBaseService(true);

       

        private static string _captchaText;
        public static string CaptchaText
        {
            get { return _captchaText; }
            set { _captchaText = value; }
        }
        private Models.EduItemModel selectedModel;
        private static bool _isAuthencation = false;
        public static bool IsAuthencation
        {
            get { return _isAuthencation; }
            set { _isAuthencation = value; }
        }

        /// <summary>
        /// 约定导航到本页面参数必须提供from字段,CLR类型为Type
        /// </summary>
        /// <param name="parameters"></param>
        public async void OnNavigatedTo(NavigationParameters parameters)
        {
            var frompage=(Type)parameters["from"];
            if (frompage == typeof(Views.CaptchaPage))
            {
                CaptchaText = (string)parameters["captchatext"];
                await Login();
                return;
            }
            else if(frompage==typeof(Views.UserInfoDetailPage))
            {
                var eduuser=_dbService.GetAll<Models.BJUTEduCenterUserinfo>().FirstOrDefault();
                BJUTEduCenterUserinfo=eduuser;
                if(selectedModel!=null)
                {
                    await Login();
                }
            }
        }
        public void OnNavigatingTo(NavigationParameters parameters)
        {
        }
        public void OnNavigatedFrom(NavigationParameters parameters)
        {
        }

        //主页面功能列表
        public List<Models.EduItemModel> EduItems { get; set; } = new List<Models.EduItemModel>
        {
            new Models.EduItemModel { Name="课程表",PageType=typeof(Views.EduSchedulePage), IconUri='\uf073'.ToString()},
            new Models.EduItemModel { Name="考试查询",PageType=typeof(Views.EduExamPage),IconUri='\uf040'.ToString()},
            new Models.EduItemModel { Name="成绩查询",PageType=typeof(Views.EduGradePage), IconUri='\uf06c'.ToString()}
        };
        
        
        #region 标识是否能链接到教务管理系统网站
        private bool _isConnected;
        public bool IsConnected
        {
            get { return _isConnected; }
            set { SetProperty(ref _isConnected, value); }
        }
        #endregion

        private INavigationService _navigationService;
        private DbService _dbService;
        public EduPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

            _dbService = new DbService();

            ItemClickCommand = new DelegateCommand<object>(ItemClick);
             LoadedCommand=new DelegateCommand(Loaded);

        }

        public DelegateCommand LoadedCommand{get;set;}
        public async void Loaded()
        {
            //加载基本账号信息，用户名，密码

            var eduuser =_dbService.GetAll<Models.BJUTEduCenterUserinfo>().FirstOrDefault();
            if (eduuser == null)
            {
                await GetUserInfo(_navigationService);
                return;
            }

            BJUTEduCenterUserinfo = eduuser;

            //获取本地时间
            var loaclEdutime = GetLocalEdutime();
            EduTime = loaclEdutime;

            IsConnected = await CoreService.GetConnectedStatus(HttpService);

            //获取教务时间信息
            var re = await GetNetEduTime();
            if (re != null)
            {
                EduTime = re;
                SetLocalEdutime(EduTime);
            }
        }
        public DelegateCommand<object> ItemClickCommand { get; set; }
        public async void ItemClick(object param)
        {
            
            var model = (Models.EduItemModel)param;
            selectedModel = model;
            if (model.PageType == null)
            {
                Services.NotityService.Notify("这个饼还没画完O(∩_∩)O");
                
                return;
            }

            //课程表页面特殊，可无网络连接查看
            if (model != null && model.PageType == typeof(Views.EduSchedulePage))
            {
                var naviParam = new NavigationParameters();
                naviParam.Add("edutime", EduTime);
                
                await _navigationService.NavigateAsync(model.PageType.Name, naviParam);
                return;
            }

            if (IsConnected)//
            {
                if (BJUTEduCenterUserinfo == null)
                {
                    //当前用户信息为空，直接转到登陆流程。
                    await Login();
                    return;
                }
                var re = await CoreService.GetAuthState(HttpService, BJUTEduCenterUserinfo.Username);
                if (re != true)//没有认证教务系统，首先获取验证码，取到验证码后开始登陆逻辑，登陆逻辑在NavigationTo方法里面处理
                {
                    await Login();
                    return;
                }
                else//已经认证，直接打开
                {
                    if (model != null && model.PageType != null)
                    {
                        
                        await _navigationService.NavigateAsync(model.PageType.Name);//导航到具体页面
                    }
                }
            }
            else
            {
                Services.NotityService.Notify("网络连接出现问题/(ㄒoㄒ)/~~");
            }
        }

        private Models.EduItemModel ClickedItem { get; set; }

        public static async Task GetCaptchaText(INavigationService naviService,Services.HttpBaseService httpService)
        {
            //获取验证码，非等待，继续执行
            var naviParam = new NavigationParameters();
            naviParam.Add("httpservice", httpService);
            naviParam.Add("ILogin",CoreService);
            await naviService.NavigateAsync(typeof(Views.CaptchaPage).Name, naviParam);
        }
        public static async Task GetUserInfo(INavigationService naviService)
        {
            NavigationParameters naviParam = new NavigationParameters();
            naviParam.Add("title", "教务管理");
            naviParam.Add("model", EduPageViewModel.BJUTEduCenterUserinfo);
            await naviService.NavigateAsync("UserInfoDetailPage", naviParam);//提示重新输入账号
        }
        public async Task Login()
        {
            IsLoading = true;
            try
            {
                if (BJUTEduCenterUserinfo == null)
                {
                    await GetUserInfo(_navigationService);
                    return;
                }
                if (string.IsNullOrEmpty(CaptchaText))
                {
                    await GetCaptchaText(_navigationService,EduPageViewModel.HttpService);
                    return;
                }

                Name = await CoreService.LoginEduCenter(HttpService, BJUTEduCenterUserinfo.Username, BJUTEduCenterUserinfo.Password, CaptchaText);

                if (selectedModel != null && selectedModel.PageType != null)
                {
                    await _navigationService.NavigateAsync(selectedModel.PageType.Name);//导航到具体页面
                }

            }
            catch (HttpRequestException)
            {
                Services.NotityService.Notify("网络错误/(ㄒoㄒ)/~~");
                //提示网络错误
            }
            catch (System.Net.WebException)
            {
                Services.NotityService.Notify("检查是否连接到校园网");
            }
            catch (InvalidCheckcodeException)
            {
                Services.NotityService.Notify("验证码错误/(ㄒoㄒ)/~~");
                await GetCaptchaText(_navigationService,EduPageViewModel.HttpService);
            }
            catch (InvalidUserInfoException)
            {
                Services.NotityService.Notify("用户名或密码错误/(ㄒoㄒ)/~~");
                await GetUserInfo(_navigationService);
            }
            catch (Exception ex)
            {
                Services.NotityService.Notify($"其他错误{ex.Message}");
                //提示其他错误
            }
            finally
            {
                EduPageViewModel.CaptchaText = string.Empty;
                IsLoading = false;
            }
        }
       
        #region 获取教学教务基础信息
        private Models.EduTimeModel _eduTime;
        public Models.EduTimeModel EduTime
        {
            get { return _eduTime; }
            set { SetProperty(ref _eduTime, value); }
        }
       
        public  Models.EduTimeModel GetLocalEdutime()
        {

            var timeinfo = _dbService.GetAll<Models.EduTimeModel>().FirstOrDefault();

            if (timeinfo != null)
            {
                //自动调整周数
                var startDayOfWeek = timeinfo.CreateTime.DayOfWeek;
                var nowDayOfWeek = DateTime.Now.Date.DayOfWeek;

                var daySpan = DateTime.Now.Date - timeinfo.CreateTime.Date;
                var weekSpan = daySpan.Days / 7;
                timeinfo.Week += weekSpan;
                var dis = nowDayOfWeek - startDayOfWeek;
                if (dis < 0 && dis > -(int)startDayOfWeek)//这一周之类
                {
                    timeinfo.Week++;
                }
            }

            return timeinfo;
        }
        public  void SetLocalEdutime(Models.EduTimeModel eduTime)
        {
            eduTime.CreateTime = DateTime.Now;
            //清除之前记录
            _dbService.DeleteAll<Models.EduTimeModel>();
            _dbService.Insert<Models.EduTimeModel>(eduTime);
        }
        public async Task<Models.EduTimeModel> GetNetEduTime()
        {

            Models.EduTimeModel eduTime = null;
            try
            {
                var re = await EduService.GetEduTime(HttpService);//自己的服务器
                if (re == null)
                {
                    re = await CoreService.GetEduBasicInfo(HttpService);//学校的教务官网
                }
                eduTime = re;
            }
            catch
            {

            }
            return eduTime;
        }

       
        #endregion

        private bool _isLoading = false;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { SetProperty(ref _isLoading ,value); }
        }

    }
}