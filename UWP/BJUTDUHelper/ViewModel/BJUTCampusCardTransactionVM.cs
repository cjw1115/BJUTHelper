using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BJUTDUHelper.ViewModel
{
    public class BJUTCampusCardTransactionVM:ViewModelBase
    {
        private Service.HttpBaseService _httpService;
        private Service.BJUTCampusCardService campusCardService;
        public BJUTCampusCardTransactionVM()
        {
            campusCardService = new Service.BJUTCampusCardService();
        }


        public void Loaded(object param)
        {
            _httpService = param as Service.HttpBaseService;
            GetTransactionInfo();
        }
        private IList<Model.CampusCardTransactionItemModel> _transacitonList;
        public IList<Model.CampusCardTransactionItemModel> TransactionList
        {
            get { return _transacitonList; }
            set { Set(ref _transacitonList, value); }
        }
        public async void GetTransactionInfo()
        {
            var re=await campusCardService.GetTransactionInfo(_httpService);
            TransactionList = re;
        }
    }
}
