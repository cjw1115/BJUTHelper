using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BJUTHelper.Model
{
    public enum IPModeEnum
    {
        IPV4=1,IPV6V4
    }
    public class UserEntity
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
    public class UserLoginEntity: UserEntity
    {
        
        public IPModeEnum IPMode { get; set; }
    }
    public class UserAuthEntity: UserEntity
    {
       
    }
    public class UserJWGLEntity : UserEntity
    {

    }
    public class UserIDCardEntity : UserEntity
    { }
}
