using BJUTDUHelperXamarin.Models;
using BJUTDUHelperXamarin.Models.MyBJUT;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BJUTDUHelperXamarin.Services
{
    public class UserService
    {
        private readonly string loginUri = "https://bjuthelper.cjw1115.com/api/user/login";
        private readonly string registUri = "https://bjuthelper.cjw1115.com/api/user/regist";
        private readonly string infoUri = "https://bjuthelper.cjw1115.com/api/user/info";
        private readonly string retrieveUri = "https://bjuthelper.cjw1115.com/api/user/forgot";

        public static UserService Instance = new UserService();
        DbService _dbService;
        private UserService()
        {
            _dbService = new DbService();
        }

        public async Task<BJUTHelperUserInfo> LoginAsync(HttpBaseService httpService, LoginModel loginModel)
        {
            try
            {
                var passwordHash = Helpers.Utility.GetMD5(loginModel.Password);
                var dic = new Dictionary<string, string>(2);
                dic.Add("Username", loginModel.Username);
                dic.Add("Password", passwordHash);

                var re = await httpService.SendRequst(loginUri, HttpMethod.Post, dic);
                if (string.IsNullOrEmpty(re))
                {
                    throw new LoginTipException("服务故障");
                }
                var model = Newtonsoft.Json.JsonConvert.DeserializeObject<LoginResultModel>(re);
                if (model == null)
                {
                    throw new LoginTipException("服务故障");
                }
                switch (model.Code)
                {
                    case 200:
                        model.Data.Token = model.Token;
                        return model.Data;
                    case 300:
                        throw new LoginTipException("用户名或密码错误");
                    case 400:
                        throw new LoginTipException("填写的登录信息错误");
                    case 500:
                        throw new LoginTipException("服务故障");
                    default:
                        throw new LoginTipException($"未知异常 Code:{model.Code}");
                }

            }
            catch (HttpRequestException)
            {
                throw new LoginTipException("服务故障");
            }
            catch(Exception e)
            {
                throw;
            }
        }
        //public class RegistModel
        //{
        //    public string Username { get; set; }
        //    public string Password { get; set; }
        //    public string NickName { get; set; }
        //    public Gender Gender { get; set; }
        //    public int Age { get; set; }
        //    public string EMail { get; set; }
        //    public string PhoneNumber { get; set; }
        //}

        public async Task RegistAsync(HttpBaseService httpService, RegistModel model)
        {
            try
            {
                var passwordHash = Helpers.Utility.GetMD5(model.Password);
                var dic = new Dictionary<string, string>();
                dic.Add("Username", model.Username);
                dic.Add("Password", passwordHash);
                dic.Add("Nickname", model.NickName);
                dic.Add("Gender", model.Gender.ToString());
                dic.Add("Age", model.Age.ToString());
                dic.Add("College", model.College.ToString());

                var re = await httpService.SendRequst(registUri, HttpMethod.Post, dic);
                if (string.IsNullOrEmpty(re))
                {
                    throw new LoginTipException("服务故障");
                }
                var resultModel = Newtonsoft.Json.JsonConvert.DeserializeObject<RegistResultModel>(re);
                if (resultModel == null)
                {
                    throw new LoginTipException("服务故障");
                }
                switch (resultModel.Code)
                {
                    case 200:
                        break;
                    case 300:
                        throw new LoginTipException("该用户名已经注册");
                    case 400:
                        throw new LoginTipException("注册信息填写错误");
                    case 500:
                        throw new LoginTipException("服务故障");
                    default:
                        throw new LoginTipException($"未知异常 Code:{resultModel.Code}");
                }
            }
            catch (HttpRequestException)
            {
                throw new LoginTipException("服务故障");
            }
            catch
            {
                throw;
            }

        }


        public async Task<BJUTHelperUserInfo> EditUserinfoAsync(HttpBaseService httpService, BJUTHelperUserInfo userinfo,string avatarPath)
        {
            FileStream fileStream = null;
            try
            {
                var content = new MultipartFormDataContent();
                content.Add(new StringContent(userinfo.Username), "Username");
                content.Add(new StringContent(userinfo.NickName), "Nickname");
                content.Add(new StringContent(userinfo.Gender.ToString()), "Gender");
                content.Add(new StringContent(userinfo.Age.ToString()), "Age");
                content.Add(new StringContent(userinfo.College.ToString()), "College");

                if (!string.IsNullOrEmpty(avatarPath))
                {
                    fileStream = File.Open(avatarPath, FileMode.Open);
                    StreamContent imageContent = imageContent = new StreamContent(fileStream);
                    content.Add(imageContent, "avatar",Path.GetFileName(avatarPath));
                }
                

                var re = await httpService.SendRequst(infoUri, HttpMethod.Post,content,Authorization: Services.UserService.Instance.Token);
                if (string.IsNullOrEmpty(re))
                {
                    throw new LoginTipException("服务故障");
                }
                var model = Newtonsoft.Json.JsonConvert.DeserializeObject<LoginResultModel>(re);
                if (model == null)
                {
                    throw new LoginTipException("服务故障");
                }
                switch (model.Code)
                {
                    case 200:
                        return model.Data;
                    case 300:
                    case 400:
                    case 401:
                        throw new LoginTipException(model.Msg);
                    case 500:
                        throw new LoginTipException("服务故障");
                    default:
                        throw new LoginTipException($"未知异常 Code:{model.Code}");
                }
            }
            catch (HttpRequestException)
            {
                throw new LoginTipException("服务故障");
            }
            catch(Exception e)
            {
                throw;
            }
            finally
            {
                fileStream?.Dispose();
            }

        }

        public async Task<BJUTHelperUserInfo> RetrieveAsync(HttpBaseService httpService, RetrieveModel model)
        {
            try
            {
                var passwordHash = Helpers.Utility.GetMD5(model.Password);
                var dic = new Dictionary<string, string>();
                dic.Add("Username", model.Username);
                dic.Add("Password", passwordHash);
                dic.Add("VarifyPassword", model.VarifyPassword);

                var re = await httpService.SendRequst(retrieveUri, HttpMethod.Post, dic);
                if (string.IsNullOrEmpty(re))
                {
                    throw new LoginTipException("服务故障");
                }
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<LoginResultModel>(re);
                if (result == null)
                {
                    throw new LoginTipException("服务故障");
                }
                switch (result.Code)
                {
                    case 200:
                        result.Data.Token = result.Token;
                        return result.Data;
                    default:
                        throw new LoginTipException(result.Msg);
                }
            }
            catch (HttpRequestException)
            {
                throw new LoginTipException("服务故障");
            }
            catch
            {
                throw;
            }

        }

        public bool GetIsSignedIn()
        {
            var user = UserService.Instance.UserInfo;
            if (user == null)
            {
                return false;
            }
            else
            {
                if (string.IsNullOrEmpty(user.Token))
                    return false;
                else
                    return true;
            }
        }

        public BJUTHelperUserInfo LoadLocalUserinfo()
        {
            try
            {
                var user = _dbService.GetAll<BJUTHelperUserInfo>().FirstOrDefault();
                return user;
            }
            catch (Exception e)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine(e.Message);
#endif
                return null;
            }
        }
        public bool SetLocalUserinfo(BJUTHelperUserInfo user)
        {
            try
            {
                _dbService.DeleteAll<BJUTHelperUserInfo>();
                _dbService.Insert<BJUTHelperUserInfo>(user);
                return true;
            }
            catch
            {
                return false;
            }

        }

        public BJUTHelperUserInfo UserInfo => LoadLocalUserinfo();
        public string Token
        {
            get
            {
                if (UserInfo == null || UserInfo.Token == null)
                {
                    return null;
                }
                else
                {
                    return "Bearer " + UserInfo.Token;
                }
            }
        }
        public string GetOriginToken()
        {
            var token = Token;
            if (string.IsNullOrEmpty(token))
            {
                return null;
            }
            return token.Replace("Bearer ", "");
        }

        public void Logout()
        {
            _dbService.DeleteAll<BJUTHelperUserInfo>();
        }

        public static string FilterUsername(string username)
        {
            var front = username.Substring(0, 2);
            var end = username.Substring(username.Length - 1, 1);

            username = front + "*****" + end;
            return username;
        }
    }
}
