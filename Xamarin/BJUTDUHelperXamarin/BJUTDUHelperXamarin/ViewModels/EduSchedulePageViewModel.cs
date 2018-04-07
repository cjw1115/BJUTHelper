using BJUTDUHelperXamarin.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BJUTDUHelperXamarin.ViewModels
{
    public class EduSchedulePageViewModel : BindableBase, INavigationAware
    {
        private Services.DbService _dbService;
        private INavigationService _navigationService;
        
        private Models.ScheduleModel _schedule;
        public Models.ScheduleModel Schedule
        {
            get { return _schedule; }
            set { SetProperty(ref _schedule, value); }
        }


        private Models.EduTimeModel _eduTime;
        public Models.EduTimeModel EduTime
        {
            get { return _eduTime; }
            set { SetProperty(ref _eduTime, value); }
        }

        #region 课程显示控制
        private bool isGetSpecific = false;
        private int _selectedSchoolYearIndex;
        public int SelectedSchoolYearIndex
        {
            get { return _selectedSchoolYearIndex; }
            set
            {
                var last = _selectedSchoolYearIndex;
                SetProperty(ref _selectedSchoolYearIndex, value);
                if (last != value)
                {
                    OnSpecificChanged(schoolYearIndex:value);
                }


            }
        }

        private List<string> _shoolYears;
        public List<string> ShoolYears
        {
            get { return _shoolYears; }
            set { _shoolYears = value; }
        }

        private int _selectedTermIndex = 1;
        public int SelectedTermIndex
        {
            get { return _selectedTermIndex; }
            set
            {
                var last = _selectedTermIndex;
                SetProperty(ref _selectedTermIndex, value);
                if (last != value)
                {
                    OnSpecificChanged(termIndex:value);
                }
            }
        }

        private List<int> _terms;
        public List<int> Terms
        {
            get { return _terms; }
            set { SetProperty(ref _terms, value); }
        }

        private List<int> _weeks;
        public List<int> Weeks
        {
            get { return _weeks; }
            set { SetProperty(ref _weeks, value); }
        }
        private int _selectedWeekIndex = -1;

        public int SelectedWeekIndex
        {
            get { return _selectedWeekIndex; }
            set
            {
                SetProperty(ref _selectedWeekIndex , value);
                
                Schedule.SelectedWeekIndex = value;
            }
        }



        private bool _isLoading=false;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { SetProperty(ref _isLoading , value); }
        }

        private bool hasLoadLoaclInfo = false;
        public void OnSpecificChanged(int? schoolYearIndex=null,int? termIndex = null)
        {
            if (isRefreshSync == true)
            {
                return;
            }
            if (hasLoadLoaclInfo == false)
                return;
           
            if (isGetSpecific == false)
                RefreshSpecific();
            SelectedWeekIndex = -1;
        }
        #endregion
        public void InitEduTimeList()
        {
            Terms = new List<int> { 1, 2 };
            ShoolYears = new List<string>();
            for (int i = DateTime.Now.Year - 4; i <= DateTime.Now.Year; i++)
            {
                string year = $"{i}-{i + 1}";
                ShoolYears.Add(year);
            }
            Weeks = new List<int>();
            for (int i = 0; i < 16; i++)
            {
                Weeks.Add(i + 1);
            }
        }

        public EduSchedulePageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            _dbService = new Services.DbService();

            RefreshCommand = new DelegateCommand(Refresh);
            ItemClickedCommand = new DelegateCommand<object>(ItemClicked);
            InitEduTimeList();

            Schedule = new Models.ScheduleModel();
        }
        public void OnNavigatedFrom(NavigationParameters parameters)
        {
        }
        /// <summary>
        /// 约定导航到本页面参数必须提供from字段,CLR类型为Type
        /// </summary>
        /// <param name="parameters"></param>
        public async void OnNavigatedTo(NavigationParameters parameters)
        {
            if (parameters.ContainsKey("from"))
            {
                if (parameters["from"] == typeof(Views.EduScheduleDetailPage))
                    return;

                if (parameters["from"] == typeof(Views.CaptchaPage))
                {
                    ViewModels.EduPageViewModel.CaptchaText = (string)parameters["captchatext"];
                    await Login();
                    if (isRefreshing)
                    {
                        Refresh();
                        isRefreshing = false;
                    }
                    return;
                }
                else if (parameters["from"] == typeof(Views.UserInfoDetailPage))
                {
                    var eduuser = _dbService.GetAll<Models.BJUTEduCenterUserinfo>().FirstOrDefault();
                    ViewModels.EduPageViewModel.BJUTEduCenterUserinfo = eduuser;
                    await Login();
                    return;
                }
                
            }
            
           EduPageViewModel.BJUTEduCenterUserinfo = _dbService.GetAll<Models.BJUTEduCenterUserinfo>().FirstOrDefault();
          
            EduTime = (Models.EduTimeModel)parameters["edutime"];


            InitSchedule();
            SetTime();
            hasLoadLoaclInfo = true;
        }

        public void InitSchedule()
        {

            var scedule = LoadSchedule();
            if (scedule != null)
            {
                Schedule = scedule;
                SelectedWeekIndex = 0;
            }
            else
            {
                Services.NotityService.Notify("本地课程表不存在，即将刷新...");
                Refresh();
            }
           
        }

        /// <summary>
        /// 更具传进来的时间设置课表时间
        /// </summary>
        public void SetTime()
        {
            isGetSpecific = true;
            if (EduTime != null)
            {
                var yearIndex = ShoolYears.IndexOf(EduTime.SchoolYear);
                SelectedSchoolYearIndex = yearIndex;

                var termIndex = Terms.IndexOf(EduTime.Term);
                SelectedTermIndex = termIndex;

                var weekIndex = Weeks.IndexOf(EduTime.Week);
                SelectedWeekIndex = weekIndex;
            }
            isGetSpecific = false;
        }

        public async Task GetCurrentSchedule(string name, string username)
        {
            string html = string.Empty;
            try
            {
                html = await EduPageViewModel.CoreService.GetCurrentSchedule(EduPageViewModel.HttpService, name, username);
            }
            catch (HttpRequestException ex)
            {
                Services.NotityService.Notify("获取数据失败");
                //GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("获取数据失败", messageToken);
            }
            //catch (System.Net.WebException)
            //{
            //    Services.NotityService.Notify("检查是否连接到校园网");
            //}
            catch
            {
                Services.NotityService.Notify("遇到意外错误");
                //GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("遇到意外错误", messageToken);
                return;
            }
            try
            {
                var list = Models.ScheduleModel.GetSchedule(html);//获取课表
                var temp = new Models.ScheduleModel { ScheduleItemList = list, ShcoolYear = EduTime.SchoolYear, Term = EduTime.Term };

                if (temp.ScheduleItemList.Count <= 0)
                {
                    Services.NotityService.Notify("没有课程信息");
                }
                Schedule = temp;
                if(Weeks.Count < EduTime.Week)
                {
                    SelectedWeekIndex = temp.Weeks.Count - 1;
                }
                else
                {
                    SelectedWeekIndex = EduTime.Week - 1;
                }


                
                SetTime();

                SaveSchedule();
            }
            catch(Exception e)
            {
                Services.NotityService.Notify("解析数据失败");
                //GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("解析数据失败", messageToken);
            }
        }

        public async Task GetSpecificSchedule(string name,string username,string schoolYear,int term)
        {
            string html = string.Empty;
            
            try
            {
                if (schoolYear == EduTime.SchoolYear && term == EduTime.Term)
                {
                    html = await ViewModels.EduPageViewModel.CoreService.GetCurrentSchedule(EduPageViewModel.HttpService, name, username);   
                }
                else
                {
                    html = await ViewModels.EduPageViewModel.CoreService.GetSpecificSchedule(EduPageViewModel.HttpService, name, username, schoolYear, term);
                }
                    
            }
            catch (HttpRequestException ex)
            {
                Services.NotityService.Notify("获取数据失败");
                 
            }
            catch
            {
                Services.NotityService.Notify("遇到意外错误");
                return;
            }
            try
            {
                var list = Models.ScheduleModel.GetSchedule(html);//获取课表
                var temp = new Models.ScheduleModel { ScheduleItemList = list, ShcoolYear = schoolYear, Term = term};

                if (temp.ScheduleItemList.Count <= 0)
                {
                    Services.NotityService.Notify("没有课程信息");
                }
                Schedule = temp;
                if (schoolYear == EduTime.SchoolYear && term == EduTime.Term)
                {
                    if (Weeks.Count < EduTime.Week)
                    {
                        SelectedWeekIndex = temp.Weeks.Count - 1;
                    }
                    else
                    {
                        SelectedWeekIndex = EduTime.Week - 1;
                    }
                }
                else
                {
                    SelectedWeekIndex = 0;
                }

                if(temp.ShcoolYear==EduTime.SchoolYear&&temp.Term== EduTime.Term)
                {
                    SaveSchedule();
                }
            }
            catch
            {
                Services.NotityService.Notify("解析数据失败");
            }
        }

        #region 本地管理逻辑
        public void SaveSchedule()
        {
            _dbService.DeleteAll<Models.ScheduleItem>();
            _dbService.Insert<Models.ScheduleItem>(Schedule.ScheduleItemList);

            _dbService.DeleteAll<Models.ScheduleModel>();
            _dbService.Insert<Models.ScheduleModel>(Schedule);

        }

        public Models.ScheduleModel LoadSchedule()
        {
            var schedule = _dbService.GetAll<Models.ScheduleModel>().FirstOrDefault();
            if (schedule == null)
                return null;

            if (schedule.ScheduleItemList == null)
            {
                schedule.ScheduleItemList = new ObservableCollection<Models.ScheduleItem>();
            }
            var list = _dbService.GetAll<Models.ScheduleItem>();
            foreach (var item in list)
            {
                schedule.ScheduleItemList.Add(item);
            }
            return schedule;
        }
        #endregion
        public DelegateCommand RefreshCommand { get; set; }

        private bool isRefreshing = false;
        private bool isRefreshSync = false;
        public async void Refresh()
        {
            isRefreshSync = true;
            SelectedSchoolYearIndex = ShoolYears.IndexOf(EduTime.SchoolYear);
            SelectedTermIndex = Terms.IndexOf(EduTime.Term);
            SelectedWeekIndex = -1;
            RefreshSpecific();
            isRefreshSync = false;
            //isRefreshing = true;
            //IsLoading = true;
            //try
            //{
            //    if (EduPageViewModel.BJUTEduCenterUserinfo == null)
            //    {
            //        await EduPageViewModel.GetUserInfo(_navigationService);
            //        return;
            //    }
            //    var re = await ViewModels.EduPageViewModel.CoreService.GetAuthState(EduPageViewModel.HttpService,EduPageViewModel.BJUTEduCenterUserinfo.Username);
            //    if (re == true)
            //    {
            //        sele
            //        var schoolYear = ShoolYears[SelectedSchoolYearIndex];
            //        var term = Terms[SelectedTermIndex];
            //        //await GetCurrentSchedule(ViewModels.EduPageViewModel.Name,EduPageViewModel.BJUTEduCenterUserinfo.Username);
            //        await GetSpecificSchedule(ViewModels.EduPageViewModel.Name, EduPageViewModel.BJUTEduCenterUserinfo.Username,);
            //    }
            //    else
            //    {
            //        await Login();
            //    }
            //}
            //catch (HttpRequestException requestException)
            //{
            //    Services.NotityService.Notify("网络异常");
            //}
            //catch (System.Net.WebException)
            //{
            //    Services.NotityService.Notify("检查是否连接到校园网");
            //}
            //catch (Exception ex)
            //{
            //    Services.NotityService.Notify($"其他错误:{ex.Message}");
            //}
            //finally
            //{
            //    IsLoading = false;
            //}
        }

        public async void RefreshSpecific()
        {
            IsLoading = true;
            try
            {
                if (EduPageViewModel.BJUTEduCenterUserinfo == null)
                {
                    await EduPageViewModel.GetUserInfo(_navigationService);
                    return;
                }
                var re = await EduPageViewModel.CoreService.GetAuthState(EduPageViewModel.HttpService, EduPageViewModel.BJUTEduCenterUserinfo.Username);
                if (re == true)
                {
                    var schoolYear = ShoolYears[SelectedSchoolYearIndex];
                    var term = Terms[SelectedTermIndex];
                    await GetSpecificSchedule(ViewModels.EduPageViewModel.Name, EduPageViewModel.BJUTEduCenterUserinfo.Username,schoolYear,term);
                }
                else
                {
                    await Login();
                }
            }
            catch (HttpRequestException requestException)
            {
                Services.NotityService.Notify("网络错误");

            }
            //catch (System.Net.WebException)
            //{
            //    Services.NotityService.Notify("检查是否连接到校园网");
            //}
            catch (Exception ex)
            {
                Services.NotityService.Notify($"其他错误:{ex.Message}");
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
            naviParam.Add("scheduleitem", param);
            await _navigationService.NavigateAsync(typeof(Views.EduScheduleDetailPage).Name, naviParam);
        }

        public async Task Login()
        {
            IsLoading = true;
            try
            {
                if (EduPageViewModel.BJUTEduCenterUserinfo == null)
                {
                    await ViewModels.EduPageViewModel.GetUserInfo(_navigationService);
                    return;
                }
                if (string.IsNullOrEmpty(EduPageViewModel.CaptchaText))
                {
                    await ViewModels.EduPageViewModel.GetCaptchaText(_navigationService,EduPageViewModel.HttpService);
                    return;
                }

                ViewModels.EduPageViewModel.Name = await EduPageViewModel.CoreService.LoginEduCenter(ViewModels.EduPageViewModel.HttpService,EduPageViewModel.BJUTEduCenterUserinfo.Username, EduPageViewModel.BJUTEduCenterUserinfo.Password, ViewModels.EduPageViewModel.CaptchaText);
            }
            catch (HttpRequestException)
            {
                Services.NotityService.Notify("网络错误/(ㄒoㄒ)/~~");
                //提示网络错误
            }
            //catch (System.Net.WebException)
            //{
            //    Services.NotityService.Notify("检查是否连接到校园网");
            //}
            catch (InvalidCheckcodeException)
            {
                Services.NotityService.Notify("验证码错误/(ㄒoㄒ)/~~");
                await ViewModels.EduPageViewModel.GetCaptchaText(_navigationService,EduPageViewModel.HttpService);
            }
            catch (InvalidUserInfoException)
            {
                Services.NotityService.Notify("用户名或密码错误/(ㄒoㄒ)/~~");
                await ViewModels.EduPageViewModel.GetUserInfo(_navigationService);
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

        public void OnNavigatingTo(NavigationParameters parameters)
        {
            
        }
    }
}