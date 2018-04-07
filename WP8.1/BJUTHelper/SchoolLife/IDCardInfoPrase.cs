using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Web.Http;

namespace BJUTHelper
{
    public class RecordEntity
    {
        public string smtDealName { get; set; }         //消费类型
        public string smtDealDateTimeTxt { get; set; }  //消费时间
        public string smtTransMoney { get; set; }       //消费金额
        public string smtOutMoney { get; set; }         //余额
        public string smtOrgName { get; set; }          //消费地点
        public string machineId { get; set; }           //刷卡机号
    }
    //暂时不需用
    public class PersonBasicInfo
    {
        public string smtSalaryno { get; set; }//个人编码
        public string smtName { get; set; } // 陈佳卫
    }
    
    public class IDCardInfo:INotifyPropertyChanged
    {
        public string smtCardid { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
            }
        }
        private string _smtShowcardno;
        public string smtShowcardno
        {
            get { return _smtShowcardno; }
            set
            {
                _smtShowcardno = value;
                OnPropertyChanged("smtShowcardno");
            }
        }//卡号 = 14024238
        public string _smtAccounts;//银行卡号 = 6217000010031624029
        public string smtAccounts
        {
            get { return _smtAccounts; }
            set
            {
                _smtAccounts = value;
                OnPropertyChanged("smtAccounts");
            }
        }
        public string _smtCarddateTxt;//注册日期 = 2014 - 09 - 05
        public string smtCarddateTxt
        {
            get { return _smtCarddateTxt; }
            set
            {
                _smtCarddateTxt = value;
                OnPropertyChanged("smtCarddateTxt");
            }
        }
        public string _smtEndcodeTxt;
        public string smtEndcodeTxt
        {
            get { return _smtEndcodeTxt; }
            set
            {
                _smtEndcodeTxt = value;
                OnPropertyChanged("smtEndcodeTxt");
            }
        }
        public string _smtValiditydateTxt;//有效期= 2020 - 08 - 21
        public string smtValiditydateTxt
        {
            get { return _smtValiditydateTxt; }
            set
            {
                _smtValiditydateTxt = value;
                OnPropertyChanged("smtValiditydateTxt");
            }
        }
        public string _smtDealdatetimeTxt;//计算余额截止日期 = 2015 - 08 - 12 11:34:05
        public string smtDealdatetimeTxt
        {
            get { return _smtDealdatetimeTxt; }
            set
            {
                _smtDealdatetimeTxt = value;
                OnPropertyChanged("smtDealdatetimeTxt");
            }
        }
        private double _balance;//余额 = 150.1

        public double balance
        {
            get
            {
                return _balance;
            }

            set
            {
                _balance = value;
                OnPropertyChanged("balance");
            }
        }

        

    }

    public class IDCardInfoPrase
    {
        public int RecordNum { get; set; } = 15;
        public ObservableCollection<RecordEntity> TransactionList { get; set; }
        public IDCardInfo idCardInfo { get; set; }
        public IDCardInfoPrase()
        {
            TransactionList = new ObservableCollection<RecordEntity>();
            idCardInfo = new IDCardInfo();
        }
        public async Task GetIDCardInfo(HttpClient client)
        {
            try
            {
                #region 获取消费记录

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, new Uri("https://cwss.bjut.edu.cn:9090/smart_web/ajax/card/list.json"));

                HttpResponseMessage response = await client.SendRequestAsync(request);
                string jsonStr = await response.Content.ReadAsStringAsync();
                PraseIDCardInfo(jsonStr);
                #endregion
            }
            catch { }
            
        }
        public async Task GetTransactionInfo(HttpClient client)
        {
            try
            {
                #region 获取消费记录

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, new Uri("https://cwss.bjut.edu.cn:9090/smart_web/ajax/tran/list.json"));
                IDictionary<string, string> parameters = new Dictionary<string, string>();
                parameters.Add("int_start", "0");
                parameters.Add("int_maxSize", RecordNum.ToString());
                request.Content = new HttpFormUrlEncodedContent(parameters);
                HttpResponseMessage response = await client.SendRequestAsync(request);
                string jsonStr = await response.Content.ReadAsStringAsync();
                PraseTransactionInfo(jsonStr);
                #endregion
            }
            catch { };
        }
        public void PraseTransactionInfo(string jsonStr)
        {
            JsonObject jsonObject = JsonObject.Parse(jsonStr);

            
            foreach (var item in jsonObject["jsonData"].GetObject()["pageData"].GetArray())
            {
                var record = item.GetObject();
                RecordEntity temp = new RecordEntity();
                temp.machineId = record.GetNamedString("machineId");
                temp.smtDealDateTimeTxt = record.GetNamedString("smtDealDateTimeTxt");
                temp.smtDealName = record.GetNamedString("smtDealName");
                temp.smtOutMoney = record.GetNamedNumber("smtOutMoney").ToString();
                temp.smtTransMoney = record.GetNamedNumber("smtTransMoney").ToString();
                temp.smtOrgName = record.GetNamedString("smtOrgName");
                TransactionList.Add(temp);
            }
                        
        }
        public void PraseIDCardInfo(string jsonStr)
        {
            JsonObject jsonObject = JsonObject.Parse(jsonStr);


            foreach (var item in jsonObject["jsonData"].GetObject()["pageData"].GetArray())
            {
                var record = item.GetObject();
                if (idCardInfo == null)
                    idCardInfo = new IDCardInfo();
                idCardInfo.balance = record.GetNamedNumber("balance");
                idCardInfo.smtAccounts = record.GetNamedString("smtAccounts");
                idCardInfo.smtDealdatetimeTxt = record.GetNamedString("smtDealdatetimeTxt");
                idCardInfo.smtCarddateTxt = record.GetNamedString("smtCarddateTxt");
                idCardInfo.smtEndcodeTxt = record.GetNamedString("smtEndcodeTxt");
                idCardInfo.smtShowcardno = record.GetNamedString("smtShowcardno");
                idCardInfo.smtValiditydateTxt = record.GetNamedString("smtValiditydateTxt");
                idCardInfo.smtCardid = record.GetNamedNumber("smtCardid").ToString();
            }

        }
    }
}
