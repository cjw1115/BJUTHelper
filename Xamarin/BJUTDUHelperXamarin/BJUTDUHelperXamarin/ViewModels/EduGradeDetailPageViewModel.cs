using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Navigation;
using Xamarin.Forms;
namespace BJUTDUHelperXamarin.ViewModels
{
    public class EduGradeDetailPageViewModel:BindableBase,INavigationAware
    {
        private Models.EduGradeItemModel _gradeItem;
        public Models.EduGradeItemModel GradeItem
        {
            get{return _gradeItem;}
            set{SetProperty(ref _gradeItem,value);}
        }
        public void OnNavigatedFrom(NavigationParameters parameters)
        {
            if (parameters == null)
            {
                parameters = new NavigationParameters();
            }
            parameters.Add("backfromdetail", true);
        }
        public void OnNavigatedTo(NavigationParameters parameters)
        {   
            var model = (Models.EduGradeItemModel)parameters["gradeitemmodel"];
            GradeItem=model;
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
            
        }
    }

}