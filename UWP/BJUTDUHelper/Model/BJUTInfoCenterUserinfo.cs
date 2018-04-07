using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BJUTDUHelper.Model
{
    public class BJUTInfoCenterUserinfo:UserBase
    {
        [NotMapped]
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
