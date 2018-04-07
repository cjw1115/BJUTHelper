using BJUTDUHelperXamarin.Models;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BJUTDUHelperXamarin.ViewModels
{
    public class EduGradeWeightViewViewModel : BindableBase
    {
        public static EduGradeWeightViewViewModel GradeWeightVM { get; set; } = new EduGradeWeightViewViewModel();
        public EduGradeWeightViewViewModel()
        {

            ReloadCommand = new DelegateCommand(ReLoadGrade);
            AddCommand = new DelegateCommand(Add);
            DeleteCommand = new DelegateCommand<object>(Delete);

            CalcGradeGpaCommand = new DelegateCommand(CalcGradeGpa);
            CalcGradeWeightCommand = new DelegateCommand(CalcGradeWeight);


            InitData();
        }
        public async void InitData()
        {
            var re=await LoadGrade();
            GradeChart = re;

            var gpaMethods=await LoadGpaMethod();
            GpaModels = gpaMethods;

        }
        public async Task<GradeChart>LoadGrade()
        {
            GradeChart gradeChart = null;
            await Task.Run(() =>
            {   gradeChart=new GradeChart();
                foreach (var item in EduGradePageViewModel.GradePageVM.GradeChart)
                {
                    EduGradeItemModel model = new EduGradeItemModel();
                    model.Subject = item.Subject;
                    model.Score = item.Score;
                    model.Credit = item.Credit;
                    model.SchoolYear = item.SchoolYear;
                    model.Term = item.Term;

                    gradeChart.Add(model);
                }

                gradeChart.InitList();
            });
            return gradeChart;
        }
        public DelegateCommand ReloadCommand { get; set; }
        public void ReLoadGrade()
        {

        }

        private EduGradeItemModel _newGrade = new EduGradeItemModel();

        public EduGradeItemModel NewGrade
        {
            get { return _newGrade; }
            set { SetProperty(ref _newGrade, value); }
        }

        public DelegateCommand AddCommand { get; set; }
        public void Add()
        {
            if (string.IsNullOrWhiteSpace(NewGrade.Subject))
                return;
            if (string.IsNullOrWhiteSpace(NewGrade.Score))
                return;
            if (string.IsNullOrWhiteSpace(NewGrade.Credit))
                return;

            var item = new EduGradeItemModel();
            item.Subject = NewGrade.Subject;
            item.Score = NewGrade.Score;
            item.Credit = NewGrade.Credit;
            GradeChart.MainList.Add(item);

            //WeightScore = GradeChart.GetWeightAvarageScore();
            WeightScore = "...";
            GpaPoint = "...";
        }

        public DelegateCommand<object> DeleteCommand { get; set; }
        public void Delete(object param)
        {
            var model = param as EduGradeItemModel;
            if (model != null)
            {
                this.GradeChart.MainList.Remove(model);
                //WeightScore = GradeChart.GetWeightAvarageScore();
                WeightScore = "...";
                GpaPoint = "...";
            }
        }

        private Models.GradeChart _gradeChart;
        public Models.GradeChart GradeChart
        {
            get { return _gradeChart; }
            set { SetProperty(ref _gradeChart, value); }
        }



        #region 学年学期列表逻辑
        private string _weightScore;
        public string WeightScore
        {
            get { return _weightScore; }
            set { SetProperty(ref _weightScore, value); }
        }

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
            //WeightScore = GradeChart.GetWeightAvarageScore();
            WeightScore = "...";
            GpaPoint = "...";
        }
        public void cbTerm_SelectionChanged()
        {
            if (SelectedTermIndex == -1)
                return;
            var selectedYear = GradeChart.SchoolYearList[SelectedYearIndex];
            var selectedTerm = GradeChart.TermList[SelectedTermIndex];
            GradeChart.GetSpecificGradeChart(selectedYear, selectedTerm);

            WeightScore = "...";
            GpaPoint = "...";
        }

        public DelegateCommand CalcGradeWeightCommand { get; set; }
        public void CalcGradeWeight()
        {
            WeightScore = GradeChart.GetWeightAvarageScore();
        }
        #endregion

        #region GpaModel

        private ObservableCollection<GpaModel> _gpaModels;
        public ObservableCollection<GpaModel> GpaModels
        {
            get { return _gpaModels; }
            set { SetProperty(ref _gpaModels , value); }
        } 
        public  async Task<ObservableCollection<GpaModel>> LoadGpaMethod()
        {
            ObservableCollection<GpaModel> list=null;
            await Task.Run(() =>
            {
                list = new ObservableCollection<GpaModel>();

                GpaModel method = new GpaModel();
                method.Name = "北工大算法";
                method.Sections = new List<GpaModel.Section>();
                method.Sections.Add(new GpaModel.Section() { Score = 0, Point = 0 });
                method.Sections.Add(new GpaModel.Section() { Score = 60, Point = 2 });
                method.Sections.Add(new GpaModel.Section() { Score = 70, Point = 3 });
                method.Sections.Add(new GpaModel.Section() { Score = 85, Point = 4 });

                list.Add(method);

                method = new GpaModel();
                method.Name = "美国4分制GPA";
                method.Sections = new List<GpaModel.Section>();
                method.Sections.Add(new GpaModel.Section() { Score = 0, Point = 0 });
                method.Sections.Add(new GpaModel.Section() { Score = 60, Point = 1 });
                method.Sections.Add(new GpaModel.Section() { Score = 70, Point = 2 });
                method.Sections.Add(new GpaModel.Section() { Score = 80, Point = 3 });
                method.Sections.Add(new GpaModel.Section() { Score = 90, Point = 4 });

                list.Add(method);

                method = new GpaModel();
                method.Name = "北大GPA算法";
                method.Sections = new List<GpaModel.Section>();
                method.Sections.Add(new GpaModel.Section() { Score = 0, Point = 0 });
                method.Sections.Add(new GpaModel.Section() { Score = 60, Point = 1 });
                method.Sections.Add(new GpaModel.Section() { Score = 63, Point = 1.3 });
                method.Sections.Add(new GpaModel.Section() { Score = 66, Point = 1.7 });
                method.Sections.Add(new GpaModel.Section() { Score = 69, Point = 2.0 });
                method.Sections.Add(new GpaModel.Section() { Score = 72, Point = 2.3 });
                method.Sections.Add(new GpaModel.Section() { Score = 75, Point = 2.7 });
                method.Sections.Add(new GpaModel.Section() { Score = 78, Point = 3.0 });
                method.Sections.Add(new GpaModel.Section() { Score = 82, Point = 3.3 });
                method.Sections.Add(new GpaModel.Section() { Score = 85, Point = 3.7 });
                method.Sections.Add(new GpaModel.Section() { Score = 90, Point = 4.0 });

                list.Add(method);
            });
            return list;
        }

        private string _gpaPoint;
        public string GpaPoint
        {
            get { return _gpaPoint;}
            set { SetProperty(ref _gpaPoint , value); }
        }


        private int _gpaMethodIndex = -1;

        public int GpaMethodIndex
        {
            get { return _gpaMethodIndex; }
            set
            {
                if (value != _gpaMethodIndex)
                    GpaPoint = "...";
                SetProperty(ref _gpaMethodIndex , value);
                if (value != -1)
                {
                    GpaMethodDetail=GpaModels[value].GetMethodDetail();
                }
            }
        }

        private string _gpaMethodDetail;

        public string GpaMethodDetail
        {
            get { return _gpaMethodDetail; }
            set { SetProperty(ref _gpaMethodDetail , value); }
        }

        public void CalcGpa(int index)
        {
            var gpaMethod=GpaModels[index];
            var gpa=GradeChart.GetAvaPoint(gpaMethod);
            GpaPoint = gpa;
        }
        public DelegateCommand CalcGradeGpaCommand { get; set; }
        public void CalcGradeGpa()
        {
            if (GpaMethodIndex != -1)
            {
                CalcGpa(GpaMethodIndex);
            }
        }
        #endregion
    }
}
