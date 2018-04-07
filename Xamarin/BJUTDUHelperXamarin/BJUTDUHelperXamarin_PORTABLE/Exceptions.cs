using System;

namespace BJUTDUHelperXamarin
{
    public class InvalidUserInfoException : System.Exception
    {
        public InvalidUserInfoException(string message) : base(message)
        {

        }
    }
    public class InvalidCheckcodeException : System.Exception
    {
        public InvalidCheckcodeException(string message) : base(message)
        {
        }
    }
    public class NullRefUserinfoException : Exception
    {
        public NullRefUserinfoException(string message) : base(message)
        {
        }
    }
    public class LoginTipException : Exception
    {
        public LoginTipException(string message) : base(message)
        {

        }

    }
}
