using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace BJUTDUHelper.ViewModel
{
    public class CheckCodeVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _checkCode;
        public string CheckCode
        {
            get { return _checkCode; }
            set { _checkCode = value; OnPropertyChanged(); }
        }
        private ImageSource _checkCodeSource;
        public ImageSource CheckCodeSource
        {
            get { return _checkCodeSource; }
            set { _checkCodeSource = value; OnPropertyChanged(); }
        }

        private bool _openCheckCodeDlg;
        public bool OpenCheckCodeDlg
        {
            get { return _openCheckCodeDlg; }
            set { _openCheckCodeDlg = value; OnPropertyChanged(); }
        }

        public event Action CheckCodeSaved;
        /// <summary>
        /// 保存验证码后登录并导航
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        public void Save(object o, EventArgs e)
        {
            CheckCodeSaved?.Invoke();

        }

        public event Action CheckCodeRefresh;
        public  void Refresh(object o, EventArgs e)
        {
            CheckCodeRefresh?.Invoke();
        }
    } 
}