using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BJUTDUHelperXamarin.ViewModels
{
    public class CampusCardTransactionDeatilPageViewModel : BindableBase,INavigationAware
    {
        public Models.CampusCardTransactionItemModel _transactionItem;
        public Models.CampusCardTransactionItemModel TransactionItem
        {
            get { return _transactionItem; }
            set { SetProperty(ref _transactionItem, value); }
        }
        public CampusCardTransactionDeatilPageViewModel()
        {

        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
            parameters.Add("from", typeof(Views.CampusCardTransactionDeatilPage).Name);
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            var model = (Models.CampusCardTransactionItemModel)parameters["campuscardtransactionitemmodel"];
            TransactionItem = model;
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
            
        }
    }
}
