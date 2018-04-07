using AngleSharp.Parser.Html;
using BJUTDUHelperXamarin.Models.NetModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BJUTDUHelperXamarin.Services
{
    public class WifiService
    {
        private readonly string messageToken = "1";
        private readonly string loginUri = "http://lgn.bjut.edu.cn/";
        private readonly string manageUri = "https://jfself.bjut.edu.cn/LoginAction.action";
        private readonly string checkcodeUri = "https://jfself.bjut.edu.cn/nav_login";
        private readonly string randomcodeUri = "https://jfself.bjut.edu.cn/RandomCodeAction.action?randomNum=0.124";
        private readonly string accountUri = "https://jfself.bjut.edu.cn/refreshaccount?t=1";
        private readonly string onlineUri = "https://jfself.bjut.edu.cn/nav_offLine";
        private readonly string logoutUri = "http://lgn.bjut.edu.cn/F.htm";
        public CancellationTokenSource cancellationTokenSource { get; set; }
        private HttpBaseService _httpService;

        private HttpBaseService _accountCenterHttpService;
        public WifiService(HttpBaseService httpService)
        {
            _httpService = httpService;
        }

        /// <summary>
        /// 注销
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Logout()
        {
            try
            {
                cancellationTokenSource = new CancellationTokenSource();
                await _httpService.SendRequst(logoutUri,HttpMethod.Get);
                return true;
            }
            catch (HttpRequestException requestException)
            {
                throw;
            }
        }

        /// <summary>
        /// 设置登录状态，检测登录页“F.htm”
        /// </summary>
        /// <returns></returns>
        public async Task<bool> GetRegistStatus()
        {
            try
            {
                var str = await _httpService.SendRequst(loginUri, HttpMethod.Get);
                if (str.Contains("F.htm"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                throw;
            }

        }

        /// 注册网关
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public async Task Register(string username, string password)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            StringBuilder bodyString = new StringBuilder();

            dic.Add("0MKKey", "");
            dic.Add("DDDDD", username);
            dic.Add("upass", password);

            var stringRespones = await _httpService.SendRequst(loginUri, HttpMethod.Post, dic);

            if (stringRespones.Contains("Msg=01"))
            {
                throw new InvalidUserInfoException("用户名或密码错误");
            }
            else if (stringRespones.Contains("Msg=06"))
            {
                throw new LoginTipException("喜闻乐见的system buffer full");
            }
            else if (stringRespones.Contains("Msg=05"))
            {
                throw new LoginTipException("本账号暂停使用");
            }
            else if (stringRespones.Contains("Msg=04"))
            {
                throw new LoginTipException("本账号费用超支或时长流量超过限制");
            }
            else if (stringRespones.Contains("UID='"))
            {
                return;
            }
            else
            {
                throw new LoginTipException("其他错误");
            }
        }

        public async Task<bool> LoginAccountCenter(string username,string password)
        {
            if (_accountCenterHttpService == null)
            {
                _accountCenterHttpService = new HttpBaseService();
            }
            string result = string.Empty;
            try
            {
                var re = await _accountCenterHttpService.SendRequst(checkcodeUri, HttpMethod.Get);
                int index = re.IndexOf("checkcode=\"");
                if (index <= 0)
                    return false;
                string checkcode = "";
                while (re[index + "checkcode=\"".Length] != '\"')
                {
                    checkcode += re[index + "checkcode=\"".Length];
                    index++;
                }

                //好像没啥意义。。
                await _accountCenterHttpService.SendRequst(randomcodeUri, HttpMethod.Get);

                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters.Add("account", username);
                parameters.Add("password", password);
                parameters.Add("checkcode", checkcode);
                result = await _accountCenterHttpService.SendRequst(manageUri, HttpMethod.Post, parameters);//登录管理中心 
               
            }
            catch
            {
                return false;
            }

            if (result.Contains("logout"))
            {
                return true;
            }
            if (result.Contains("errormsn"))
            {
                throw new InvalidUserInfoException("用户名或密码错误");
            }
            return false;

        }
        /// <summary>
        /// 获取账号的流量，余额等信息
        /// </summary>
        /// <param name="username"></param>
        /// <param name="passowrd"></param>
        public async Task<Models.InfoCenterAccountInfo> GetAccountBasicInfo(string username, string passowrd)
        {
            Models.InfoCenterAccountInfo accountinfo = new Models.InfoCenterAccountInfo();
            double allFlu = 0;
            double leftFlu = 0;
            try
            {
                var status=await LoginAccountCenter(username, passowrd);
                if (status == false)
                {
                    return accountinfo;
                }
                var re = await _accountCenterHttpService.SendRequst(accountUri, HttpMethod.Get);
                var json = Newtonsoft.Json.Linq.JObject.Parse(re);
                //dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(re);

                var totalFlu = (string)json["note"]["leftFlow"];
                var service = (string)json["note"]["service"];
                var leftmoney = (string)json["note"]["leftmoeny"];
                var flutype = (string)json["note"]["welcome"];


                accountinfo.TotalFlu = AutoAdjustUnit(double.Parse(totalFlu));

                allFlu = double.Parse(totalFlu);

                accountinfo.Balance = leftmoney;
                accountinfo.FluPackageType = flutype;
            }
            catch
            {
            }
            try
            {
                var re = await _accountCenterHttpService.SendRequst(loginUri, HttpMethod.Get);
                string usedflu = FindInHtml(re, "flow='", false);

                accountinfo.UsedFlu = AutoAdjustUnit(double.Parse(usedflu));

                leftFlu = allFlu - double.Parse(usedflu);
                
                if (leftFlu < 0)
                {
                    leftFlu = 0;
                    accountinfo.LeftFlu = 0 + "MB";
                }
                else
                {
                    accountinfo.LeftFlu = AutoAdjustUnit(leftFlu);
                }
                if (allFlu <= 0)
                {

                    accountinfo.FluPercent = 0;
                }
                else
                {
                    var value = leftFlu / allFlu;

                    accountinfo.FluPercent = value > 1 ? 1 : value ;
                }
            }
            catch
            {

            }
            return accountinfo;

        }
        public String AutoAdjustUnit(double value)
        {
            var gb = (int)value / 1024;
            value = value % 1024;
            var mb = value / 1024;
            return (gb + mb).ToString("#.#") + "GB";
        }
        public async  Task<List<OnlineAccountModel>> GetOnlineAccountAsync()
        {
            var models = new List<OnlineAccountModel>();
            try
            {
                
                var re = await _accountCenterHttpService.SendRequst(onlineUri, HttpMethod.Get);
                var parser = new HtmlParser();
                var document = parser.Parse(re);

                var emphasize = document.GetElementById("Maint1");
                var trs = emphasize.QuerySelectorAll("tbody tr");
                foreach (var tr in trs)
                {
                    var model = new OnlineAccountModel();
                    var tds = tr.QuerySelectorAll("td");
                    model.IPv4 = tds[0].TextContent.Trim();
                    model.IPv6 = tds[1].TextContent.Trim();
                    model.Mac = tds[2].TextContent.Trim();
                    model.SessionID = tds[3].TextContent.Trim();
                    models.Add(model);
                }
            }
            catch
            {

            }
            return models;
        }
        public async Task<bool> OfflineAsync(string sessionID)
        {
            try
            {
                var uri = $"https://jfself.bjut.edu.cn/tooffline?t={DateTime.Now.Millisecond.ToString()}&fldsessionid={sessionID}";
                var re = await _accountCenterHttpService.SendRequst(uri, HttpMethod.Get);
                //{ "date":"success","note":null,"outmessage":"true","serverDate":"2017-08-23"}
                if (re.Contains("success"))
                {
                    return true;

                }
                else
                {
                    return false;
                }
            }
            catch(Exception e)
            {
                return false;
            }
            
        }

        
        /// <summary>
        /// 计算流量余额
        /// </summary>
        /// <param name="strHtml"></param>
        /// <param name="strFlag"></param>
        /// <param name="isMoney"></param>
        /// <returns></returns>
        private string FindInHtml(string strHtml, string strFlag, bool isMoney)
        {
            ///使用量
            string strNum = null;

            int pos = strHtml.IndexOf(strFlag);
            int i = 0;
            while (strHtml[pos + strFlag.Length + i] >= '0' && strHtml[pos + strFlag.Length + i] <= '9')
            {
                strNum += strHtml[pos + strFlag.Length + i];
                i++;
            }
            int iNum = Convert.ToInt32(strNum);
            double fNum;
            fNum = ((double)iNum) / 1024;

            return Convert.ToDouble(fNum).ToString("0.000");
        }




        private readonly string bjutUri = "http://my.bjut.edu.cn/";
        private readonly string authUri = "https://wlgn.bjut.edu.cn/";
        private readonly string libPostUri = "http://172.24.39.253/portal/logon.cgi";
        private readonly string libLoginUri = "http://172.24.39.253/portal/logon.htm";

       
        /// <summary>
        /// 设置登录状态，检测登录页“F.htm”
        /// </summary>
        /// <returns></returns>
        public async Task<bool> GetAuthenStatus()
        {
            try
            {
                var code = await _httpService.GetResponseCode(bjutUri);
                if (code == System.Net.HttpStatusCode.OK)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                throw;
            }

        }
        public async Task Authenticate(string username, string password,string ssid)
        {
            cancellationTokenSource = new CancellationTokenSource();
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            
            switch (ssid)
            {
                case "bjut_wifi":
                    {
                        parameters.Clear();
                        parameters.Add("0MKKey", "123");
                        parameters.Add("DDDDD", username);
                        parameters.Add("upass", password);

                        var responseString = await _httpService.SendRequst(authUri, HttpMethod.Post, parameters, cancellation: cancellationTokenSource.Token);
                        #region 针对其他认证方式的结果判断逻辑

                        if (responseString.Contains("1.htm"))
                        {

                        }
                        else
                        {
                            int statusCode;
                            string flagString = "Msg=";
                            string status = string.Empty;
                            int index = responseString.IndexOf(flagString);
                            for (int i = index + flagString.Length + 1; i < index + flagString.Length + 10; i++)
                            {
                                var t = responseString[i];
                                if (t != ';')
                                    status += t;
                                else
                                    break;
                            }
                            statusCode = int.Parse(status);
                            if (statusCode == 1)
                            {
                                throw new InvalidUserInfoException("账号或密码不正确");
                            }
                            else if (statusCode == 2)
                            {
                                //GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("该账号正在使用中", messageToken);
                                throw new LoginTipException("该账号正在使用中");
                            }
                            else if (statusCode == 3)
                            {
                                throw new LoginTipException("用户名无效，可能已从本系统注销");
                                //GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("用户名无效，可能已从本系统注销", messageToken);
                            }
                            else if (statusCode == 4)
                            {
                                throw new LoginTipException("本账号费用超支或时长流量超过限制");
                                //GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("本账号费用超支或时长流量超过限制", messageToken);
                            }
                            else if (statusCode == 5)
                            {
                                throw new LoginTipException("本账号暂停使用");
                                //GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("本账号暂停使用", messageToken);
                            }
                            else if (statusCode == 15)
                            {
                                //throw new LoginTipException("本账号暂停使用");
                                //GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("认证成功", messageToken);
                            }
                        }
                        #endregion
                    }
                    break;
                case "Tushuguan":
                    {
                        parameters.Clear();
                        parameters.Add("PtUser", username);
                        parameters.Add("PtPwd", password);
                        parameters.Add("PtButton", "登录");


                        try
                        {
                            string responseString = await _httpService.SendRequst(libPostUri, HttpMethod.Post, cancellation: cancellationTokenSource.Token);


                            responseString = await _httpService.SendRequst(libLoginUri, HttpMethod.Get);

                            if (responseString.Contains("value='Logoff'"))
                            {


                            }
                            else
                            {
                                throw new InvalidUserInfoException("认证失败，请检查用户名或密码");
                                //GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("认证失败，请检查用户名或密码", messageToken);
                            }

                        }

                        catch (Exception ex)
                        {
                            //GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("认证失败", messageToken);
                            throw;
                        }
                    }
                    break;
                default:

                    break;
            }
        }

        public async Task<bool> GetIsBjutWifi()
        {
            var re=await _httpService.GetCanResponse(authUri);
            if (re == true)
            {
                return true;
            }
            else
            {
                re = await _httpService.GetCanResponse(libPostUri);
                return re;
            }
        }
    }
}
