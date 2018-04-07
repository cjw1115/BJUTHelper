using BJUTDUHelperXamarin.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BJUTDUHelperXamarin.ViewModels
{
    public class EduGradePageViewModel : BindableBase,INavigationAware
    {
        
        private Services.HttpBaseService _httpService;
        private Services.EduService _coreService;
        private INavigationService _navigationService;
        private Services.DbService _dbService;
        

        private string _studentName;

        public  Models.GradeChart GradeChart { get; set; } = new Models.GradeChart();

        public static EduGradePageViewModel GradePageVM;
        private bool _isLoading = false;

        public bool IsLoading
        {
            get { return _isLoading; }
            set { SetProperty(ref _isLoading , value); }
        }


        private Models.BJUTEduCenterUserinfo BJUTEduCenterUserinfo { get; set; }
        public EduGradePageViewModel(INavigationService navigationService)
        {
            GradePageVM = this;

            _navigationService =navigationService;
            _dbService=new Services.DbService();
            _coreService = new Services.EduService();

            
            ItemClickCommand = new DelegateCommand<object>(ItemClick);
            RefreshCommand = new DelegateCommand(Refresh);
            ToGradeInfoCommand = new DelegateCommand(ToGradeInfo);
        }
        public void OnNavigatedFrom(NavigationParameters parameters)
        {   
        }
        public void OnNavigatedTo(NavigationParameters parameters)
        {
            if (!parameters.ContainsKey("backfromdetail"))
            {
                _coreService = EduPageViewModel.CoreService;
                _httpService = EduPageViewModel.HttpService;
                _studentName = EduPageViewModel.Name;

                //Loaded();
                //加载离线数据
                LoadGradeChart();

                //获取最新成绩
                LoadOnlineData();
            }
        }

        private bool _isRefreshing=false;
        public bool IsRefreshing
        {
            get { return _isRefreshing; }
            set {SetProperty(ref _isRefreshing ,value); }
        }
        public DelegateCommand RefreshCommand { get; set; }
        public async void Refresh()
        {
            IsRefreshing = true;
            try
            {
                BJUTEduCenterUserinfo = _dbService.GetAll<Models.BJUTEduCenterUserinfo>().FirstOrDefault();
                if (BJUTEduCenterUserinfo != null)
                {

                    var gradeChart = await GetGrade(_studentName, BJUTEduCenterUserinfo.Username);//获取最新数据
                    if (gradeChart != null)
                    {
                        GradeChart.Clear();
                        GradeChart.MainList.Clear();

                        foreach (var item in gradeChart)
                        {
                            GradeChart.Add(item);
                        }

                        GradeChart.InitList();

                        SaveGradeChart();//保存成绩表
                    }
                }
            }
            catch
            {
                Services.NotityService.Notify("解析最新数据失败");
            }
            finally
            {
                
                IsRefreshing = false;
           
            }
        }

        public async void LoadOnlineData()
        {
            IsLoading = true;
            try
            {
                BJUTEduCenterUserinfo = _dbService.GetAll<Models.BJUTEduCenterUserinfo>().FirstOrDefault();
                if (BJUTEduCenterUserinfo != null)
                {

                    var gradeChart = await GetGrade(_studentName, BJUTEduCenterUserinfo.Username);//获取最新数据
                    if (gradeChart != null)
                    {
                        GradeChart.Clear();
                        GradeChart.MainList.Clear();

                        foreach (var item in gradeChart)
                        {
                            GradeChart.Add(item);
                        }

                        GradeChart.InitList();

                        SaveGradeChart();//保存成绩表
                    }
                }
            }
            catch
            {
                Services.NotityService.Notify("解析最新数据失败");
            }
            finally
            {

                IsLoading = false;

            }
        }
        public DelegateCommand<object> ItemClickCommand{get;set;}
        public async void ItemClick(object param)
        {
            var model=(Models.EduGradeItemModel)param;
            NavigationParameters naviParam=new NavigationParameters();
            naviParam.Add("gradeitemmodel", model);
            await _navigationService.NavigateAsync("EduGradeDetailPage", naviParam);
        }

        public DelegateCommand ToGradeInfoCommand { get; set;}
        public async void ToGradeInfo()
        {
            await _navigationService.NavigateAsync(typeof(Views.EduGradeInfoPage).Name);
        }

        #region 成绩本地管理逻辑
        public  void SaveGradeChart()
        {   
            _dbService.DeleteAll<Models.EduGradeItemModel>();
            _dbService.Insert<Models.EduGradeItemModel>(GradeChart);
        }

        public  void LoadGradeChart()
        {
            var list=_dbService.GetAll<Models.EduGradeItemModel>();
            if(list!=null)
            {
                foreach (var item in list)
                {
                    GradeChart.Add(item);
                }
            }
            GradeChart.InitList();
        }
        #endregion

        #region 学年学期列表逻辑

        private int _selectedTermIndex = -1;
        public int SelectedTermIndex
        {
            get { return _selectedTermIndex; }
            set
            {
                SetProperty(ref _selectedTermIndex, value);
                cbTerm_SelectionChanged();
            }
        }

        private int _selectedYearIndex;
        public int SelectedYearIndex
        {
            get { return _selectedYearIndex; }
            set
            {
                SetProperty(ref _selectedYearIndex, value);
                cbSchoolYear_SelectionChanged();
            }
        }

        public void cbSchoolYear_SelectionChanged()
        {
            if (SelectedYearIndex == -1)
                return;
            SelectedTermIndex = 0;
           var selectedYear = GradeChart.SchoolYearList[SelectedYearIndex];
            var selectedTerm = GradeChart.TermList[SelectedTermIndex];
            GradeChart.GetSpecificGradeChart(selectedYear, selectedTerm);
            //lvGrade.ItemsSource = gd.gc;
            
        }
        public void cbTerm_SelectionChanged()
        {
            if (SelectedTermIndex == -1)
                return;
            var selectedYear = GradeChart.SchoolYearList[SelectedYearIndex];
            var selectedTerm= GradeChart.TermList[SelectedTermIndex];
            GradeChart.GetSpecificGradeChart(selectedYear, selectedTerm);

           
        }
        #endregion

        private async Task<Models.GradeChart> GetGrade(string name, string username)
        {
            
            string html = string.Empty;
            try
            {
                html=await _coreService.GetGrade(_httpService, name, username);
                var gradeChart = new Models.GradeChart();
                gradeChart.GetGradeChart(html);//解析成绩表
                return gradeChart;
            }
            catch (HttpRequestException requestException)
            {
                Services.NotityService.Notify("获取数据失败");
                // GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("获取数据失败", messageToken);
            }
            //catch (System.Net.WebException)
            //{
            //    Services.NotityService.Notify("检查是否连接到校园网");
            //}
            catch
            {
                Services.NotityService.Notify("意外错误");
                // GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("意外错误", messageToken);
            }
          
            return null;
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
            
        }
    }
}
