using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Windows;
using System.ComponentModel;
using Windows.Web.Http;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.Web.Http.Filters;
using Windows.Data.Json;

namespace BJUTHelper
{
    public class NetAccountInfo: INotifyPropertyChanged
    { 
        public  event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (this.PropertyChanged != null) this.PropertyChanged(this, e);
        }
        private Point _startPoint;
        public Point StartPoint
        {
            get
            {
                return _startPoint;
            }

            set
            {
                _startPoint = value;
                OnPropertyChanged(new PropertyChangedEventArgs("StartPoint"));
            }
        }
        private double _throughpuRate;
        public double ThroughpuRate
        {
            get
            {
                return _throughpuRate;
            }

            set
            {
                _throughpuRate = value;
                StartPoint = new Point { X = 0.5, Y = _throughpuRate };
                OnPropertyChanged(new PropertyChangedEventArgs("ThroughpuRate"));
            }
        }

        //当前已登录账户信息
        private string _currentUser;
        public string CurrentUser
        {
            get { return _currentUser; }
            set
            { _currentUser = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CurrentUser"));
            }
        }
        //套餐总量
        private  double _totalThroughputDigit= 1024d;
        public  double TotalThroughputDigit
        {
            get { return _totalThroughputDigit; }
            set{ _totalThroughputDigit = value; }
        }

        //套餐类型
        private string service;
        public string Service
        {
            get { return service; }
            set
            {
                service = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Service"));
            }
        }

        //套餐使用量（数字与文本表示）
        private  double _usedThroughputDigit;
        public  double UsedThroughputDigit
        {
            get { return _usedThroughputDigit; }
            set
            {
                _usedThroughputDigit = value;//计算数字值

                UsedThroughput = value.ToString();//拼接流量信息字符串

                ThroughpuRate = _usedThroughputDigit / TotalThroughputDigit;//计算比率
                OnPropertyChanged(new PropertyChangedEventArgs("UsedThroughputDigit"));
            }
        }
        private  string _usedThroughput;
        public  string UsedThroughput
        {
            get
            {
                return _usedThroughput;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _usedThroughput = "已使用流量:" + value + "MByte";
                    OnPropertyChanged(new PropertyChangedEventArgs("UsedThroughput"));
                }
            }
        }

        //余额
        private string lastMoney;
        public string LastMoney
        {
            get
            {
                return lastMoney;
            }
            set
            {
                lastMoney = "余额:"+value+"元";
                OnPropertyChanged(new PropertyChangedEventArgs("LastMoney"));
            }
        }

        public  NetAccountInfo(CoreDispatcher Dispatcher)
        {
            
            Task.Run(async () =>
            {
                while (true)
                {
                    GetAccountInfo(Dispatcher);
                    await Task.Delay(20 * 1000);
                }
            });
        }
        public async void GetAccountInfo(CoreDispatcher Dispatcher)
        {
            try
            {
                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, new Uri("https://lgn.bjut.edu.cn/"));
                HttpResponseMessage response = (HttpResponseMessage)await client.SendRequestAsync(request);

                using (StreamReader sr = new StreamReader((await response.Content.ReadAsInputStreamAsync()).AsStreamForRead(), Encoding.UTF8))
                {
                    string strHtml = sr.ReadToEnd();
                    string flow = FindInHtml(strHtml, "flow='", false);
                    if (!string.IsNullOrEmpty(flow))
                    {
                        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                          {
                              UsedThroughputDigit = double.Parse(flow);
                          });

                        //await Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        //{
                        //    UsedThroughputDigit = double.Parse(flow);
                        //});
                    }

                    string fee = FindInHtml(strHtml, "fee='", true);
                    if (!string.IsNullOrEmpty(fee))
                    {
                        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            LastMoney = fee;
                        });
                        //await Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        //{
                        //    LastMoney = fee;
                        //});

                    }
                }
                request.Dispose();
                response.Dispose();
                client.Dispose();
            }
            catch (Exception ex)
            {
                UsedThroughput = "";
                lastMoney = "";
            }
            //finally { }

        }
        public async void GetTotalThroughputDigit()
        {
            try
            {
                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, new Uri("https://lgn.bjut.edu.cn/"));
                HttpResponseMessage response = (HttpResponseMessage)await client.SendRequestAsync(request);
                
            }
            catch (Exception ex)
            {
                
            }
        }
        private static string FindInHtml(string strHtml, string strFlag, bool isMoney)
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
            if (isMoney)
            {
                fNum = ((double)iNum) / 10000;
            }
            else
            {
                fNum = ((double)iNum) / 1024;
            }
            return Convert.ToDouble(fNum).ToString("0.00");
        }

        public   async void GetAccountBasicInfo(string username,string passowrd)
        {
            HttpClient client = null;
            HttpRequestMessage request = null;
            HttpResponseMessage response = null;
            HttpBaseProtocolFilter filter = new HttpBaseProtocolFilter();
            try
            {
                client = new HttpClient(filter);

                response = await client.GetAsync(new Uri("https://jfself.bjut.edu.cn/LoginAction.action"));

                string re = await response.Content.ReadAsStringAsync();
                int index = re.IndexOf("checkcode=\"");
                if (index <= 0)
                    return;
                string checkcode = "";
                while (re[index + "checkcode=\"".Length] != '\"')
                {
                    checkcode += re[index + "checkcode=\"".Length];
                    index++;
                }

                response = await client.GetAsync(new Uri("https://jfself.bjut.edu.cn/RandomCodeAction.action?randomNum=0.124"));
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters.Add("account", username);
                parameters.Add("password", passowrd);
                parameters.Add("checkcode", checkcode);

                request = new HttpRequestMessage(HttpMethod.Post, new Uri("https://jfself.bjut.edu.cn/LoginAction.action"));
                request.Content = new HttpFormUrlEncodedContent(parameters);
                request.Headers["ContentType"] = "application/x-www-form-urlencoded";
                await client.SendRequestAsync(request);

                request.Dispose();
                request = new HttpRequestMessage(HttpMethod.Get, new Uri("https://jfself.bjut.edu.cn/refreshaccount?t=1"));
                request.Headers["XRequestedWith"] = "XMLHttpRequest";
                response = await client.SendRequestAsync(request);
                JsonObject json = JsonObject.Parse(await response.Content.ReadAsStringAsync());
                TotalThroughputDigit = Convert.ToDouble(json["note"].GetObject()["leftFlow"].GetString());
                Service = json["note"].GetObject()["service"].GetString();
                CurrentUser = json["note"].GetObject()["welcome"].GetString();
            }
            catch
            {
                TotalThroughputDigit = 0;
                service = "";
                CurrentUser = "";
            }
            finally
            {
                if (response != null)
                {
                    response.Dispose();
                }
                if (request != null)
                {
                    request.Dispose();
                }
                if (client != null)
                {
                    client.Dispose();
                }
                if (filter != null)
                {
                    filter.Dispose();
                }
                
            }

        }
    }
    
}