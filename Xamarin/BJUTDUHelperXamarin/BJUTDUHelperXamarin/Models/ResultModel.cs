using BJUTDUHelperXamarin.Models.MyBJUT;
using System;
using System.Collections.Generic;
using System.Text;

namespace BJUTDUHelperXamarin.Models
{
    public class LoginResultModel
    {
        /// <summary>
        /// success:200,content error:400,server error:500,
        /// </summary>
        public int Code { get; set; }
        public string Msg { get; set; }

        public string Token { get; set; }

        public BJUTHelperUserInfo Data { get; set; }
    }
    public class RegistResultModel
    {
        /// <summary>
        /// success:200,content error:400,server error:500,
        /// </summary>
        public int Code { get; set; }
        public string Msg { get; set; }
    }
}
