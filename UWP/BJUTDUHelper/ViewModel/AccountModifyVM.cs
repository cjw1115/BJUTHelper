using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BJUTDUHelper.ViewModel
{
    public class AccountModifyVM:INotifyPropertyChanged
    {
        
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string propertyName=null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)) ;
        }
        #region 用户名密码框逻辑代码
        //控制用户名密码框的状态

        private string _username;
        public string Username
        {
            get { return _username; }
            set { _username = value;OnPropertyChanged(); }
        }
        private string _password;
        public string Password
        {
            get { return _password; }
            set { _password = value; OnPropertyChanged(); }
        }

        private bool _open;
        public bool Open
        {
            get { return _open; }
            set
            {
                _open = value;
                OnPropertyChanged();

            }
        }

        public event Action Saved;

        public void Save()
        {
            Saved?.Invoke();
        }
        #endregion
    }
}
