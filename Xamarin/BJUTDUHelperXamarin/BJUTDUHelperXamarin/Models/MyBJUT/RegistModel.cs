using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace BJUTDUHelperXamarin.Models.MyBJUT
{
    public class RetrieveModel
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void Set<T>(ref T filed, T value, [CallerMemberName]string propertyName = null)
        {
            filed = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private string _username;
        public string Username { get => _username; set => Set(ref _username, value); }

        private string _password;
        
        public string Password { get => _password; set=>Set(ref _password,value); }

        private string _varifyPassword;
        public string VarifyPassword { get => _varifyPassword; set=>Set(ref _varifyPassword,value); }
    }
    public class RegistModel:INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _username;
        public string Username{ get => _username; set => Set(ref _username , value); }

        public string _password;
        public string Password { get => _password; set=>Set(ref _password,value); }

        public string _passwordConfirm;
        public string PasswordConfirm { get => _passwordConfirm; set => Set(ref _passwordConfirm, value); }

        private string _nickName;
        public string NickName { get => _nickName; set=>Set(ref _nickName,value); }

        public BJUTCollege? _college;
        public BJUTCollege? College { get => _college; set=>Set(ref _college,value); }

        private Gender? _gender;
        public Gender? Gender { get => _gender; set=>Set(ref _gender,value); }

        private int _age;
        public int Age { get => _age; set=>Set(ref _age,value); }

        public void Set<T>(ref T filed,T value,[CallerMemberName]string propertyName=null)
        {
            filed = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }

    public class LoginModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string _username;
        public string Username { get => _username; set => Set(ref _username , value); }

        public string _password;
        public string Password { get => _password; set => Set(ref _password, value); }

        public void Set<T>(ref T filed, T value, [CallerMemberName]string propertyName = null)
        {
            filed = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
    public enum Gender
    {
        隐私 = 0,
        男,
        女
    }
    public enum BJUTCollege
    {
        机械工程与应用电子技术学院
        ,信息学部
        ,建筑工程学院
        ,环境与能源工程学院
        ,应用数理学院
        ,材料科学与工程学院
        ,经济与管理学院
        ,人文社会科学学院
        ,建筑与城市规划学院
        ,生命科学与生物工程学院
        ,外国语学院
        ,艺术设计学院
        ,继续教育学院
        ,体育教学部
        ,激光工程研究院
        ,固体微结构与性能研究所
        ,循环经济研究院
        ,高等教育研究所
        ,马克思主义学院
        ,国际学院
        ,北京都柏林国际学院
        ,城市交通学院
        ,实验学院
        ,樊恭烋学院
        ,创新创业学院
        ,北京科学与工程计算研究院
        ,北京古月新材料研究院
        ,北京智慧城市研究院
        ,北京知识产权学院_研究院
        ,北京未来网络科技高精尖创新中心
        ,京津冀绿色发展研究院
    }
}
