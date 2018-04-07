using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BJUTDUHelper.Model;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;

namespace BJUTDUHelper.ViewModel
{
    public  class WIFIHelperVM:ViewModelBase
    {
        public  ObservableCollection<BJUTInfoCenterUserinfo> BJUTInfoCenterUserinfos { get; set; }
        private BJUTInfoCenterUserinfo _infoUser;
        public BJUTInfoCenterUserinfo InfoUser
        {
            get { return _infoUser; }
            set { Set(ref _infoUser, value); }
        }

        public WIFIHelperVM()
        {
            BJUTInfoCenterUserinfos = new ObservableCollection<Model.BJUTInfoCenterUserinfo>();
            InfoUser = new BJUTInfoCenterUserinfo();

           
        }
        public void Loaded()
        {
            var users = Service.DbService.GetInfoCenterUserinfo<Model.BJUTInfoCenterUserinfo>();
            BJUTInfoCenterUserinfos.Clear();
            foreach (var item in users)
            {
                BJUTInfoCenterUserinfos.Add(item);
            }
        }
    }
}
