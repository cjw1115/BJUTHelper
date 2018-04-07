using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BJUTDUHelper.Model
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
