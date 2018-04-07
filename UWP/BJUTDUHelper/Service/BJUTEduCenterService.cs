using BJUTDUHelper.BJUTDUHelperlException;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace BJUTDUHelper.Service
{
    public class BJUTEduCenterService
    {
        public readonly string checckAuthUri = "http://gdjwgl.bjut.edu.cn/xs_main.aspx?xh=";
        public readonly string checkCodeUri = "http://gdjwgl.bjut.edu.cn/CheckCode.aspx";
        public readonly string educenterUri = "http://gdjwgl.bjut.edu.cn/default2.aspx";
        public readonly string calendarUri = "http://undergrad.bjut.edu.cn/CalendarFile/CalendarBig.aspx";
        public readonly string edutimeUri = "http://www.cjw1115.com/BJUTDUHelper/getedutime";

        //获取验证码
        public async Task<ImageSource> GetCheckCode(Service.HttpBaseService _httpService)
        {
            Stream stream = null;
            try
            {
                stream = await _httpService.SendRequstForStream(checkCodeUri, HttpMethod.Get);
                stream.Seek(0, SeekOrigin.Begin);
                byte[] byteBuffer = new byte[stream.Length];
                await stream.ReadAsync(byteBuffer, 0, byteBuffer.Length);

                var source = await Helper.ImageTool.SaveToImageSource(byteBuffer);
                return source;
                //BitmapImage bitmap = new BitmapImage();
                ////using (MemoryStream mem=new MemoryStream())
                ////{
                ////    await stream.CopyToAsync(mem);
                ////    var ras=mem.AsRandomAccessStream();
                ////    bitmap.SetSource(ras);
                ////}
                //await bitmap.SetSourceAsync(stream.AsRandomAccessStream());
                //CheckCodeSource = bitmap;

            }
            catch
            {
                return null;
            }


        }
        //登录教务管理中心
        public async Task<string> LoginEduCenter(Service.HttpBaseService _httpService,string username, string password, string checkCode)
        {
            try
            {
                var str = await _httpService.SendRequst(educenterUri, HttpMethod.Get);
                var __VIEWSTATEString = Service.BJUTEduCenterService.GetViewstate(str);
                if (__VIEWSTATEString == "")
                {
                    return null;
                }
                var validation= Service.BJUTEduCenterService.GetValidation(str);
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


                var reStr = await _httpService.SendRequst("http://gdjwgl.bjut.edu.cn/default2.aspx", HttpMethod.Post, parameters);
                var name = Service.BJUTEduCenterService.GetName(reStr);
                if (string.IsNullOrEmpty(name))
                {
                    string messageRegex = @"(?<=defer\>alert\(')\w.+(?='\);)";
                    var message=Regex.Match(reStr, messageRegex).Value;
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
            catch(Exception ex)
            {
                throw;
            }
        }
        //获取学年学期信息
        public async Task<Model.EduTimeModel> GetEduBasicInfo(Service.HttpBaseService _httpService)
        {
            var re = await _httpService.SendRequst(calendarUri, HttpMethod.Get);
            var p = Regex.Match(re, @"<.*weekteaching.*\s*.*\s*</p>").Value;
            var year = Regex.Match(p, @"\d+-\d+").Value;
            var term = Regex.Match(p, @".(?=学期)").Value == "二" ? 2 : 1;
            var week = Regex.Match(p, @"\d*(?=\s*教学)").Value;

            Model.EduTimeModel model = new Model.EduTimeModel();
            model.SchoolYear = year;
            model.Term = term;
            model.Week = int.Parse(week);
            return model;
        }
        public async Task<Model.EduTimeModel> GetEduTime(Service.HttpBaseService _httpService)
        {
            var re = await _httpService.SendRequst(edutimeUri, HttpMethod.Get);
            var model=Newtonsoft.Json.JsonConvert.DeserializeObject<Model.EduTimeModel>(re);
            return model;
        }
        
        //从页面解析用户姓名
        public static string GetName(string html)
        {
            var nameRegex= "(?<=id\\=\"xhxm\"\\>)\\w+(?=\\</span\\>)";
            var  name=Regex.Match(html, nameRegex).Value;

            //string Name = "";
            //string specificStr = "xhxm\">";
            //int start = html.IndexOf(specificStr);
            //if (start <= 0)
            //    return Name;
            //int i = 0;

            //while (html[start + specificStr.Length + i] != '同')
            //{
            //    Name += html[start + specificStr.Length + i++];
            //}
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
            var re=Regex.Match(html, regex1).ToString();
            return Regex.Match(re, regex2).ToString();
        }

        //检测是否能链接到教务系统
        public async Task<bool> GetConnectedStatus(Service.HttpBaseService _httpService )
        {
            try
            {
                var re = await _httpService.GetResponseCode(educenterUri);
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
        public async Task<bool> GetAuthState(Service.HttpBaseService _httpService,string username)
        {
           
            var re = await _httpService.GetResponseCode(checckAuthUri + username);
            if (re == System.Net.HttpStatusCode.OK)
            {
                return true;
            }
            return false;
        }

        //获取考试信息
        public async Task<string> GetExamInfo(Service.HttpBaseService _httpService, string name, string username)
        {
            //http://gdjwgl.bjut.edu.cn/xskscx.aspx?xh=14024238&xm=%B3%C2%BC%D1%CE%C0&gnmkdm=N121604
            string re;
            re = await _httpService.SendRequst("http://gdjwgl.bjut.edu.cn/xskscx.aspx?xh=" + username + "&xm=" + name + "&gnmkdm=N121604", HttpMethod.Get, referUri: "http://gdjwgl.bjut.edu.cn/xs_main.aspx?xh=" + username);
            return re;
        }

        //获取成绩
        public async Task<string> GetGrade(Service.HttpBaseService _httpService,string name, string username)
        { 
            string re;
            re = await _httpService.SendRequst("http://gdjwgl.bjut.edu.cn/xscjcx.aspx?xh=" + username + "&xm=" + name + "&gnmkdm=N121605", HttpMethod.Get, referUri: "http://gdjwgl.bjut.edu.cn/xscjcx.aspx?xh=" + username + "&xm=" + "" + "&gnmkdm=N121605");


            string __VIEWSTATEString;
            __VIEWSTATEString = Service.BJUTEduCenterService.GetViewstate(re);
            var __EVENTVALIDATION = Service.BJUTEduCenterService.GetValidation(re);

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

            re = await _httpService.SendRequst("http://gdjwgl.bjut.edu.cn/xscjcx.aspx?xh=" + username + "&xm=" + "" + "&gnmkdm=N121605", HttpMethod.Post, parameters);

            return re;
                
        }

        //获取课程表数据
        public async Task<string> GetCurrentSchedule(Service.HttpBaseService _httpService, string name, string username)
        {
            string re;
            re = await _httpService.SendRequst("http://gdjwgl.bjut.edu.cn/xskbcx.aspx?xh=" + username + "&xm=" + name + "&gnmkdm=N121603", HttpMethod.Get, referUri: "http://gdjwgl.bjut.edu.cn/xs_main.aspx?xh=" + username);
            return re;
        }

        public async Task<string> GetSpecificSchedule(Service.HttpBaseService _httpService, string name, string username, string year, int term)
        {

            string re;
            re = await _httpService.SendRequst("http://gdjwgl.bjut.edu.cn/xskbcx.aspx?xh=" + username + "&xm=" + name + "&gnmkdm=N121603", HttpMethod.Get, referUri: "http://gdjwgl.bjut.edu.cn/xs_main.aspx?xh=" + username);
            //re = await _httpService.SendRequst("http://gdjwgl.bjut.edu.cn/xs_main.aspx?xh=" + username, HttpMethod.Get);


            string __VIEWSTATEString;
            __VIEWSTATEString = Service.BJUTEduCenterService.GetViewstate(re);
            var __EVENTVALIDATION = Service.BJUTEduCenterService.GetValidation(re);
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("__EVENTVALIDATION", __EVENTVALIDATION);
            parameters.Add("__EVENTTARGET", "");
            parameters.Add("__EVENTARGUMENT", "");
            parameters.Add("__VIEWSTATE", __VIEWSTATEString);
            parameters.Add("xnd", year);
            parameters.Add("xqd", term.ToString());

            re = await _httpService.SendRequst("http://gdjwgl.bjut.edu.cn/xskbcx.aspx?xh=" + username + "&xm=" + name + "&gnmkdm=N121603", HttpMethod.Post, parameters);

            return re;
        }
    }

}
