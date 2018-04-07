using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BJUTDUHelperXamarin.Models
{
    public class BJUTEduCenterUserinfo:UserBase
    {
        public EduSystemType EduSystemType { get; set; }
    }
    public enum EduSystemType
    {
        BDIC,
        BJUT
    }
}
