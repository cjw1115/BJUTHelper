using BJUTDUHelper.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace BJUTDUHelper.ViewModel
{
    public class UserManagerVM:ViewModelBase
    {
        private readonly string messageToken = "1";
        public UserManagerVM()
        {
            SaveCommand = new RelayCommand<object>(Save);
            

            BJUTEduCenterUserinfos = new ObservableCollection<BJUTEduCenterUserinfo>();
            BJUTInfoCenterUserinfos = new ObservableCollection<BJUTInfoCenterUserinfo>();
            BJUTLibCenterUserinfos = new ObservableCollection<BJUTLibCenterUserinfo>();

            ThemeColors = new ObservableCollection<ThemeColorModel>();
        }

        public void Loaded()
        {
            LoadAccountInfo();//加载本地账号信息

            ThemeColors.Clear();
            var colors=Service.SettingService.GetAllColor();
            foreach (var item in colors)
            {
                ThemeColors.Add(item);
            }
            
        }

        #region 用户主题设置
        public ObservableCollection<Model.ThemeColorModel> ThemeColors { get; set; }
        public async void ThemeItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as Model.ThemeColorModel;
            Service.SettingService.SetThemeColor(item.ThemeColor);

            //存储颜色
            DAL.LocalSetting _localSetting = new DAL.LocalSetting();
            await _localSetting.SetLocalInfo<Model.ThemeColorModel>(typeof(Model.ThemeColorModel).Name, item);
        }
        #endregion


        #region 账号管理
        private string _studentID;
        public string StudentID
        {
            get { return _studentID; }
            set { Set(ref _studentID, value); }
        }


        public ObservableCollection<Model.BJUTInfoCenterUserinfo> BJUTInfoCenterUserinfos { get; set; }
        public ObservableCollection<Model.BJUTEduCenterUserinfo>  BJUTEduCenterUserinfos { get; set; }
        public ObservableCollection<Model.BJUTLibCenterUserinfo> BJUTLibCenterUserinfos { get; set; }

        private BJUTInfoCenterUserinfo _infoUser = new BJUTInfoCenterUserinfo();
        public BJUTInfoCenterUserinfo InfoUser
        {
            get { return _infoUser; }
            set { Set(ref _infoUser, value); }
        }
        private BJUTEduCenterUserinfo _eduUser = new BJUTEduCenterUserinfo();
        public BJUTEduCenterUserinfo EduUser
        {
            get { return _eduUser; }
            set { Set(ref _eduUser, value); }
        }
        private BJUTLibCenterUserinfo _libUser = new BJUTLibCenterUserinfo();
        public BJUTLibCenterUserinfo LibUser
        {
            get { return _libUser; }
            set { Set(ref _libUser, value); }
        }

        public ICommand SaveCommand { get; set; }
        public void Save(object param)
        {
            string usertype = (string)param;
            View.UserEditNaviParam naviParam = new View.UserEditNaviParam();

            naviParam.BJUTEduCenterUserinfos = BJUTEduCenterUserinfos;
            naviParam.BJUTInfoCenterUserinfos = BJUTInfoCenterUserinfos;
            naviParam.BJUTLibCenterUserinfos = BJUTLibCenterUserinfos;

            naviParam.UserType = usertype;
            NavigationVM.DetailFrame.Navigate(typeof(View.UserEditPage), naviParam);
        }
        private  void LoadAccountInfo()
        {
            StudentID = Service.FileService.GetStudentID();

            var infouser=Service.DbService.GetInfoCenterUserinfo<Model.BJUTInfoCenterUserinfo>();
            BJUTInfoCenterUserinfos.Clear();
            foreach (var item in infouser)
            {
                BJUTInfoCenterUserinfos.Add(item);
            }

            var eduuser = Service.DbService.GetInfoCenterUserinfo<Model.BJUTEduCenterUserinfo>();
            BJUTEduCenterUserinfos.Clear();
            foreach (var item in eduuser)
            {
                BJUTEduCenterUserinfos.Add(item);
            }

            var libuser = Service.DbService.GetInfoCenterUserinfo<Model.BJUTLibCenterUserinfo>();
            BJUTLibCenterUserinfos.Clear();
            foreach (var item in libuser)
            {
                BJUTLibCenterUserinfos.Add(item);
            }
        }

        public void SetMainID()
        {
            Service.FileService.SetStudentID(StudentID);
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("设置学号成功", messageToken);
        }

        public void EditInfoClick(string username)
        {
            View.UserEditNaviParam naviParam = new View.UserEditNaviParam();
            naviParam.UserType = "BJUTInfoCenterUserinfo";

            var user=Service.DbService.GetInfoCenterUserinfo<Model.BJUTInfoCenterUserinfo>().Where(m=>m.Username==username).FirstOrDefault();
            naviParam.User = user;
            NavigationVM.DetailFrame.Navigate(typeof(View.UserEditPage), naviParam);
        }
        public void EditLibClick(string username)
        {
            View.UserEditNaviParam naviParam = new View.UserEditNaviParam();
            naviParam.UserType = "BJUTLibCenterUserinfo";

            var user = Service.DbService.GetInfoCenterUserinfo<Model.BJUTLibCenterUserinfo>().Where(m => m.Username == username).FirstOrDefault();
            naviParam.User = user;
            NavigationVM.DetailFrame.Navigate(typeof(View.UserEditPage), naviParam);
        }
        public void EditEduClick(string username)
        {
            View.UserEditNaviParam naviParam = new View.UserEditNaviParam();
            naviParam.UserType = "BJUTEduCenterUserinfo";

            var user = Service.DbService.GetInfoCenterUserinfo<Model.BJUTEduCenterUserinfo>().Where(m => m.Username == username).FirstOrDefault();
            naviParam.User = user;
            NavigationVM.DetailFrame.Navigate(typeof(View.UserEditPage), naviParam);
        }


        public void DeleteLibClick(string username)
        {
            var user = BJUTLibCenterUserinfos.Where(m => m.Username == username).FirstOrDefault();
            if (user != null)
            {
                BJUTLibCenterUserinfos.Remove(user);
            }

            Service.DbService.RemoveInfoCenterUserinfo<Model.BJUTLibCenterUserinfo>(username);
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("删除成功", messageToken);
        }
        public void DeleteEduClick(string username)
        {
            var user = BJUTEduCenterUserinfos.Where(m => m.Username == username).FirstOrDefault();
            if (user != null)
            {
                BJUTEduCenterUserinfos.Remove(user);
            }

            Service.DbService.RemoveInfoCenterUserinfo<Model.BJUTEduCenterUserinfo>(username);
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("删除成功", messageToken);
        }
        public void DeleteInfoClick(string username)
        {
            var user = BJUTInfoCenterUserinfos.Where(m => m.Username == username).FirstOrDefault();
            if (user != null)
            {
                BJUTInfoCenterUserinfos.Remove(user);
            }
            Service.DbService.RemoveInfoCenterUserinfo<Model.BJUTInfoCenterUserinfo>(username);
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("删除成功", messageToken);
        }
        #endregion

    }
}
