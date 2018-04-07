using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace BJUTDUHelper.Model
{
    public class CampusCardInfoModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName]string PropertyName="")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        #region 一卡通个人基础信息
        private string _smtSalaryno;
        public string Username
        {
            get { return _smtSalaryno; }
            set { _smtSalaryno = value; OnPropertyChanged(); }
        }

        private string _smtName;
        public string Name
        {
            get { return _smtName; }
            set
            {
                _smtName = value; OnPropertyChanged();
            }
        }

        private string _smtSex;
        public string Gender
        {
            get { return _smtSex; }
            set { _smtSex = value; OnPropertyChanged(); }
        }
        private string _smtDeptcodeTxt;
        public string DepartmentName
        {
            get { return _smtDeptcodeTxt; }
            set { _smtDeptcodeTxt = value; OnPropertyChanged(); }
        }

        private ImageSource _personImage;
        [IgnoreDataMember]
        public ImageSource PersonImage
        {
            get { return _personImage; }
            set { _personImage = value;OnPropertyChanged(); }
        }
        #endregion
        private string _smtCardid;
        public string smtCardid
        {
            get { return _smtCardid; }
            set
            {
                _smtCardid = value;
                OnPropertyChanged();
            }
        }//卡号 = 14024238

        private string _smtShowcardno;
        public string smtShowcardno
        {
            get { return _smtShowcardno; }
            set
            {
                _smtShowcardno = value;
                OnPropertyChanged( );
            }
        }//卡号 = 14024238
        public string _smtAccounts;//银行卡号 = 6217000010031624029
        public string smtAccounts
        {
            get { return _smtAccounts; }
            set
            {
                _smtAccounts = value;
                OnPropertyChanged( );
            }
        }
        public string _smtCarddateTxt;//注册日期 = 2014 - 09 - 05
        public string smtCarddateTxt
        {
            get { return _smtCarddateTxt; }
            set
            {
                _smtCarddateTxt = value;
                OnPropertyChanged( );
            }
        }
        public string _smtEndcodeTxt;
        public string smtEndcodeTxt
        {
            get { return _smtEndcodeTxt; }
            set
            {
                _smtEndcodeTxt = value;
                OnPropertyChanged( );
            }
        }
        public string _smtValiditydateTxt;//有效期= 2020 - 08 - 21
        public string smtValiditydateTxt
        {
            get { return _smtValiditydateTxt; }
            set
            {
                _smtValiditydateTxt = value;
                OnPropertyChanged( );
            }
        }
        public string _smtDealdatetimeTxt;//计算余额截止日期 = 2015 - 08 - 12 11:34:05
        public string smtDealdatetimeTxt
        {
            get { return _smtDealdatetimeTxt; }
            set
            {
                _smtDealdatetimeTxt = value;
                OnPropertyChanged( );
            }
        }
        private double _balance;//余额 = 150.1

        public double balance
        {
            get
            {
                return _balance;
            }

            set
            {
                _balance = value;
                OnPropertyChanged( );
            }
        }

    }
}
