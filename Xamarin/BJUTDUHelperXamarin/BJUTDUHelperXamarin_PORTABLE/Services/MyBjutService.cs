using BJUTDUHelperXamarin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BJUTDUHelperXamarin.Services
{
    public class MyBjutService
    {
        private static readonly string querySalaryUri = "https://cjw1115.com/zzzx/query";
        private static readonly string bookingGradeUri = "https://bjuthelper.cjw1115.com/api/grade";
        public static async Task<IList<Models.SalaryModel>> QuerySalaries(HttpBaseService _httpService,string studentID,string name)
        {
            string re = null;
            try
            {
                var dic = new Dictionary<string, string>();
                dic.Add("StudentID", studentID);
                dic.Add("Name", name);
                re= await _httpService.SendRequst(querySalaryUri, HttpMethod.Post, dic);
                var json = Newtonsoft.Json.Linq.JObject.Parse(re);
                var salariesStr = json["salaries"].ToString();
                var salaries=Newtonsoft.Json.JsonConvert.DeserializeObject <IList<Models.SalaryModel>>(salariesStr);
                return salaries;
            }
            catch(HttpRequestException)
            {
                throw new Exception("网络请求出现错误");
            }
            catch(Newtonsoft.Json.JsonException)
            {
                throw new Exception("遇到不规则的数据格式！");
            }
        } 

        public static async Task<bool> GetIsBookingGrade(HttpBaseService _httpService,string username)
        {
            try
            {
                var queryString = $"/{username}";
                var re = await _httpService.SendRequst(bookingGradeUri+ queryString, HttpMethod.Get);
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<BookingGradeResult>(re);
                switch (result.Code)
                {
                    case 200:
                        return true;
                    case 201:
                        return false;
                    default:
                        return false;
                }
            }
            catch
            {
                throw;
            }
            
        }
        public static async Task<BookingGradeResult> BookingGrade(HttpBaseService _httpService, string username,string password)
        {
            try
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("username", username);
                dic.Add("password", password);

                var re = await _httpService.SendRequst(bookingGradeUri, HttpMethod.Post,dic);
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<BookingGradeResult>(re);
                return result;
            }
            catch
            {
                throw;
            }

        }
        public static async Task<BookingGradeResult> UnBookingGrade(HttpBaseService _httpService, string username, string password)
        {
            try
            {

                var queryString = $"?username={username}&password={password}";
                var re = await _httpService.SendRequst(bookingGradeUri+ queryString, HttpMethod.Delete);
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<BookingGradeResult>(re);
                return result;
            }
            catch
            {
                throw;
            }

        }

        public static async Task<string> GetBookingGradeServerStatus(HttpBaseService _httpService)
        {
            try
            {
                var queryString = $"/count";
                var re = await _httpService.SendRequst(bookingGradeUri + queryString, HttpMethod.Get);
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<BookingGradeResult>(re);
                switch (result.Code)
                {
                    case 200:
                        return result.Msg;
                    default:
                        return string.Empty;
                }
            }
            catch
            {
                throw;
            }

        }
    }
}
