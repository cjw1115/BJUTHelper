using Prism.Unity;
using BJUTDUHelperXamarin.Views;
using Xamarin.Forms;
using Prism;
using Prism.Unity;
namespace BJUTDUHelperXamarin
{
    //public static class ViewModelLocator
    //{
    //    public static ViewModels.SalaryPageViewModel SalaryPageVM => new ViewModels.SalaryPageViewModel();
    //}
    public partial class App : PrismApplication
    {
        public Prism.Navigation.INavigationService GetNavigationService()
        {
            return this.NavigationService;
        }
        public App(IPlatformInitializer initializer = null) : base(initializer)
        {
        }

        public static string ThemeColor { get; set; }

        protected async override void OnInitialized()
        {
            InitializeComponent();

            //初始化设置
            Models.Settings.InitSetting();
            //await NavigationService.NavigateAsync("NavigationPage");
            this.MainPage = new Views.NavigationPage();
            //await 

            var _httpService = new Services.HttpBaseService();
            if (Device.OS == TargetPlatform.Android)
            {
                Services.ToolService.CheckUpdate(_httpService);
            }
            Services.ToolService.Pant(_httpService);
        }

        protected override void RegisterTypes()
        {

            Container.RegisterTypeForNavigation<Views.NavigationPage>();

            Container.RegisterTypeForNavigation<WifiHelperPage>();

            Container.RegisterTypeForNavigation<EduPage>();
            Container.RegisterTypeForNavigation<EduGradePage>();
            Container.RegisterTypeForNavigation<EduGradeDetailPage>();
            Container.RegisterTypeForNavigation<EduSchedulePage>();
            Container.RegisterTypeForNavigation<EduScheduleDetailPage>();


            Container.RegisterTypeForNavigation<UserPage>();
            Container.RegisterTypeForNavigation<UserInfoDetailPage>();

            Container.RegisterTypeForNavigation<CaptchaPage>();


            Container.RegisterTypeForNavigation<CampusCardPage>();
            Container.RegisterTypeForNavigation<CampusCardTransactionDeatilPage>();


            Container.RegisterTypeForNavigation<AboutPage>();
            Container.RegisterTypeForNavigation<FeedBackPage>();
            Container.RegisterTypeForNavigation<EduExamPage>();

            Container.RegisterTypeForNavigation<MyBjutPage>();
            Container.RegisterTypeForNavigation<SalaryPage>();
            Container.RegisterTypeForNavigation<NewsPage>();

            Container.RegisterTypeForNavigation<EduGradeInfoPage>();

            Container.RegisterTypeForNavigation<BookingGradePage>();
        }


    }
}
