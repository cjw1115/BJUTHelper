using BJUTDUHelper.Model;
using BJUTDUHelper.Service;
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
using Windows.Data.Json;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using static BJUTDUHelper.ViewModel.UserManagerVM;
using BJUTDUHelper.BJUTDUHelperlException;

namespace BJUTDUHelper.ViewModel
{
    public class WIFIHelperRegVM : ViewModelBase
    {
        private HttpBaseService _httpService = new HttpBaseService();
        private readonly string messageToken = "1";
        private readonly string loginUri = "http://lgn.bjut.edu.cn/";
        private readonly string manageUri = "https://jfself.bjut.edu.cn/LoginAction.action";
        private readonly string checkcodeUri = "https://jfself.bjut.edu.cn/nav_login";
        private readonly string randomcodeUri = "https://jfself.bjut.edu.cn/RandomCodeAction.action?randomNum=0.124";
        private readonly string accountUri = "https://jfself.bjut.edu.cn/refreshaccount?t=1";
        private readonly string logoutUri = "https://lgn.bjut.edu.cn/F.htm";
        public CancellationTokenSource cancellationTokenSource { get; set; }
        
        public ViewModel.AccountModifyVM AccountModifyVM { get; set; }
        private WIFIHelperVM _WIFIHelperVM;
        public WIFIHelperVM WIFIHelperVM
        {
            get { return _WIFIHelperVM; }
            set { Set(ref _WIFIHelperVM, value); }
        }

        private InfoCenterAccountInfo _accountInfo;
        public InfoCenterAccountInfo AccountInfo
        {
            get { return _accountInfo; }
            set { Set(ref _accountInfo, value); }
        }

        private bool _isRegisted = false;
        public bool IsRegisted
        {
            get { return _isRegisted; }
            set
            {
                Set(ref _isRegisted, value);
            }
        }
        private bool _isGetAccountInfo = true;
        public bool IsGetAccountInfo
        {
            get { return _isGetAccountInfo; }
            set { Set(ref _isGetAccountInfo, value); }
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


        public WIFIHelperRegVM()
        {
            AccountModifyVM = new ViewModel.AccountModifyVM();
            AccountModifyVM.Saved+= SaveUserinfo;
            AccountInfo = new InfoCenterAccountInfo();
            SetRegistStatus();

        }
        public async void Login()
        {
            Active = true;
            ProgressMessage = "正在注册网关";

            await SetRegistStatus();
            if (IsRegisted)
            {
                GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("已经登录成功", messageToken);
            }
            else
            {
                try
                {
                    if (WIFIHelperVM.InfoUser == null||string.IsNullOrWhiteSpace(WIFIHelperVM.InfoUser.Username)|| string.IsNullOrWhiteSpace(WIFIHelperVM.InfoUser.Password))
                    {
                        throw new NullRefUserinfoException("请选择账号");
                    }
                    var re = await Register(WIFIHelperVM.InfoUser.Username, WIFIHelperVM.InfoUser.Password);
                    if (re == true)
                    {
                        IsRegisted = true;//主动修改登录状态
                    }

                }
                catch (NullRefUserinfoException)
                {
                    GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("请选择账号", messageToken);
                    //Open = true;//打开
                    //Saved += SaveUserinfo;
                    //Saved += (o) => { Login(this, null); };
                }
                catch(InvalidUserInfoException )
                {
                    GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("用户名或密码错误", messageToken);
                    AccountModifyVM.Open = true;
                    AccountModifyVM.Saved -= Login;
                    AccountModifyVM.Saved += Login;
                }
                catch (HttpRequestException)
                {
                    GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("网络异常", messageToken);
                }
                catch(LoginTipException tip)
                {
                    GalaSoft.MvvmLight.Messaging.Messenger.Default.Send(tip.Message, messageToken);
                }
                catch
                {
                    GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("其他错误", messageToken);
                }
            }

            //更新数据
            if(WIFIHelperVM.InfoUser!=null)
                GetAccountBasicInfo(WIFIHelperVM.InfoUser.Username, WIFIHelperVM.InfoUser.Password);
            Active = false;

        }
        public async void Logout(object sender, RoutedEventArgs e)
        {
            Active = true;
            ProgressMessage = "正在注销";

            await SetRegistStatus();
            if (IsRegisted)
            {
                try
                {
                    cancellationTokenSource = new CancellationTokenSource();
                       var code = await _httpService.GetResponseCode(logoutUri,cancellationTokenSource.Token);
                    if (code == System.Net.HttpStatusCode.OK)
                    {
                        GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("注销成功", messageToken);
                        IsRegisted = false;//主动修改登陆状态
                    }
                    else
                    {
                        GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("注销出现问题", messageToken);
                    }
                }
                catch (HttpRequestException requestException)
                {
                    GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("网络已断开", messageToken);
                    
                }
                finally
                {
                    Active = false;
                }
            }
            else
            {
                GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("已注销", messageToken);
            }
            Active = false;
        }
      
        /// <summary>
        /// 设置登录状态，检测登录页“F.htm”
        /// </summary>
        /// <returns></returns>
        public async Task SetRegistStatus()
        {
            try
            {
                var str = await _httpService.SendRequst(loginUri, HttpMethod.Get);
                if (str.Contains("F.htm"))
                {
                    IsRegisted = true;
                }
                else
                {
                    IsRegisted = false;
                }
            }
            catch
            {
                IsRegisted = false;
            }

        }
        /// <summary>
        /// 注册网关
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public async Task<bool> Register(string username, string password)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            StringBuilder bodyString = new StringBuilder();

            dic.Add("0MKKey", "");
            dic.Add("DDDDD", username);
            dic.Add("upass", password);

            cancellationTokenSource = new CancellationTokenSource();
            var stringRespones = await _httpService.SendRequst(loginUri, HttpMethod.Post, dic, cancellation: cancellationTokenSource.Token);

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
                return true;
            }
            else
            {
                throw new LoginTipException("其他错误");
            }
        }

        public async void GetAccountBasicInfo(string username, string passowrd)
        {
            try
            {
                var re = await _httpService.SendRequst(checkcodeUri, HttpMethod.Get);
                int index = re.IndexOf("checkcode=\"");
                if (index <= 0)
                    return;
                string checkcode = "";
                while (re[index + "checkcode=\"".Length] != '\"')
                {
                    checkcode += re[index + "checkcode=\"".Length];
                    index++;
                }

                //好像没啥意义。。
                await _httpService.SendRequst(randomcodeUri, HttpMethod.Get);

                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters.Add("account", username);
                parameters.Add("password", passowrd);
                parameters.Add("checkcode", checkcode);
                await _httpService.SendRequst(manageUri, HttpMethod.Post, parameters);//登录管理中心

                re = await _httpService.SendRequst(accountUri, HttpMethod.Get);
                JsonObject json = JsonObject.Parse(re);
                var totalFlu = json["note"].GetObject()["leftFlow"].GetString();
                var service = json["note"].GetObject()["service"].GetString();
                var leftmoney = json["note"].GetObject()["leftmoeny"].GetString();
                var flutype = json["note"].GetObject()["welcome"].GetString();

                AccountInfo.TotalFlu = totalFlu + "MB";
                AccountInfo.Balance = leftmoney;
                AccountInfo.FluPackageType = flutype;

                IsGetAccountInfo = false;
            }
            catch
            {
            }
            try
            {
                var re = await _httpService.SendRequst(loginUri, HttpMethod.Get);
                string usedflu = FindInHtml(re, "flow='", false);

               

                AccountInfo.UsedFlu = usedflu + "MB";

                IsGetAccountInfo = false;
            }
            catch
            {

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


        public  void Loaded(object o)
        {
            View.WIFIRegViewParam param = o as View.WIFIRegViewParam;
            WIFIHelperVM = param.WIFIHelperVM;
            //WIFIHelperVM.BJUTInfoCenterUserinfos = param.WIFIHelperVM.BJUTInfoCenterUserinfos;

            if (param .IsInternet!= null&& param.IsInternet==true)
            {
                //直接登陆
                Login();
            }
            if (WIFIHelperVM.InfoUser != null)
                GetAccountBasicInfo(WIFIHelperVM.InfoUser.Username, WIFIHelperVM.InfoUser.Password);
            else
                GetAccountBasicInfo(null,null);
        }

      
        public void SaveUserinfo()
        {
            BJUTInfoCenterUserinfo BJUTInfoCenterUserinfo = new BJUTInfoCenterUserinfo();
            BJUTInfoCenterUserinfo.Username = AccountModifyVM.Username;
            BJUTInfoCenterUserinfo.Password = AccountModifyVM.Password;

            //保存到数据库
            Service.DbService.SaveInfoCenterUserinfo(BJUTInfoCenterUserinfo);
            //同时修改当前选定账号
            WIFIHelperVM.InfoUser = BJUTInfoCenterUserinfo;

            GalaSoft.MvvmLight.Messaging.Messenger.Default.Send<string>("保存成功", messageToken);

        }

        public void AppBarButton_Click()
        {
            View.WIFIAuthViewParam param = new View.WIFIAuthViewParam { WIFIHelperVM = WIFIHelperVM };
            NavigationVM.FuncFrame.Navigate(typeof(View.WIFIHelperAuthView), param);
        }
    }
}
