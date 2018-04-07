using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BJUTDUHelperXamarin.Models
{
    public class CampusCardTransactionItemModel:BindableBase
    {
        private string _smtDealName;
        private string _smtDealDateTimeTxt;
        private string _smtTransMoney;
        private string _smtOutMoney;
        private string _smtOrgName;
        private string _machineId;

        public string smtDealName {
            get { return _smtDealName; }
            set { SetProperty(ref _smtDealName, value); } }         //消费类型
        public string smtDealDateTimeTxt {
            get { return _smtDealDateTimeTxt; }
            set { SetProperty(ref _smtDealDateTimeTxt, value); }
        }  //消费时间
        public string smtTransMoney {
            get { return _smtTransMoney; }
            set { SetProperty(ref _smtTransMoney, value); }
        }       //消费金额
        public string smtOutMoney {
            get { return _smtOutMoney; }
            set { SetProperty(ref _smtOutMoney, value); }
        }         //余额
        public string smtOrgName {
            get { return _smtOrgName; }
            set { SetProperty(ref _smtOrgName, value); }
        }          //消费地点
        public string machineId {
            get { return _machineId; }
            set { SetProperty(ref _machineId, value); }
        }           //刷卡机号
    }
}
