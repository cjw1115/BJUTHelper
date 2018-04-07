
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace BJUTDUHelperXamarin.Services
{
    public class CampusCardService
    {
        private readonly string loginUri = "https://my.bjut.edu.cn/userPasswordValidate.portal";
        private readonly string campusCardLoginUri = "https://cwss.bjut.edu.cn:9090/smart_web/";
        private readonly string campusCardAjaxLoginUri = "https://cwss.bjut.edu.cn:9090/smart_web/ajax/login/sso";
        private readonly string cardSalaryInfoUri = "https://cwss.bjut.edu.cn:9090/smart_web/ajax/card/list.json";
        private readonly string cardBasicInfoUri = "https://cwss.bjut.edu.cn:9090/smart_web/ajax/person/getById.json";
        private readonly string personImageUri = "https://cwss.bjut.edu.cn:9090/smart_web/ajax/person/getImage";
        private readonly string campusCardTransactionUri = "https://cwss.bjut.edu.cn:9090/smart_web/ajax/tran/list.json";
        //登录信息门户

        public async Task LoginInfoCenter(Services.HttpBaseService _httpService, string username, string password)
        {
            try
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters.Add("Login.Token1", username);
                parameters.Add("Login.Token2", password);
                parameters.Add("goto", "http://my.bjut.edu.cn/loginSuccess.portal");
                parameters.Add("gotoOnFail", "http://my.bjut.edu.cn/loginFailure.portal");

                var re = await _httpService.SendRequst(loginUri, HttpMethod.Post, parameters);

                if (re.Contains("handleLoginSuccessed"))
                {
                    return;
                }
                else
                {
                    throw new InvalidUserInfoException("用户名或密码错误");
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
        public async Task GetCardCenterClient(Services.HttpBaseService _httpSerice)
        {
            try
            {
                var re = await _httpSerice.SendRequst(campusCardLoginUri, HttpMethod.Get);
                re = await _httpSerice.SendRequst(campusCardAjaxLoginUri, HttpMethod.Get);
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public async Task<Models.CampusCardInfoModel> GetCampusCardBasicInfo(Services.HttpBaseService _httpService)
        {
            var re = await _httpService.SendRequst(cardBasicInfoUri, HttpMethod.Get);
            var info = PraseCampusCardBasicInfo(re);
            //using (var stream = await _httpService.SendRequstForStream(personImageUri, HttpMethod.Get))
            //{
            //    stream.Seek(0, SeekOrigin.Begin);
            //    using (var ras = stream.AsRandomAccessStream())
            //    {
            //        WriteableBitmap bitmap = new WriteableBitmap(1, 1);
            //        bitmap.SetSource(ras);
            //        info.PersonImage = bitmap;
            //    }
            //}
            return info;
        }

        public async Task<Models.CampusCardInfoModel> GetCampusCardSalaryInfo(Services.HttpBaseService _httpService)
        {
            var re = await _httpService.SendRequst(cardSalaryInfoUri, HttpMethod.Get);
            var info = PraseCampusCardSalaryInfo(re);
            return info;
        }
        private Models.CampusCardInfoModel PraseCampusCardBasicInfo(string jsonStr)
        {
            Models.CampusCardInfoModel cardInfo = new Models.CampusCardInfoModel();
            var jsonObject = Newtonsoft.Json.Linq.JObject.Parse(jsonStr);
            var record = jsonObject["jsonData"];

            cardInfo.Username = (string)jsonObject["jsonData"]["jsmtSalaryno"];
            cardInfo.Name = (string)jsonObject["jsonData"]["smtName"];
            cardInfo.Gender = (string)jsonObject["jsonData"]["smtSex"];
            cardInfo.DepartmentName = (string)jsonObject["jsonData"]["smtDeptcodeTxt"];

            return cardInfo;

        }
        private Models.CampusCardInfoModel PraseCampusCardSalaryInfo(string jsonStr)
        {
            Models.CampusCardInfoModel cardInfo = new Models.CampusCardInfoModel();
            var jsonObject = Newtonsoft.Json.Linq.JObject.Parse(jsonStr);

            foreach (var item in jsonObject["jsonData"]["pageData"])
            {
                var record = item;
                cardInfo.balance = (double)record["balance"];
                cardInfo.smtAccounts = (string)record["smtAccounts"];
                cardInfo.smtDealdatetimeTxt = (string)record["smtDealdatetimeTxt"];
                cardInfo.smtCarddateTxt = (string)record["smtCarddateTxt"];
                cardInfo.smtEndcodeTxt =(string) record["smtEndcodeTxt"];
                cardInfo.smtShowcardno =(string) record["smtShowcardno"];
                cardInfo.smtValiditydateTxt = (string)record[" smtValiditydateTxt"];
                cardInfo.smtCardid = (string)record["smtCardid"];
            }

            return cardInfo;

        }
        public async Task<IList<Models.CampusCardTransactionItemModel>> GetTransactionInfo(Services.HttpBaseService _httpService)
        {
            try
            {
                #region 获取消费记录

                IDictionary<string, string> parameters = new Dictionary<string, string>();
                parameters.Add("int_start", "0");
                parameters.Add("int_maxSize", "15");
                var re = await _httpService.SendRequst(campusCardTransactionUri, HttpMethod.Post, parameters);
                var list = PraseTransactionInfo(re);
                return list;
                #endregion
            }
            catch
            {
                return null;
            }
        }
        public IList<Models.CampusCardTransactionItemModel> PraseTransactionInfo(string jsonStr)
        {
            var jsonObject = Newtonsoft.Json.Linq.JObject.Parse(jsonStr);

            List<Models.CampusCardTransactionItemModel> list = new List<Models.CampusCardTransactionItemModel>();
            foreach (var item in jsonObject["jsonData"]["pageData"])
            {
                var record = item;
                Models.CampusCardTransactionItemModel tansaction = new Models.CampusCardTransactionItemModel();
                tansaction.machineId = (string)record["machineId"];
                tansaction.smtDealDateTimeTxt = (string)record["smtDealDateTimeTxt"] ;
                tansaction.smtDealName = (string)record["smtDealName"];
                tansaction.smtOutMoney = (string)record["smtOutMoney"];
                tansaction.smtTransMoney = (string)record["smtTransMoney"];
                tansaction.smtOrgName = (string)record["smtOrgName"];
                list.Add(tansaction);
            }
            return list;
        }

        private readonly string reportLossUri = "https://cwss.bjut.edu.cn:9090/smart_web/ajax/card/modifyCardStatus.json?str_smtEndcode=2";
        private readonly string foundCampusUri = "https://cwss.bjut.edu.cn:9090/smart_web/ajax/card/modifyCardStatus.json?str_smtEndcode=0";
        public async Task<string> LostCampusCard(Services.HttpBaseService _httpService, string smtCardid)
        {
            try
            {
                var parameters = new Dictionary<string, string>();
                parameters.Add("lg_smtCardid", smtCardid);
                var re = await _httpService.SendRequst(reportLossUri, HttpMethod.Post, parameters);
                var json = Newtonsoft.Json.Linq.JObject.Parse(re);
                var messageObject = (string)json["message"];
                return string.IsNullOrWhiteSpace(messageObject) ? "挂失成功，请及时查找或者补办" : messageObject;
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
        public async Task<string> FundCampusCard(Services.HttpBaseService _httpService, string smtCardid)
        {
            try
            {
                var parameters = new Dictionary<string, string>();
                parameters.Add("lg_smtCardid", smtCardid);
                var re = await _httpService.SendRequst(foundCampusUri, HttpMethod.Post, parameters);
                var json = Newtonsoft.Json.Linq.JObject.Parse(re);
                var messageObject = (string)json["message"];
                return string.IsNullOrWhiteSpace(messageObject) ? "解挂成功，注意保存一卡通" : messageObject;

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
