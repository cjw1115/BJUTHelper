using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography.Certificates;

namespace BJUTHelper.SchoolLife
{
    public class SchoolLifeCommon
    {
        //信息门户登录结果类
        public class LoginReault
        {
            public enum LoginResultEnums
            {
                LoginSuccess = 1, LoginFailure = 2, Other = 4

            }
            public LoginResultEnums LoginResultEnum { get; set; }
            public HttpClient Client { get; set; }
        }
        //登录信息门户
        public static async Task<LoginReault> LoginSchoolLifeCenter(string username, string password)
        {
            LoginReault result = new LoginReault();

            try
            {

                HttpBaseProtocolFilter filter = new HttpBaseProtocolFilter();
                filter.IgnorableServerCertificateErrors.Add(ChainValidationResult.Untrusted);
                filter.IgnorableServerCertificateErrors.Add(ChainValidationResult.InvalidName);
                filter.IgnorableServerCertificateErrors.Add(ChainValidationResult.RevocationFailure);
                HttpClient client = new HttpClient(filter);

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, new Uri("https://my.bjut.edu.cn/userPasswordValidate.portal"));

                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters.Add("Login.Token1", username);
                parameters.Add("Login.Token2", password);
                parameters.Add("goto", "http://my.bjut.edu.cn/loginSuccess.portal");
                parameters.Add("gotoOnFail", "http://my.bjut.edu.cn/loginFailure.portal");

                request.Content = new HttpFormUrlEncodedContent(parameters);
                request.Headers["ContentType"] = "application/x-www-form-urlencoded";

                HttpResponseMessage response = await client.SendRequestAsync(request);

                string re = await response.Content.ReadAsStringAsync();
                if (re.Contains("handleLoginSuccessed"))
                {
                    result.LoginResultEnum = LoginReault.LoginResultEnums.LoginSuccess;

                    result.Client = client;
                    return result;
                }
                else
                {
                    result.LoginResultEnum = LoginReault.LoginResultEnums.LoginFailure;
                    client.Dispose();
                    result.Client = null;
                    return result;
                }

            }
            catch (Exception e)
            {
                
                result.LoginResultEnum = LoginReault.LoginResultEnums.Other;
                result.Client = null;
                return result;
            }


        }
        //获取信息门户到一卡通中心的filter
        public static async Task<HttpClient> GetCardCenterClient(HttpClient client)
        {
            try
            {


                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, new Uri("https://cwss.bjut.edu.cn:9090/smart_web/"));
                HttpResponseMessage response = await client.SendRequestAsync(request);

                request.Dispose();
                request = new HttpRequestMessage(HttpMethod.Get, new Uri("https://cwss.bjut.edu.cn:9090/smart_web/ajax/login/sso"));
                response = await client.SendRequestAsync(request);

                return client;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
