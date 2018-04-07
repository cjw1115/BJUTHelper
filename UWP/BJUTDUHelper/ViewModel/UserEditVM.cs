using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BJUTDUHelper.Model;
using System.Collections.ObjectModel;

namespace BJUTDUHelper.ViewModel
{
    public class UserEditVM:ViewModelBase
    {
        //消息标识码
        private readonly string messageToken = "1";

        private string _username;
        public string Username
        {
            get { return _username; }
            set { Set(ref _username, value); }
        }
        private string _password;
        public string Password
        {
            get { return _password; }
            set { Set(ref _password, value); }
        }

        private Type _userType;
        public Type UserType
        {
            get { return _userType; }
            set { Set(ref _userType, value); }
        }
        private string _title;

        public string Title
        {
            get { return _title; }
            set { Set(ref _title, value); }
        }

        private ObservableCollection<Model.BJUTInfoCenterUserinfo> BJUTInfoCenterUserinfos { get; set; }
        private ObservableCollection<Model.BJUTEduCenterUserinfo> BJUTEduCenterUserinfos { get; set; }
        private ObservableCollection<Model.BJUTLibCenterUserinfo> BJUTLibCenterUserinfos { get; set; }

        public void Loaded(View.UserEditNaviParam param)
        {
            BJUTInfoCenterUserinfos = param.BJUTInfoCenterUserinfos;
            BJUTEduCenterUserinfos = param.BJUTEduCenterUserinfos;
            BJUTLibCenterUserinfos = param.BJUTLibCenterUserinfos;

            if (param.User != null)
            {
                Username = param.User.Username;
                Password = param.User.Password;
            }
            switch (param.UserType)
            {
                case "BJUTInfoCenterUserinfo":
                    UserType = typeof(Model.BJUTInfoCenterUserinfo);
                    Title = "信息门户";
                    break;
                case "BJUTLibCenterUserinfo":
                    UserType = typeof(Model.BJUTLibCenterUserinfo);
                    Title = "图书馆";
                    break;
                case "BJUTEduCenterUserinfo":
                    UserType = typeof(Model.BJUTEduCenterUserinfo);
                    Title = "教务中心";
                    break;
                case "StudentID":
                    
                default:
                    break;
            }
        }
        public void Save()
        {
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("测试", messageToken);
            if(string.IsNullOrWhiteSpace(Username))
            {
                GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("账号填写不正确", messageToken);
                return;
            }
            if (string.IsNullOrWhiteSpace(Password))
            {
                GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("密码填写不正确", messageToken);
                return;
            }
            switch (UserType.Name)
            {
                case "BJUTInfoCenterUserinfo":
                    var InfoUser = new Model.BJUTInfoCenterUserinfo() { Username = Username, Password = Password };
                    Service.DbService.SaveInfoCenterUserinfo(InfoUser);
                    if (BJUTInfoCenterUserinfos != null)
                    {
                        if (BJUTInfoCenterUserinfos.Count(m => m.Username == InfoUser.Username) <= 0)
                            BJUTInfoCenterUserinfos.Add(InfoUser);
                    }
                    break;
                case "BJUTLibCenterUserinfo":
                   
                    var LibUser = new Model.BJUTLibCenterUserinfo() { Username = Username, Password = Password };
                    Service.DbService.SaveInfoCenterUserinfo(LibUser);
                    
                    if (BJUTLibCenterUserinfos != null)
                    {
                        if (BJUTLibCenterUserinfos.Count(m => m.Username == LibUser.Username) <= 0)
                            BJUTLibCenterUserinfos.Add(LibUser);
                    }
                    break;
                case "BJUTEduCenterUserinfo":
                  
                    var EduUser = new Model.BJUTEduCenterUserinfo() { Username = Username, Password = Password };
                    Service.DbService.SaveInfoCenterUserinfo(EduUser);
                    
                    if (BJUTEduCenterUserinfos != null)
                    {
                        if (BJUTEduCenterUserinfos.Count(m => m.Username == EduUser.Username) <= 0)
                            BJUTEduCenterUserinfos.Add(EduUser);
                    }
                    break;
                case "StudentID":
                    //Service.FileService.SetStudentID(StudentID); break;
                default:
                    break;
            }
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("保存成功", messageToken);

            
            
        }

    }
}
