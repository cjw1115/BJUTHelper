using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace BJUTDUHelperXamarin.Models.MyBJUT
{

    public class BJUTHelperUserInfo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public int ID { get; set; }

        private string _avatar;
        public string Avatar
        {
            get => _avatar;
            set => Set(ref _avatar, value);
        }
        private string _username;
        public string Username { get => _username; set => Set(ref _username, value); }

        public string _password;
        public string Password { get => _password; set => Set(ref _password, value); }

        private string _nickName;
        public string NickName { get => _nickName; set => Set(ref _nickName, value); }

        public BJUTCollege _college;
        public BJUTCollege College { get => _college; set => Set(ref _college, value); }

        private Gender _gender;
        public Gender Gender { get => _gender; set => Set(ref _gender, value); }

        private int _age;
        public int Age { get => _age; set => Set(ref _age, value); }

        public string Token { get; set; }

        public void Set<T>(ref T filed, T value, [CallerMemberName]string propertyName = null)
        {
            filed = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
