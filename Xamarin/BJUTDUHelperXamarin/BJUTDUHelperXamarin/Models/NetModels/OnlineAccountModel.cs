using System;
using System.Collections.Generic;
using System.Text;

namespace BJUTDUHelperXamarin.Models.NetModels
{
    public class OnlineAccountModel
    {
        public string IPv4 { get; set; }
        public string IPv6 { get; set; }
        public string Mac { get; set; }
        public string SessionID { get; set; }
        public bool IsCurrentIP { get; set; }
    }
}
