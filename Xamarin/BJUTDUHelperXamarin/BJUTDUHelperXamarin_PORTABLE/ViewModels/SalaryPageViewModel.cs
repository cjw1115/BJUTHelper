using BJUTDUHelperXamarin.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace BJUTDUHelperXamarin.ViewModels
{
    public class SalaryPageViewModel : BindableBase,INavigationAware
    {
        Services.DbService _deService;
        private Services.HttpBaseService _httpService;
        public SalaryPageViewModel()
        {
            _deService = new DbService();
            _httpService = new Services.HttpBaseService();
            QueryCommand = new DelegateCommand(Query);
            Salaries = new ObservableCollection<Models.SalaryModel>();
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name , value); }
        }

        private string _studentID;
        public string StudentID
        {
            get { return _studentID; }
            set { SetProperty(ref _studentID , value); }
        }

        private ObservableCollection<Models.SalaryModel> _salaries;
        public ObservableCollection<Models.SalaryModel> Salaries
        {
            get { return _salaries; }
            set { SetProperty(ref _salaries, value); }
        }
        public ICommand QueryCommand { get; set; }
        public async void Query()
        {
            if (string.IsNullOrWhiteSpace(StudentID))
            {
                Services.NotityService.Notify("请输入学号");
                return;
            }
            if (string.IsNullOrWhiteSpace(Name))
            {
                Services.NotityService.Notify("请输入姓名");
                return;
            }
            IsLoading = true;
            try
            {
                var re = await Services.MyBjutService.QuerySalaries(_httpService, StudentID, Name);
                if (re == null || re.Count == 0)
                {
                    Services.NotityService.Notify("没有查询到相关的信息");
                    Salaries.Clear();
                  
                    Total = $"没有相关的信息";
                    return;
                }
                Salaries.Clear();
                foreach (var item in re)
                {
                    Salaries.Add(item);
                }

                var salary = Salaries.Aggregate(0d, (sum, m) => { return sum + m.Salary; });
                Total = $"参与零工 {Salaries.Count} 次，共收入 {salary } 元";
            }
            catch(Exception e)
            {
                Services.NotityService.Notify(e.Message);
                return;
            }
            finally
            {
                IsLoading = false;
            }
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
              
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            var infouser = _deService.GetAll<Models.BJUTInfoCenterUserinfo>().FirstOrDefault();
            var eduuser = _deService.GetAll<Models.BJUTInfoCenterUserinfo>().FirstOrDefault();
            if (infouser == null&&eduuser!=null)
            {
                StudentID = eduuser.Username;
            }
            else if(infouser != null && eduuser == null)
            {
                StudentID = infouser.Username;
            }
            else if(infouser != null && eduuser != null)
            {
                StudentID = infouser.Username == eduuser.Username ? infouser.Username : string.Empty;
            }
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
            
        }

        private string _total;

        public string Total
        {
            get { return _total; }
            set { SetProperty(ref _total ,value); }
        }

        private bool _isLoading = false;

        public bool IsLoading
        {
            get { return _isLoading; }
            set { SetProperty(ref _isLoading , value); }
        }

    }
}
