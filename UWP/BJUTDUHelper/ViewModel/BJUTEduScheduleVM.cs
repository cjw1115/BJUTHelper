
using BJUTDUHelper.BJUTDUHelperlException;
using BJUTDUHelper.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace BJUTDUHelper.ViewModel
{
    public class BJUTEduScheduleVM:ViewModelBase
    {
        private readonly string messageToken = "1";
        private Service.HttpBaseService _httpService;
        private Service.BJUTEduCenterService _coreService;
        private Model.BJUTEduCenterUserinfo BJUTEduCenterUserinfo;
        private Model.ScheduleModel _schedule;
        public Model.ScheduleModel Schedule
        {
            get { return _schedule; }
            set { Set(ref _schedule, value); }
        }

        public ViewModel.CheckCodeVM CheckCodeVM { get; set; }
        public AccountModifyVM AccountModifyVM { get; set; }
        public  string Name { get; set; }

       
        private EduTimeModel _eduTime;
        public EduTimeModel EduTime
        {
            get { return _eduTime; }
            set { Set(ref _eduTime, value); }
        }

        private string _selectedSchoolYear;
        public string SelectedSchoolYear
        {
            get { return _selectedSchoolYear; }
            set { Set(ref _selectedSchoolYear, value); }
        }

        private List<string> _shoolYears;
        public List<string> ShoolYears
        {
            get { return _shoolYears; }
            set { _shoolYears = value; }
        }

        private int _selectedTerm = 1;
        public int SelectedTerm
        {
            get { return _selectedTerm; }
            set { Set(ref _selectedTerm, value); }
        }
        private List<int> _terms;
        public List<int> Terms
        {
            get { return _terms; }
            set { Set(ref _terms, value); }
        }

        private bool hasLoadLoaclInfo = false;
        public async void cbTrems_SelectionChanged(object sender,SelectionChangedEventArgs args)
        {
            if (hasLoadLoaclInfo == false)
                return;
            //if (args.AddedItems != null && args.AddedItems.Count > 0&& EduTime!=null)
            //{
            //    if (EduTime.Term == (int)args.AddedItems[0])
            //    {
            //        return;
            //    }
            //}
            //else
            //    return;

            RefreshSpecific();
        }
        public async void cbSchoolYears_SelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (hasLoadLoaclInfo == false)
                return;
            //if (args.AddedItems != null && args.AddedItems.Count > 0&&EduTime != null)
            //{
            //    if (EduTime.SchoolYear == (string)args.AddedItems[0])
            //    {
            //        return;
            //    }
            //}
            //else
            //    return;

            RefreshSpecific();
        }
        public void InitEduTimeList()
        {
            Terms = new List<int> { 1, 2 };
            ShoolYears = new List<string>();
            for (int i = DateTime.Now.Year - 3; i <= DateTime.Now.Year; i++)
            {
                string year = $"{i}-{i + 1}";
                ShoolYears.Add(year);
            }
            
        }

        public BJUTEduScheduleVM()
        {
            _coreService = new Service.BJUTEduCenterService();
            CheckCodeVM = new CheckCodeVM();
            CheckCodeVM.CheckCodeSaved += CheckCodeSaved;
            CheckCodeVM.CheckCodeRefresh += CheckCodeRefresh;

            AccountModifyVM = new AccountModifyVM();
            AccountModifyVM.Saved += SaveUserinfo;

            InitEduTimeList();
        }
        public async void Loaded(object param)
        {
            if (param != null)
            {
                View.EduCenterViewParam eduCenterViewParam = param as View.EduCenterViewParam;
                BJUTEduCenterUserinfo = eduCenterViewParam.BJUTEduCenterUserinfo;
                _httpService = eduCenterViewParam.HttpService;

                EduTime = eduCenterViewParam.Other as EduTimeModel;
            }

            if (EduTime != null)
            {
                SelectedSchoolYear = EduTime.SchoolYear;
                SelectedTerm = EduTime.Term;
            }
         
            var scedule= await LoadSchedule();
            if (scedule != null)
            {
                if (Schedule == null)
                {
                    Schedule = new Model.ScheduleModel();
                }
               
                Schedule.ScheduleItemList = scedule.ScheduleItemList;
                Schedule.Weeks = scedule.Weeks;
                Schedule.AllWeek = scedule.AllWeek;


                if (EduTime!=null&&EduTime.Week != 0)
                {
                    Schedule.CurrentWeek = EduTime.Week;
                    Schedule.SelectedWeek = EduTime.Week;
                }
                else
                {
                    Schedule.CurrentWeek = scedule.CurrentWeek;
                    Schedule.SelectedWeek = scedule.SelectedWeek;
                }
            }

            hasLoadLoaclInfo = true;
        }
       
        public async void GetCurrentSchedule(string name,string username)
        {
            string html = string.Empty;
            try
            {
                html=await _coreService.GetCurrentSchedule(_httpService, name, username);   
            }
            catch (HttpRequestException ex)
            {
                GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("获取数据失败", messageToken);
            }
            catch
            {
                GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("遇到意外错误", messageToken);
                return;
            }
            try
            {
                var list = Model.ScheduleModel.GetSchedule(html);//获取课表
                var temp = new Model.ScheduleModel { ScheduleItemList = list, CurrentWeek = EduTime.Week, };

                temp.GetAllWeek();//获取最大周数
                temp.SelectedWeek = EduTime.Week;

                Schedule = temp;
                SaveSchedule();
            }
            catch
            {
                GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("解析数据失败", messageToken);
            }
        }

        public async void GetSpecificSchedule()
        {
            string html = string.Empty;
            try
            {
                html = await _coreService.GetSpecificSchedule(_httpService, Name,BJUTEduCenterUserinfo.Username,SelectedSchoolYear,SelectedTerm);
            }
            catch (HttpRequestException ex)
            {
                GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("获取数据失败", messageToken);
            }
            catch
            {
                GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("遇到意外错误", messageToken);
                return;
            }
            try
            {
                var list = Model.ScheduleModel.GetSchedule(html);//获取课表
                var temp = new Model.ScheduleModel { ScheduleItemList = list, CurrentWeek = 1, SelectedWeek = 1 };

                temp.GetAllWeek();//获取最大周数
                Schedule = temp;
                
            }
            catch
            {
                GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("解析数据失败", messageToken);
            }
        }




        #region 本地管理逻辑
        public async void SaveSchedule()
        {
            DAL.LocalSetting _localSetting = new DAL.LocalSetting();
            await _localSetting.SetLocalInfo<Model.ScheduleModel>(typeof(Model.ScheduleModel).Name, Schedule);
        }

        private object lockObject = new object();
        public async Task<Model.ScheduleModel> LoadSchedule()
        {
            DAL.LocalSetting _localSetting = new DAL.LocalSetting();
            var scheduleModel = await _localSetting.GetLocalInfo<Model.ScheduleModel>(typeof(Model.ScheduleModel).Name);
            return scheduleModel;
        }
        #endregion

        public async void Refresh()
        {
            try
            {
                var re = await _coreService.GetAuthState(_httpService, BJUTEduCenterUserinfo.Username);
                if (re == true)
                {
                    GetCurrentSchedule(Name, BJUTEduCenterUserinfo.Username);
                }
                else
                {
                    CheckCodeRefresh();
                }
            }
            catch(HttpRequestException requestException)
            {
                GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("网络错误", messageToken);
            }
            catch (Exception ex)
            {
                GalaSoft.MvvmLight.Messaging.Messenger.Default.Send(ex.Message, messageToken);
            }
        }
        public async void RefreshSpecific()
        {
            try
            {
                var re = await _coreService.GetAuthState(_httpService, BJUTEduCenterUserinfo.Username);
                if (re == true)
                {
                    GetSpecificSchedule();
                }
                else
                {
                    CheckCodeRefresh();
                }
            }
            catch (HttpRequestException requestException)
            {
                GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("网络错误", messageToken);
            }
            catch (Exception ex)
            {
                GalaSoft.MvvmLight.Messaging.Messenger.Default.Send(ex.Message, messageToken);
            }
        }

        //保存验证码后登录并导航
        public async void CheckCodeSaved()
        {
            try
            {
                if (BJUTEduCenterUserinfo == null)
                {
                    throw new NullRefUserinfoException("请输入用户名和密码");
                }
                var name = await _coreService.LoginEduCenter(_httpService, BJUTEduCenterUserinfo.Username, BJUTEduCenterUserinfo.Password, CheckCodeVM.CheckCode);

                Name = name;
                GetCurrentSchedule(name, BJUTEduCenterUserinfo.Username);

            }
            catch(NullRefUserinfoException  )
            {
                GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("请输入用户名和密码", messageToken);
                AccountModifyVM.Open = true;
                AccountModifyVM.Saved -= CheckCodeRefresh;
                AccountModifyVM.Saved += CheckCodeRefresh;
            }
            catch (HttpRequestException requestException)
            {
                GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("网络错误", messageToken);
            }
            catch (InvalidCheckcodeException checkcodeExcepiton)
            {
                GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("验证码错误", messageToken);

                CheckCodeRefresh();
            }
            catch (InvalidUserInfoException userInfoException)
            {
                GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("用户名或密码错误", messageToken);

                AccountModifyVM.Open = true;
                AccountModifyVM.Saved -= CheckCodeRefresh;
                AccountModifyVM.Saved += CheckCodeRefresh;

            }
            catch (Exception ex)
            {
                GalaSoft.MvvmLight.Messaging.Messenger.Default.Send(ex.Message, messageToken);
            }
        }
        //刷新验证码的逻辑
        public async void CheckCodeRefresh()
        {
            CheckCodeVM.OpenCheckCodeDlg = true;
            var source = await _coreService.GetCheckCode(_httpService);
            CheckCodeVM.CheckCodeSource = source;
        }
       
        //保存用户名密码
        public async void SaveUserinfo()
        {
            if(BJUTEduCenterUserinfo==null)
                BJUTEduCenterUserinfo = new Model.BJUTEduCenterUserinfo();
            BJUTEduCenterUserinfo.Username = AccountModifyVM.Username;
            BJUTEduCenterUserinfo.Password =  AccountModifyVM.Password;

            Service.DbService.SaveInfoCenterUserinfo(BJUTEduCenterUserinfo);
            
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Send<string>("保存成功", messageToken);
            
        }
    }
}