
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;

namespace BJUTDUHelper.ViewModel
{
    public class BJUTEduGradeVM:ViewModelBase
    {
        private readonly string messageToken = "1";
        private Service.HttpBaseService _httpService;
        private Model.GradeChart _gradeChart;
        public Model.GradeChart GradeChart
        {
            get { return _gradeChart; }
            set { Set(ref _gradeChart, value); }
        }

        private Service.BJUTEduCenterService _coreService;
        private Model.BJUTEduCenterUserinfo BJUTEduCenterUserinfo { get; set; }
        public BJUTEduGradeVM()
        {
            _coreService = new Service.BJUTEduCenterService();
        }
        public async void Loaded(object parms)
        {
            //加载离线数据
            LoadGradeChart();

            if (parms != null)
            {
                View.EduCenterViewParam eduCenterViewParam = parms as View.EduCenterViewParam;
                _httpService = eduCenterViewParam.HttpService;
                BJUTEduCenterUserinfo = eduCenterViewParam.BJUTEduCenterUserinfo;

                GetGrade(ViewModel.BJUTEduCenterVM.Name, BJUTEduCenterUserinfo.Username);//获取最新数据
            }
          
        }


        #region 成绩本地管理逻辑
        public async void SaveGradeChart()
        {
            DAL.LocalSetting _localSetting = new DAL.LocalSetting();
            await _localSetting.SetLocalInfo<Model.GradeChart>(typeof(Model.GradeChart).Name, GradeChart);
        }

        private object lockObject = new object();
        public async void LoadGradeChart()
        {
            DAL.LocalSetting _localSetting = new DAL.LocalSetting();
            var chart=await _localSetting.GetLocalInfo<Model.GradeChart>(typeof(Model.GradeChart).Name);
            lock (lockObject)
            {
                GradeChart = chart;
            }
            GradeChart?.InitList();
        }
        #endregion

        #region 学年学期列表逻辑
        private string _weightScore;
        public string WeightScore
        {
            get { return _weightScore; }
            set { Set(ref _weightScore, value); }
        }

        private int _selectedTermIndex = -1;
        public int SelectedTermIndex
        {
            get { return _selectedTermIndex; }
            set { Set(ref _selectedTermIndex, value); }
        }

        private string _selectedYear;
        public string SelectedYear
        {
            get { return _selectedYear; }
            set { Set(ref _selectedYear, value); }
        }

        public void cbSchoolYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           SelectedTermIndex = -1;
           GradeChart.GetSpecificGradeChart(SelectedYear, SelectedTermIndex.ToString());
            //lvGrade.ItemsSource = gd.gc;
            WeightScore = GradeChart.GetWeightAvarageScore();
        }
        public void cbTerm_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GradeChart.GetSpecificGradeChart(SelectedYear, (SelectedTermIndex+1).ToString());

            WeightScore = GradeChart.GetWeightAvarageScore();
        }
        #endregion

        private async void GetGrade(string name, string username)
        {
            string html = string.Empty;
            try
            {
                html=await _coreService.GetGrade(_httpService, name, username);
                
            }
            catch (HttpRequestException requestException)
            {
                GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("获取数据失败", messageToken);
            }
            catch
            {
                GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("意外错误", messageToken);
                return;
            }
            try
            {
                if (GradeChart == null)
                {
                    GradeChart = new Model.GradeChart();
                }
                GradeChart.GetGradeChart(html);//解析成绩表
                SaveGradeChart();//保存成绩表
            }
            catch
            {
                GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("解析数据失败", messageToken);
            }
           
        }
    }
}
