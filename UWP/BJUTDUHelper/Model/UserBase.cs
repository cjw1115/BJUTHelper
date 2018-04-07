using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BJUTDUHelper.Model
{

    public class UserBase
    {
        public int ID{get;set;}
        public string Username { get; set; }
        public string Password { get; set; }
        
    }
}
