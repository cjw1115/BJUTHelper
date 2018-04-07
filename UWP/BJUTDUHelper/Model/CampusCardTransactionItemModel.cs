using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BJUTDUHelper.Model
{
    public class CampusCardTransactionItemModel
    {
        public string smtDealName { get; set; }         //消费类型
        public string smtDealDateTimeTxt { get; set; }  //消费时间
        public string smtTransMoney { get; set; }       //消费金额
        public string smtOutMoney { get; set; }         //余额
        public string smtOrgName { get; set; }          //消费地点
        public string machineId { get; set; }           //刷卡机号
    }
}
