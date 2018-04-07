using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using GalaSoft.MvvmLight.Views;

namespace BJUTDUHelper.ViewModel
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            //注册导航服务
            var navigationService = CreateNavigationService();
            SimpleIoc.Default.Register(() => navigationService);
            SimpleIoc.Default.Register<NavigationVM>();
            SimpleIoc.Default.Register<WIFIHelperRegVM>();
            SimpleIoc.Default.Register<UserManagerVM>();
            SimpleIoc.Default.Register<WIFIHelperAuthVM>();
            SimpleIoc.Default.Register<BJUTEduCenterVM>();
            SimpleIoc.Default.Register<BJUTEduGradeVM>();
            SimpleIoc.Default.Register<BJUTEduScheduleVM>();
            SimpleIoc.Default.Register<BJUTCampusCardVM>();
            SimpleIoc.Default.Register<BJUTCampusCardTransactionVM>();
            SimpleIoc.Default.Register<BJUTEduExamVM>();
            SimpleIoc.Default.Register<WIFIHelperVM>();
            SimpleIoc.Default.Register<UserEditVM>();
        }
        public NavigationVM NavigationVM
        {
            get { return SimpleIoc.Default.GetInstance<NavigationVM>(); }
        }
        public WIFIHelperRegVM WIFIHelperRegVM
        {
            get { return SimpleIoc.Default.GetInstance<WIFIHelperRegVM>(); }
        }
        public UserManagerVM UserManagerVM
        {
            get { return SimpleIoc.Default.GetInstance<UserManagerVM>(); }
        }
        public WIFIHelperAuthVM WIFIHelperAuthVM
        {
            get { return SimpleIoc.Default.GetInstance<WIFIHelperAuthVM>(); }
        }
        public BJUTEduCenterVM BJUTEduCenterVM
        {
            get { return SimpleIoc.Default.GetInstance<BJUTEduCenterVM>(); }
        }
        public BJUTEduGradeVM BJUTEduGradeVM
        {
            get { return SimpleIoc.Default.GetInstance<BJUTEduGradeVM>(); }
        }
        public BJUTEduScheduleVM BJUTEduScheduleVM
        {
            get { return SimpleIoc.Default.GetInstance<BJUTEduScheduleVM>(); }
        }
        public BJUTCampusCardVM BJUTCampusCardVM
        {
            get { return SimpleIoc.Default.GetInstance<BJUTCampusCardVM>(); }
        }
        public BJUTCampusCardTransactionVM BJUTCampusCardTransactionVM
        {
            get { return SimpleIoc.Default.GetInstance<BJUTCampusCardTransactionVM>(); }
        }
        public BJUTEduExamVM BJUTEduExamVM
        {
            get { return SimpleIoc.Default.GetInstance<BJUTEduExamVM>(); }
        }
        public WIFIHelperVM WIFIHelperVM
        {
            get { return SimpleIoc.Default.GetInstance<WIFIHelperVM>(); }
        }

        public UserEditVM UserEditVM
        {
            get { return SimpleIoc.Default.GetInstance<UserEditVM>(); }
        }

        public INavigationService CreateNavigationService()
        {
            var navigationService = new NavigationService();
            //navigationService.Configure(typoef("PageName").Name,typeof("PageName"));
            return navigationService;
        }
    }
}
