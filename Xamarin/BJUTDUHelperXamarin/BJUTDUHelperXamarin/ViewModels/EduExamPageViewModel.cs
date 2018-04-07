using AngleSharp.Parser.Html;
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

namespace BJUTDUHelperXamarin.ViewModels
{
    public class EduExamPageViewModel : BindableBase,INavigationAware
    {
        private Services.HttpBaseService _httpService;
        private Services.EduService _coreService;
        private Services.DbService _dbService;
        private Models.BJUTEduCenterUserinfo BJUTEduCenterUserinfo { get; set; }
        private string _studentName;

        public EduExamPageViewModel()
        {
            //_dbService = new Services.DbService();
            _dbService = new DbService();
            _coreService = new Services.EduService();
        }
        public void OnNavigatedFrom(NavigationParameters parameters)
        {
            
        }

        public async void OnNavigatedTo(NavigationParameters parameters)
        {
            if (parameters != null)
            {

                _coreService = EduPageViewModel.CoreService;
                _httpService = EduPageViewModel.HttpService;
                _studentName = EduPageViewModel.Name;

                BJUTEduCenterUserinfo = _dbService.GetAll<Models.BJUTEduCenterUserinfo>().FirstOrDefault();

                IsLoading = true;
                await GetExamInfo();
                IsLoading = false;
            }
        }

        private bool _isLoading = false;

        public bool IsLoading
        {
            get { return _isLoading; }
            set { SetProperty(ref _isLoading, value); }
        }

        public async Task GetExamInfo()
        {
            string html = null;
            try
            {
                html = await _coreService.GetExamInfo(_httpService, _studentName, BJUTEduCenterUserinfo.Username);
            }
            catch (HttpRequestException requestException)
            {
                Services.NotityService.Notify("获取数据失败");
                
            }
            catch
            {
                Services.NotityService.Notify("遇到错误/(ㄒoㄒ)/~~");
                return;
            }

            try
            {
                ExamList = await ParseExamInfo(html);
            }
            catch
            {
                Services.NotityService.Notify("解析数据失败/(ㄒoㄒ)/~~");
            }
        }



        public List<EduExamModel> _examList;
        public List<EduExamModel> ExamList
        {
            get { return _examList; }
            set { SetProperty(ref _examList, value); }
        }

        private async Task<List<EduExamModel>> ParseExamInfo(string html)
        {
            List<EduExamModel> list = new
                   List<EduExamModel>();

            var htmlParser = new HtmlParser();
            var doc = htmlParser.Parse(html);
            var table = doc.GetElementById("DataGrid1");
            var trs = table.QuerySelectorAll("tr");
            for (int i = 1; i < trs.Count(); i++)
            {
                EduExamModel model = new EduExamModel();
                var tds = trs[i].QuerySelectorAll("td");
                model.CourseName = tds[1].InnerHtml;
                model.Time = tds[3].InnerHtml;
                model.Address = tds[4].InnerHtml;
                list.Add(model);
            }
            return list;
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
            
        }
    }
}
