using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BJUTDUHelperXamarin.ViewModels
{
    public class EduScheduleDetailPageViewModel : BindableBase,INavigationAware
    {
        private Models.ScheduleItem _scheduleItem;

        public Models.ScheduleItem ScheduleItem
        {
            get { return _scheduleItem; }
            set { SetProperty(ref _scheduleItem , value); }
        }

        public EduScheduleDetailPageViewModel()
        {
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
            if (parameters == null)
                parameters = new NavigationParameters();
            parameters.Add("from", typeof(Views.EduScheduleDetailPage));

        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            ScheduleItem=(Models.ScheduleItem)parameters["scheduleitem"];
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
        }
    }
}
