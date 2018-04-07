
using BJUTDUHelperXamarin.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BJUTDUHelperXamarin.Services
{
    public class EduService: ILogin
    {   
        public string baseUri = "http://gdjwgl.cjw1115.com/";
        public string bjut = "http://gdjwgl.bjut.edu.cn/";
        public string experimentalUri = "http://jw.pcbjut.cn/(airwhw45l3fflyezmpb1nofl)/";

        public readonly  string checckAuthUri = "xs_main.aspx?xh=";
        public readonly  string checkCodeUri = "CheckCode.aspx";
        public readonly string educenterUri = "default2.aspx";
        public readonly string eduexamUri = "xskscx.aspx?xh=";
        public readonly string edugradeUri = "xscjcx.aspx?xh=";
        public readonly string eduscheduleUri = "xskbcx.aspx?xh=";
        public  string calendarUri = "http://undergrad.bjut.edu.cn/CalendarFile/CalendarBig.aspx";
        public static readonly  string edutimeUri = "https://www.cjw1115.com/BJUTDUHelper/getedutime";
        public EduService()
        {
        }

        public string GetBaseUri()
        {
            if (Models.Settings.EduExperimentalSetting == true)
            {
                return experimentalUri;
            }
            if (Models.Settings.EduProxySetting == true)
            {
                return baseUri ;
            }
            else
            {
                return bjut;
            }
        }
        //获取验证码
        public async Task<ImageSource> GetCheckCode(Services.HttpBaseService _httpService)
        {
            Stream stream = null;
            try
            {
                stream = await _httpService.SendRequstForStream(GetBaseUri()+checkCodeUri, HttpMethod.Get);
                var iamgesource=ImageSource.FromStream(() => { return stream; });
                return iamgesource;
            }
            catch
            {
                return null;
            }
        }
        //登录教务管理中心
        public async Task<string> LoginEduCenter(Services.HttpBaseService _httpService, string username, string password, string checkCode)
        {
            try
            {
                var str = await _httpService.SendRequst(GetBaseUri() + educenterUri, HttpMethod.Get);
                var __VIEWSTATEString = Services.EduService.GetViewstate(str);
                if (__VIEWSTATEString == "")
                {
                    return null;
                }
                var validation = Services.EduService.GetValidation(str);
                if (validation == null)
                    return null;
                IDictionary<string, string> parameters = new Dictionary<string, string>();
                parameters.Add("__VIEWSTATE", __VIEWSTATEString);
                parameters.Add("__EVENTVALIDATION", validation);
                parameters.Add("txtUserName", username);
                parameters.Add("TextBox2", password);
                parameters.Add("txtSecretCode", checkCode);
                parameters.Add("RadioButtonList1", "学生");
                parameters.Add("Button1", "");
                parameters.Add("lbLanguage", "");
                parameters.Add("hidPdrs", "");
                parameters.Add("hidsc", "");


                var reStr = await _httpService.SendRequst(GetBaseUri() + educenterUri, HttpMethod.Post, parameters);
                var name = Services.EduService.GetName(reStr);
                if (string.IsNullOrEmpty(name))
                {
                    string messageRegex = @"(?<=defer\>alert\(')\w.+(?='\);)";
                    var message = Regex.Match(reStr, messageRegex).Value;
                    if (message.Contains("验证"))
                    {
                        throw new InvalidCheckcodeException(message);
                    }
                    else
                    {
                        throw new InvalidUserInfoException(message);
                    }
                }
                return name;

            }
            catch (HttpRequestException requestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        //获取学年学期信息
        public async Task<Models.EduTimeModel> GetEduBasicInfo(Services.HttpBaseService _httpService)
        {
            var re = await _httpService.SendRequst(calendarUri, HttpMethod.Get);
            var p = Regex.Match(re, @"<.*weekteaching.*\s*.*\s*</p>").Value;
            var year = Regex.Match(p, @"\d+-\d+").Value;
            var term = Regex.Match(p, @".(?=学期)").Value == "二" ? 2 : 1;
            var week = Regex.Match(p, @"\d*(?=\s*教学)").Value;

            Models.EduTimeModel model = new Models.EduTimeModel();
            model.SchoolYear = year;
            model.Term = term;
            model.Week = int.Parse(week);
            return model;
        }
        public static async Task<Models.EduTimeModel> GetEduTime(Services.HttpBaseService _httpService)
        {
            var re = await _httpService.SendRequst(edutimeUri, HttpMethod.Get);
            var model = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.EduTimeModel>(re);
            return model;
        }

        //从页面解析用户姓名
        public static string GetName(string html)
        {
            var nameRegex = "(?<=id\\=\"xhxm\"\\>)\\w+(?=\\</span\\>)";
            var name = Regex.Match(html, nameRegex).Value;
            return name;
        }
        //获取ViewState
        public static string GetViewstate(string html)
        {
            string specifcStr = "__VIEWSTATE\" value=\"";
            string goal = "";
            int start = html.IndexOf("__VIEWSTATE\" value=\"");
            if (start <= 0)
                return goal;
            int i = 0;
            while (html[start + specifcStr.Length + i] != '"')
            {
                goal += html[start + specifcStr.Length + i++];
            }
            return goal;
        }
        //获取验证信息
        public static string GetValidation(string html)
        {
            string regex1 = @"EVENTVALIDATION([\w\W]*)/>";
            string regex2 = "(?<=value=\")([\\S]*)(?=\")";
            var re = Regex.Match(html, regex1).ToString();
            return Regex.Match(re, regex2).ToString();
        }

        //检测是否能链接到教务系统
        public async Task<bool> GetConnectedStatus(Services.HttpBaseService _httpService)
        {
            try
            {
                var re = await _httpService.GetResponseCode(GetBaseUri() + educenterUri);
                if (re == System.Net.HttpStatusCode.OK)
                {
                    return true;
                }
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        //检测是否已经认证过教务系统
        public async Task<bool> GetAuthState(Services.HttpBaseService _httpService, string username)
        {

            var re = await _httpService.GetResponseCode(GetBaseUri() + checckAuthUri + username);
            if (re == System.Net.HttpStatusCode.OK)
            {
                return true;
            }
            return false;
        }

        //获取考试信息
        public async Task<string> GetExamInfo(Services.HttpBaseService _httpService, string name, string username)
        {
            
            string re;
            re = await _httpService.SendRequst(GetBaseUri() + eduexamUri + username + "&xm=" + name + "&gnmkdm=N121604", HttpMethod.Get, referUri: GetBaseUri() + checckAuthUri + username);
            return re;
        }

        //获取成绩
        public async Task<string> GetGrade(Services.HttpBaseService _httpService, string name, string username)
        {
            //http://gdjwgl.cjw1115.com/xscjcx.aspx?xh=14024238&xm=%B3%C2%BC%D1%CE%C0&gnmkdm=N121605
            string re;
            re = await _httpService.SendRequst(GetBaseUri() + edugradeUri + username + "&xm=" + name + "&gnmkdm=N121605", HttpMethod.Get, referUri: GetBaseUri() + edugradeUri + username + "&xm=" + "" + "&gnmkdm=N121605");


            string __VIEWSTATEString;
            __VIEWSTATEString = Services.EduService.GetViewstate(re);
            var __EVENTVALIDATION = Services.EduService.GetValidation(re);

            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("__EVENTTARGET", "");
            parameters.Add("__EVENTVALIDATION", __EVENTVALIDATION);
            parameters.Add("__EVENTARGUMENT", "");
            parameters.Add("__VIEWSTATE", __VIEWSTATEString);
            parameters.Add("hidLanguage", "");
            parameters.Add("ddlXN", "");
            parameters.Add("ddlXQ", "");
            parameters.Add("ddl_kcxz", "");
            parameters.Add("btn_zcj", "历年成绩");

            re = await _httpService.SendRequst(GetBaseUri() + edugradeUri + username + "&xm=" + name + "&gnmkdm=N121605", HttpMethod.Post, parameters);

            return re;

        }

        //获取课程表数据
        public async Task<string> GetCurrentSchedule(Services.HttpBaseService _httpService, string name, string username)
        {
            string re;
            re = await _httpService.SendRequst(GetBaseUri() + eduscheduleUri + username + "&xm=" + name + "&gnmkdm=N121603", HttpMethod.Get, referUri: GetBaseUri() + checckAuthUri + username);
            return re;
        }

        public async Task<string> GetSpecificSchedule(Services.HttpBaseService _httpService, string name, string username, string year, int term)
        {
            
            string re;
            re = await _httpService.SendRequst(GetBaseUri() + eduscheduleUri + username + "&xm=" + name + "&gnmkdm=N121603", HttpMethod.Get, referUri: GetBaseUri() + checckAuthUri + username);

            string __VIEWSTATEString;
            __VIEWSTATEString = Services.EduService.GetViewstate(re);
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("__EVENTTARGET", "xnd");
            parameters.Add("__EVENTARGUMENT", "");
            parameters.Add("__VIEWSTATE", __VIEWSTATEString);
            parameters.Add("xnd", year);
            parameters.Add("xqd", "2");

            re = await _httpService.SendRequst(GetBaseUri() + eduscheduleUri + username + "&xm=" + name + "&gnmkdm=N121603", HttpMethod.Post, parameters);

            if (term == 1)
            {
                __VIEWSTATEString = Services.EduService.GetViewstate(re);
                
                parameters["__EVENTTARGET"]= "xqd";
                parameters["__VIEWSTATE"]= __VIEWSTATEString;
                parameters["xqd"]= "1";

                re = await _httpService.SendRequst(GetBaseUri() + eduscheduleUri + username + "&xm=" + name + "&gnmkdm=N121603", HttpMethod.Post, parameters);
            }
            return re;
        }

        public static bool IsExperimental(string username)
        {
            //实验学院 52 54 57 61
            try
            {
                var codes = new string[] { "52", "54", "57", "61" };
                var code = username.Substring(2, 2);
                foreach (var item in codes)
                {
                    if (item.Contains(code))
                    {
                        return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }

}
