using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using System.Runtime.CompilerServices;

namespace BJUTDUHelperXamarin.Models
{
    public class BJUTInfoCenterUserinfo:UserBase
    {
        [Ignore]
        public InfoCenterAccountInfo InfoCenterAccountInfo { get; set; }

    }
    public class InfoCenterAccountInfo:INotifyPropertyChanged
    {
        private string _fluPackageType;
        public string FluPackageType
        {
            get { return _fluPackageType; }
            set
            {
                _fluPackageType = value;
                OnPropertyChanged();
                //Set(ref _fluPackageType, value);
            }
        }
        private string _totalFlu;
        public string TotalFlu
        {
            get { return _totalFlu; }
            set
            {
                _totalFlu = value;
                OnPropertyChanged();
                //Set(ref _totalFlu, value);
            }
        }

        private string _usedFlu;
        public string UsedFlu
        {
            get { return _usedFlu; }
            set
            {
                _usedFlu = value;
                OnPropertyChanged();
                //Set(ref _uedFlu, value);
            }
        }
        private string _balance;
        public string Balance
        {
            get { return _balance; }
            set
            {
                //Set(ref _balance, value);
                _balance = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
