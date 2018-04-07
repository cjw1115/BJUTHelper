using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BJUTHelper.NetworkManager
{
    public enum ConnectMode
    {
        Ipv4,
        IPv6,
        IPv4v6=Ipv4|IPv6
    }
    public enum CodeResult
    {
        Success=1,Fail
    }
    public class Result
    {
        public CodeResult CodeResult { get; set; }
        public string Msg { get; set; }
    }
    public class BjutInternet
    {
        public static async Task<Result> Login(string username,string password,ConnectMode connectMode)
        {
            HttpClient client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(10);
            HttpRequestMessage request = null;
            HttpResponseMessage response = null;
            Result re = new Result();
            try
            {
                string ipv6 = "";
                Dictionary<string, string> dic = new Dictionary<string, string>();
                StringBuilder bodyString = new StringBuilder();

                if (connectMode== ConnectMode.IPv4v6)
                {
                    request = new HttpRequestMessage(HttpMethod.Post, new Uri("https://lgn6.bjut.edu.cn/V6?https://lgn.bjut.edu.cn"));
                    request.Headers.Add("ContentType", "application/x-www-form-urlencoded");
                    
                    dic.Add("DDDDD", username);
                    dic.Add("upass", password);

                    for (int i=0;i<dic.Count;i++)
                    {
                        var key = dic.Keys.ToArray()[i];
                        if (i > 0)
                        {
                            bodyString.Append($"&{key}={dic[key]}");
                        }
                        else
                        {
                            bodyString.Append($"{key}={dic[key]}");
                        }
                        
                    }
                    StringContent t = new StringContent(bodyString.ToString());
                    request.Content = t;

                    response = await client.SendAsync(request);
                    var stream = await response.Content.ReadAsStreamAsync();
                    string str;
                    using (StreamReader sr = new StreamReader(stream))
                    {
                        str = sr.ReadToEnd();
                    }
                    //获取IPV6地址
                    int index = str.LastIndexOf("value");
                    for (int i = index + 7; str[i] != '\''; i++)
                    {
                        ipv6 += str[i];
                    }
                    request.Dispose();
                    response.Dispose();
                }
                request = new HttpRequestMessage(HttpMethod.Post, new Uri("https://lgn.bjut.edu.cn/"));
                dic.Clear();
                dic.Add("0MKKey", "");
                dic.Add("DDDDD", username);
                dic.Add("upass", password);
                dic.Add("v6ip", ipv6);
                request.Headers.Add("ContentType", "application/x-www-form-urlencoded");

                bodyString.Clear();
                for (int i = 0; i < dic.Count; i++)
                {
                    var key = dic.Keys.ToArray()[i];
                    if (i > 0)
                    {
                        bodyString.Append($"&{key}={dic[key]}");
                    }
                    else
                    {
                        bodyString.Append($"{key}={dic[key]}");
                    }
                        
                }
                StringContent requestContent = new StringContent(bodyString.ToString());
                request.Content = requestContent;
                
                response = await client.SendAsync(request);

                string stringRespones = await response.Content.ReadAsStringAsync();
                if (response.StatusCode ==  System.Net.HttpStatusCode.OK)//一下检测登录是否成功标志值需要重新抓取
                {
                    if (stringRespones.Contains("Msg=01"))
                    {
                        re.CodeResult = CodeResult.Fail;
                        re.Msg = "密码错误";
                    }
                    else if (stringRespones.Contains("Msg=06"))
                    {
                        re.CodeResult = CodeResult.Fail;
                        re.Msg = "喜闻乐见的system buffer full";
                    }
                    else if (stringRespones.Contains("Msg=05"))
                    {
                        re.CodeResult = CodeResult.Fail;
                        re.Msg = "本账号暂停使用";
                    }
                    else if (stringRespones.Contains("Msg=04"))
                    {
                        re.CodeResult = CodeResult.Fail;
                        re.Msg = "本账号费用超支或时长流量超过限制";
                    }
                    else if (stringRespones.Contains("UID='"))
                    {
                        re.CodeResult = CodeResult.Success;
                        re.Msg = "登陆成功";
                    }
                }
                else
                {
                    re.CodeResult = CodeResult.Fail;
                    re.Msg = "网络异常";
                }
            }
            catch (Exception ex)
            {
                re.CodeResult = CodeResult.Fail;
                re.Msg = ex.Message;
            }
            finally
            {
                request?.Dispose();
                response?.Dispose();
                client?.Dispose();
            }
            return re;
        }
        public static async Task<Result> Logout()
        {
            HttpClient client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(5);
            HttpResponseMessage response = null;
            Result re = new Result();
            try
            {
                response = await client.GetAsync(new Uri("https://lgn.bjut.edu.cn/F.htm"));

                string content = await response.Content.ReadAsStringAsync();
                if (content.Contains("Msg=14"))
                {
                    re.CodeResult = CodeResult.Success;
                    re.Msg = "注销成功";
                }
                else
                {
                    re.CodeResult = CodeResult.Fail;
                    re.Msg = "注销出现问题";
                }
            }
            catch (Exception ex)
            {
                re.CodeResult = CodeResult.Fail;
                re.Msg = ex.Message;
            }
            finally
            {
                response?.Dispose();
                client?.Dispose();
            }
            return re;
        } 
    }
}
