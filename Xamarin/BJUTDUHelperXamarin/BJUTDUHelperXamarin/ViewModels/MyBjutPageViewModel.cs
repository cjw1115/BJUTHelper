using BJUTDUHelperXamarin.Models;
using BJUTDUHelperXamarin.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BJUTDUHelperXamarin.ViewModels
{
    public class MyBjutPageViewModel : BindableBase,INavigatedAware
    {
        
        private Services.HttpBaseService _httpService;
        private Services.DbService _dbService;
        private INavigationService _navigationService;
        public MyBjutPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            _httpService = new HttpBaseService();
            _dbService =new DbService();

            ItemClickCommand = new DelegateCommand<object>(HeaderItemClicked);

            SalaryCommand = new DelegateCommand(Salary);
            LoadedCommand = new DelegateCommand(Loaded);
            UnLoadedCommand = new DelegateCommand(UnLoaded);
            ScheduleCommand = new DelegateCommand(Schedule);
            BookingGradeCommand = new DelegateCommand(BookingGrade);
            MomentsCommand = new DelegateCommand(Moments);

            HeaderList = new List<NewsHeaderModel>();

            
        }

        public DelegateCommand BookingGradeCommand { get; set; }
        public void BookingGrade()
        {
            _navigationService.NavigateAsync(typeof(Views.BookingGradePage).Name);
        }

        private bool _isAutoPlay = true;

        public bool IsAutoPlay
        {
            get { return _isAutoPlay; }
            set { SetProperty(ref _isAutoPlay , value); }
        }

        public DelegateCommand LoadedCommand { get; set; }
        public async void Loaded()
        {

            //获取本地时间
            var loaclEdutime = GetLocalEdutime();
            EduTime = loaclEdutime;
            //获取教务时间信息
            var re = await GetNetEduTime();
            if (re != null)
            {
                EduTime = re;
                SetLocalEdutime(EduTime);
            }

            if (!_isLoadedNews)
            {
                LoadNewsHeaders();
            }
            IsAutoPlay = true;
        }
        public DelegateCommand UnLoadedCommand { get; set; }
        public  void UnLoaded()
        {
            IsAutoPlay = false;
        }
        private bool _isLoadedNews = false;
        private List<NewsHeaderModel> _headerList;

        public List<NewsHeaderModel> HeaderList 
        {
            get { return _headerList; }
            set { SetProperty(ref _headerList, value); }
        }
        private async void LoadNewsHeaders()
        {
            _isLoadedNews = true;

            LoadLocalNewsHeaders();
            var newList = new List<NewsHeaderModel>();
            var re=await Services.NewsService.GetHeaders(_httpService);
            if (re != null && re.Count > 0)
            {
                
                foreach (var item in re)
                {
                    newList.Add(item);
                }
            }
            if (newList != null && newList.Count > 0)
            {
                newList.Insert(0, HeaderList[0]);
                HeaderList = newList;
            }
               
        }

        public ICommand ItemClickCommand { get; set; }
        public async void HeaderItemClicked(object param)
        {
            var header = param as Models.NewsHeaderModel;
            if (header != null&&!string.IsNullOrWhiteSpace(header.ContentUri))
            {
                var naviParam = new NavigationParameters();
                naviParam.Add(typeof(NewsHeaderModel).Name, header);
                naviParam.Add("httpservice", _httpService);
                await _navigationService.NavigateAsync(typeof(Views.NewsPage).Name, naviParam);
            }
            
        }
        public void LoadLocalNewsHeaders()
        {
            HeaderList = new List<NewsHeaderModel> { new NewsHeaderModel() { ContentUri = "https://cjw1115.com/bjutduhelper/appview/", ImageUri = "notice.png" } };
        }
        #region 获取教学教务基础信息
        public static Models.EduTimeModel EduTimeProxy;
        private Models.EduTimeModel _eduTime;
        public Models.EduTimeModel EduTime
        {
            get { return _eduTime; }
            set { SetProperty(ref _eduTime, value);
                MyBjutPageViewModel.EduTimeProxy = value;
            }
        }

        public Models.EduTimeModel GetLocalEdutime()
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
        public void SetLocalEdutime(Models.EduTimeModel eduTime)
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
                var re = await Services.EduService.GetEduTime(_httpService);//自己的服务器
                eduTime = re;
            }
            catch
            {
            }
            return eduTime;
        }
        #endregion

        #region 功能命令相关
        public ICommand SalaryCommand { get; set; }
        public void Salary()
        {
            _navigationService.NavigateAsync(typeof(Views.SalaryPage).Name);
        }

        public ICommand ScheduleCommand { get; set; }
        public async void Schedule()
        {
            var naviParam = new NavigationParameters();
            naviParam.Add("edutime", EduTime);
            await _navigationService.NavigateAsync(typeof(Views.EduSchedulePage).Name, naviParam);
        }

        public ICommand MomentsCommand { get; set; }
        public async void Moments()
        {
            var naviParam = new NavigationParameters();
            naviParam.Add("IsLoad", true);
            await _navigationService.NavigateAsync(typeof(Views.MyBJUT.MomentsViewPage).Name, naviParam);
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
            
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            
        }
        #endregion
    }
}
