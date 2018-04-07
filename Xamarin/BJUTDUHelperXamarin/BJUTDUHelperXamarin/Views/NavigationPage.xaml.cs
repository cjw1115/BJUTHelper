using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Linq;
using System.Threading.Tasks;
using Prism.Unity;

namespace BJUTDUHelperXamarin.Views
{
    public partial class NavigationPage : MasterDetailPage
    {
        public static Xamarin.Forms.NavigationPage CurrentNavigationPage { get; set; }
        public NavigationPage()
        {
            InitializeComponent();
            CurrentNavigationPage = this.Detail as Xamarin.Forms.NavigationPage;

            this.btnLogin.Clicked += BtnLogin_Clicked;
            this.IsPresentedChanged += NavigationPage_IsPresentedChanged;

            this.naviListView.ItemTapped += NaviListView_ItemTapped;
            Command aboutClickCommand = new Command(AboutClick);
            aboutLayout.GestureRecognizers.Add(new TapGestureRecognizer() { Command = aboutClickCommand });
        }

        private void NavigationPage_IsPresentedChanged(object sender, EventArgs e)
        {
            if (this.IsPresented == true)
            {
                var vm = this.BindingContext as ViewModels.NavigationPageViewModel;
                vm.Load();
            }
        }

        private async void BtnLogin_Clicked(object sender, EventArgs e)
        {
            await CurrentNavigationPage.PushAsync(new Views.MyBJUT.LoginPage());
            //await Navigate(typeof(Views.MyBJUT.LoginPage));
            IsPresented = false;
        }
     
        private async void AboutClick()
        {
            await Navigate(typeof(Views.AboutPage));
            IsPresented = false;
        }
        private static Dictionary<Type, Xamarin.Forms.Page> naviPageDic = new Dictionary<Type, Xamarin.Forms.Page>();
        private async void NaviListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var model = e.Item as Models.NavigationItemModel;
            if (model != null && model.PageType != null)
            {
                await Navigate(model.PageType);

                naviListView.SelectedItem = null;
                IsPresented = false;
                
            }
            else
            {
                Services.NotityService.Notify("这个饼还没画完呢O(∩_∩)O");
            }
        }

        private async Task Navigate(Type navipage)
        {
            var app=PrismApplication.Current as App;
            var naviService=app.GetNavigationService();
            //await naviService.NavigateAsync(navipage.Name);
            Page page = null;
            if (naviPageDic.ContainsKey(navipage))
            {
                page = naviPageDic[navipage];
            }
            else
            {

                page = (Page)Activator.CreateInstance(navipage);
                naviPageDic.Add(navipage, page);
            }
            var list = new List<Page>();
            list.AddRange(naviPage.Navigation.NavigationStack);
            for (int i = 1; i < list.Count - 1; i++)
            {
                naviPage.Navigation.RemovePage(list[i]);
            }


            //app.MainPage = this;
            ////Prism.Common.PageUtilities.
            await naviPage.PushAsync(page);
            //await naviService.NavigateAsync(navipage.Name);

            if (list.Count >= 2 && page != list.Last())
                naviPage.Navigation.RemovePage(list.Last());
        }
    }
}