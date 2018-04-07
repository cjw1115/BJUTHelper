using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BJUTDUHelper.BJUTDUHelperlException;
using Windows.Data.Json;
using System.IO;
using Windows.UI.Xaml.Media.Imaging;

namespace BJUTDUHelper.Service
{
    public class BJUTCampusCardService
    {
        private readonly string loginUri = "https://my.bjut.edu.cn/userPasswordValidate.portal";
        private readonly string campusCardLoginUri = "https://cwss.bjut.edu.cn:9090/smart_web/";
        private readonly string campusCardAjaxLoginUri= "https://cwss.bjut.edu.cn:9090/smart_web/ajax/login/sso";
        private readonly string cardSalaryInfoUri = "https://cwss.bjut.edu.cn:9090/smart_web/ajax/card/list.json";
        private readonly string cardBasicInfoUri = "https://cwss.bjut.edu.cn:9090/smart_web/ajax/person/getById.json";
        private readonly string personImageUri = "https://cwss.bjut.edu.cn:9090/smart_web/ajax/person/getImage";
        private readonly string campusCardTransactionUri = "https://cwss.bjut.edu.cn:9090/smart_web/ajax/tran/list.json";
        //登录信息门户
       
        public  async Task LoginInfoCenter( Service.HttpBaseService _httpService,string username, string password)
        {
            try
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters.Add("Login.Token1", username);
                parameters.Add("Login.Token2", password);
                parameters.Add("goto", "http://my.bjut.edu.cn/loginSuccess.portal");
                parameters.Add("gotoOnFail", "http://my.bjut.edu.cn/loginFailure.portal");

                var re=await _httpService.SendRequst(loginUri, HttpMethod.Post, parameters);
               
                if (re.Contains("handleLoginSuccessed"))
                {
                }
                else
                {
                    throw new BJUTDUHelperlException.InvalidUserInfoException("用户名或密码错误");
                    //登录失败
                }

            }
            catch (Exception e)
            {
                throw;
                //其他异常
            }
        }
        //获取信息门户到一卡通中心的filter
        public  async Task  GetCardCenterClient(Service.HttpBaseService _httpSerice)
        {
            try
            {
                var re =await  _httpSerice.SendRequst(campusCardLoginUri, HttpMethod.Get);
                re = await _httpSerice.SendRequst(campusCardAjaxLoginUri, HttpMethod.Get);
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public async Task<Model.CampusCardInfoModel> GetCampusCardBasicInfo(Service.HttpBaseService _httpService)
        {
            var re = await _httpService.SendRequst(cardBasicInfoUri, HttpMethod.Get);
            var info = PraseCampusCardBasicInfo(re);
            using (var stream = await _httpService.SendRequstForStream(personImageUri, HttpMethod.Get))
            {
                stream.Seek(0, SeekOrigin.Begin);
                using (var ras = stream.AsRandomAccessStream())
                {
                    WriteableBitmap bitmap = new WriteableBitmap(1,1);
                    bitmap.SetSource(ras);
                    info.PersonImage = bitmap;
                }
            }
            return info;
        }

        public async Task<Model.CampusCardInfoModel> GetCampusCardSalaryInfo(Service.HttpBaseService _httpService)
        {
            var re = await _httpService.SendRequst(cardSalaryInfoUri, HttpMethod.Get);
            var info= PraseCampusCardSalaryInfo(re);
            return info;
        }
        private Model.CampusCardInfoModel PraseCampusCardBasicInfo(string jsonStr)
        {
            Model.CampusCardInfoModel cardInfo = new Model.CampusCardInfoModel();
            JsonObject jsonObject = JsonObject.Parse(jsonStr);
            var record = jsonObject["jsonData"].GetObject();
                if (cardInfo == null)
                cardInfo = new Model.CampusCardInfoModel();
            cardInfo.Username = record.GetNamedString("smtSalaryno");
            cardInfo.Name = record.GetNamedString("smtName");
            cardInfo.Gender = record.GetNamedString("smtSex");
            cardInfo.DepartmentName = record.GetNamedString("smtDeptcodeTxt");

            return cardInfo;

        }
        private Model.CampusCardInfoModel PraseCampusCardSalaryInfo(string jsonStr)
        {
            Model.CampusCardInfoModel cardInfo = new Model.CampusCardInfoModel();
            JsonObject jsonObject = JsonObject.Parse(jsonStr);


            foreach (var item in jsonObject["jsonData"].GetObject()["pageData"].GetArray())
            {
                var record = item.GetObject();
                if (cardInfo == null)
                    cardInfo = new Model.CampusCardInfoModel(); 
                cardInfo.balance = record.GetNamedNumber("balance");
                cardInfo.smtAccounts = record.GetNamedString("smtAccounts");
                cardInfo.smtDealdatetimeTxt = record.GetNamedString("smtDealdatetimeTxt");
                cardInfo.smtCarddateTxt = record.GetNamedString("smtCarddateTxt");
                cardInfo.smtEndcodeTxt = record.GetNamedString("smtEndcodeTxt");
                cardInfo.smtShowcardno = record.GetNamedString("smtShowcardno");
                cardInfo.smtValiditydateTxt = record.GetNamedString("smtValiditydateTxt");
                cardInfo.smtCardid= record.GetNamedNumber("smtCardid").ToString();
            }

            return cardInfo;

        }
        public async Task<IList<Model.CampusCardTransactionItemModel>> GetTransactionInfo(Service.HttpBaseService _httpService)
        {
            try
            {
                #region 获取消费记录
                
                IDictionary<string, string> parameters = new Dictionary<string, string>();
                parameters.Add("int_start", "0");
                parameters.Add("int_maxSize", "15");
                var re =await  _httpService.SendRequst(campusCardTransactionUri, HttpMethod.Post, parameters); 
                var list=PraseTransactionInfo(re);
                return list;
                #endregion
            }
            catch
            {
                return null;
            }
        }
        public IList<Model.CampusCardTransactionItemModel> PraseTransactionInfo(string jsonStr)
        {
            JsonObject jsonObject = JsonObject.Parse(jsonStr);

            List<Model.CampusCardTransactionItemModel> list = new List<Model.CampusCardTransactionItemModel>();
            foreach (var item in jsonObject["jsonData"].GetObject()["pageData"].GetArray())
            {
                var record = item.GetObject();
                Model.CampusCardTransactionItemModel tansaction = new Model.CampusCardTransactionItemModel();
                tansaction.machineId = record.GetNamedString("machineId");
                tansaction.smtDealDateTimeTxt = record.GetNamedString("smtDealDateTimeTxt");
                tansaction.smtDealName = record.GetNamedString("smtDealName");
                tansaction.smtOutMoney = record.GetNamedNumber("smtOutMoney").ToString();
                tansaction.smtTransMoney = record.GetNamedNumber("smtTransMoney").ToString();
                tansaction.smtOrgName = record.GetNamedString("smtOrgName");
                list.Add(tansaction);
            }
            return list;
        }

        private readonly string reportLossUri = "https://cwss.bjut.edu.cn:9090/smart_web/ajax/card/modifyCardStatus.json?str_smtEndcode=2";
        private readonly string foundCampusUri = "https://cwss.bjut.edu.cn:9090/smart_web/ajax/card/modifyCardStatus.json?str_smtEndcode=0";
        public async Task<string> LostCampusCard(Service.HttpBaseService _httpService, string smtCardid)
        {
            try
            {
                var parameters = new Dictionary<string, string>();
                parameters.Add("lg_smtCardid", smtCardid);
                var re = await _httpService.SendRequst(reportLossUri, HttpMethod.Post, parameters);
                var json=JsonObject.Parse(re);
                var messageObject = json["message"];
                return messageObject.ValueType == JsonValueType.Null ? "挂失成功，请及时查找或者补办" : messageObject.GetString();
            }
            catch (HttpRequestException requestException)
            {
                return "网络错误";
            }
            catch
            {
                return "其他错误";
            }
        }
        public async Task<string> FundCampusCard(Service.HttpBaseService _httpService, string smtCardid)
        {
            try
            {
                var parameters = new Dictionary<string, string>();
                parameters.Add("lg_smtCardid", smtCardid);
                var re = await _httpService.SendRequst(foundCampusUri, HttpMethod.Post, parameters);
                var json = JsonObject.Parse(re);
                var messageObject = json["message"];
                return messageObject.ValueType == JsonValueType.Null ?"解挂成功，注意保存一卡通":messageObject.GetString();
                
            }
            catch (HttpRequestException requestException)
            {
                return "网络错误";
            }
            catch
            {
                return "其他错误";
            }
        }
    }
}
