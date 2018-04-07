using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using BJUTDUHelper.BJUTDUHelperlException;
using System.Collections.ObjectModel;
using BJUTDUHelper.Model;

namespace BJUTDUHelper.ViewModel
{
    public class WIFIHelperAuthVM:ViewModelBase
    {
        private readonly string messageToken = "1";
        private readonly string bjutUri = "http://my.bjut.edu.cn/";
        private readonly string authUri = "http://wlgn.bjut.edu.cn/";
        private readonly string libPostUri = "http://172.24.39.253/portal/logon.cgi";
        private readonly string libLoginUri = "http://172.24.39.253/portal/logon.htm";

        public CancellationTokenSource cancellationTokenSource { get; set; }

        public ViewModel.AccountModifyVM AccountModifyVM { get; set; }

        private WIFIHelperVM _WIFIHelperVM;
        public WIFIHelperVM WIFIHelperVM
        {
            get { return _WIFIHelperVM; }
            set { Set(ref _WIFIHelperVM, value); }
        }

        private bool _isAuthencated = false;
        public bool IsAuthencated
        {
            get { return _isAuthencated; }
            set { Set(ref _isAuthencated, value); }
        }

        private int _selectIndex = -1;
        public int SelectIndex
        {
            get { return _selectIndex; }
            set { Set(ref _selectIndex, value); }
        }

        private bool? _isInternet = true;
        public bool? IsInternet
        {
            get { return _isInternet; }
            set { Set(ref _isInternet, value); }
        }

        #region 进度环
        private bool _active;
        public bool Active
        {
            get { return _active; }
            set { Set(ref _active, value); }
        }

        private string _progressMessage;
        public string ProgressMessage
        {
            get { return _progressMessage; }
            set { Set(ref _progressMessage, value); }
        }
        #endregion


        public Service.WIFIService WIFIService { get; set; }= new Service.WIFIService();
        private Service.HttpBaseService _httpService = new Service.HttpBaseService();

        public WIFIHelperAuthVM()
        {
            AccountModifyVM = new AccountModifyVM();
            AccountModifyVM.Saved += SaveUserinfo;

        }
        public async void Loaded(object o)
        {
            View.WIFIAuthViewParam param = o as View.WIFIAuthViewParam;
            WIFIHelperVM = param.WIFIHelperVM;

            if (WIFIService == null)
                WIFIService = new Service.WIFIService();
            WIFIService.ScanWifiList();

            SetAuthenStatus();

            
        }
        /// <summary>
        /// 连接wifi
        /// </summary>
        public async void Connect()
        {
            Active = true;
            ProgressMessage = "连接WIFI中···";

            if (SelectIndex < 0)
            {
                GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("选择一个WIFI", messageToken);
                Active = false;
                return;
            }
            if(WIFIHelperVM.InfoUser==null||string.IsNullOrWhiteSpace(WIFIHelperVM.InfoUser.Username)|| string.IsNullOrWhiteSpace(WIFIHelperVM.InfoUser.Password))
            {
                GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("请选择一个账号", messageToken);
                Active = false;
                return;
            }
            var re=await WIFIService.ConnectWifi(WIFIService.WifiList[SelectIndex]);
            Active = false;
            if (re == true)
            {
                Auth(); 
            }
            else
            {
                GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("连接到wifi失败",messageToken);
                
            }
            
        }
        public async void Auth()
        {
            Active = true;
            ProgressMessage = "无线认证中···";

            if (!IsAuthencated)
            {
                try
                {
                    var user = WIFIHelperVM.InfoUser;
                    if (user == null)
                    {
                        throw new NullRefUserinfoException("请输入用户名和密码");
                        //Open = true;//提示重新输入账号
                        //Active = false;
                    }
                    else
                    {
                        await Authenticate(user.Username, user.Password);//不抛异常标明登录成功
                        //手动修改认证成功状态
                        IsAuthencated = true;

                        //IsInternet==true,则直接注册网关
                        NavigationVM.FuncFrame.Navigate(typeof(View.WIFIHelperRegView), new View.WIFIRegViewParam { IsInternet= IsInternet, WIFIHelperVM= this.WIFIHelperVM });
                    }
                }
                catch (NullRefUserinfoException)
                {
                    AccountModifyVM.Open = true;
                    AccountModifyVM.Saved -= Auth;
                    AccountModifyVM.Saved += Auth;
                }
                catch (InvalidUserInfoException)
                {
                    AccountModifyVM.Open = true;
                    AccountModifyVM.Saved -= Auth;
                    AccountModifyVM.Saved += Auth;
                }
                catch (LoginTipException tip)
                {
                    GalaSoft.MvvmLight.Messaging.Messenger.Default.Send(tip.Message, messageToken);
                }
                catch
                {
                    GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("连接异常", messageToken);
                }
                
            }
            Active = false;
        }
        /// <summary>
        /// 设置登录状态，检测登录页“F.htm”
        /// </summary>
        /// <returns></returns>
        public async Task SetAuthenStatus()
        {
            try
            {
                var code = await _httpService.GetResponseCode(bjutUri);
                if (code == System.Net.HttpStatusCode.OK)
                {
                    IsAuthencated = true;
                }
                else
                {
                    IsAuthencated = false;
                }
            }
            catch
            {
                IsAuthencated = false;
            }

        }
        public async Task Authenticate(string username,string password)
        {
            cancellationTokenSource = new CancellationTokenSource();
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            if (SelectIndex < 0)
            {
                GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("选择一个WIFI");
                return;
            }
            switch (WIFIService.WifiList[SelectIndex].Ssid)
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
                            string responseString =await _httpService.SendRequst(libPostUri, HttpMethod.Post,cancellation: cancellationTokenSource.Token);

                            
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

       
       
        public void SaveUserinfo()
        {
            Model.BJUTInfoCenterUserinfo BJUTInfoCenterUserinfo = new Model.BJUTInfoCenterUserinfo();
            BJUTInfoCenterUserinfo.Username =  AccountModifyVM.Username;
            BJUTInfoCenterUserinfo.Password = AccountModifyVM.Password;
            //保存到数据库
            Service.DbService.SaveInfoCenterUserinfo(BJUTInfoCenterUserinfo);
            //同时修改当前选定账号
            WIFIHelperVM.InfoUser = BJUTInfoCenterUserinfo;

            GalaSoft.MvvmLight.Messaging.Messenger.Default.Send<string>("保存成功", messageToken);

        }


        

    }
}
