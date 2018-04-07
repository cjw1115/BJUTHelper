using AngleSharp.Parser.Html;
using BJUTDUHelper.Model;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BJUTDUHelper.ViewModel
{
    public class BJUTEduExamVM:ViewModelBase
    {
        private readonly string messageToken = "1";
        private Service.HttpBaseService _httpService;
        private Model.BJUTEduCenterUserinfo BJUTEduCenterUserinfo { get; set; }

        private Service.BJUTEduCenterService _coreService;
        public BJUTEduExamVM()
        {
            _coreService = new Service.BJUTEduCenterService();
        }
        public async void Loaded(object parms)
        {
            if (parms != null)
            {
                View.EduCenterViewParam eduCenterViewParam = parms as View.EduCenterViewParam;
                _httpService = eduCenterViewParam.HttpService;
                BJUTEduCenterUserinfo = eduCenterViewParam.BJUTEduCenterUserinfo;
                
            }
            
        }
        public async void GetExamInfo()
        {
            string html = null;
            try
            {
                html = await _coreService.GetExamInfo(_httpService, ViewModel.BJUTEduCenterVM.Name, BJUTEduCenterUserinfo.Username);
            }
            catch (HttpRequestException requestException)
            {
                GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("获取数据失败", messageToken);
            }
            catch
            {
                GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("遇到错误/(ㄒoㄒ)/~~", messageToken);
                return;
            }

            try
            {
                ParseExamInfo(html);
            }
            catch
            {
                GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("解析数据失败", messageToken);

            }
        }



        public List<ExamModel> _examList;
        public List<ExamModel> ExamList
        {
            get { return _examList; }
            set { Set(ref _examList, value); }
        }
       
        private void ParseExamInfo(string html)
        {
            List<ExamModel> list = new
                   List<ExamModel>();

            var htmlParser = new HtmlParser();
            var doc = htmlParser.Parse(html);
            var table = doc.GetElementById("DataGrid1");
            var trs = table.QuerySelectorAll("tr");
            for (int i = 1; i < trs.Count(); i++)
            {
                ExamModel model = new ExamModel();
                var tds = trs[i].QuerySelectorAll("td");
                model.CourseName = tds[1].InnerHtml;
                model.Time = tds[3].InnerHtml;
                model.Address = tds[4].InnerHtml;
                list.Add(model);
            }
            ExamList = list;
        }
    }
}
