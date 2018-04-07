using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace BJUTDUHelper.ViewModel
{
    public class BJUTCampusCardVM:ViewModelBase
    {
        public readonly string messageToken = "1";
        public List<Model.CampusCardNavigationModel> EduNavigationList { get; set; } = new List<Model.CampusCardNavigationModel>
        {
            new Model.CampusCardNavigationModel {Name="交易明细",PageType=typeof(View.BJUTCampusCardTransactionView)},
           
        };
        private Model.CampusCardInfoModel _campusCardInfoModel;
        public Model.CampusCardInfoModel CampusCardInfoModel
        {
            get { return _campusCardInfoModel; }
            set { Set(ref _campusCardInfoModel, value); }
        }

        public Model.BJUTInfoCenterUserinfo BJUTInfoCenterUserinfo { get; set; }
        public Service.HttpBaseService _httpService;
        private Service.BJUTCampusCardService campusCardService;

        public AccountModifyVM AccountModifyVM { get; set; }
        public BJUTCampusCardVM()
        {
            //SaveCommand = new RelayCommand<object>(Save);
               _httpService = new Service.HttpBaseService();
            campusCardService = new Service.BJUTCampusCardService();
            AccountModifyVM = new AccountModifyVM();
            AccountModifyVM.Saved += SaveUserinfo;
        }
        public  void Loaded()
        {
            var studentid = Service.FileService.GetStudentID();
            var user = Service.DbService.GetInfoCenterUserinfo<Model.BJUTInfoCenterUserinfo>().Where(m => m.Username == studentid).FirstOrDefault();
            if (user == null)
                return;
            BJUTInfoCenterUserinfo = user;
            LoadCampascadInfo();
            LoginInfoCenter();

        }
        public async void SaveCampascadInfo()
        {
            DAL.LocalSetting _local = new DAL.LocalSetting();
            await _local.SetLocalInfo("Campuscard" + BJUTInfoCenterUserinfo.Username, CampusCardInfoModel);

            await SaveImageToFile("Campuscard" + BJUTInfoCenterUserinfo.Username + "Image", CampusCardInfoModel.PersonImage, ApplicationData.Current.LocalFolder);
        }
        private object lockObject = new object();
        public async void LoadCampascadInfo()
        {
            if (BJUTInfoCenterUserinfo == null)
            {
                return;
            }
            DAL.LocalSetting _local = new DAL.LocalSetting();
            var model=await _local.GetLocalInfo<Model.CampusCardInfoModel>("Campuscard" + BJUTInfoCenterUserinfo.Username);
            lock (lockObject)
            {
                CampusCardInfoModel = model;
            }
            var item=await Windows.Storage.ApplicationData.Current.LocalFolder.TryGetItemAsync("Campuscard" + BJUTInfoCenterUserinfo.Username + "Image");
            if (item != null)
            {
                StorageFile imagefile = item as StorageFile;
                using (var ras = await imagefile.OpenReadAsync( ))
                {
                    BitmapDecoder decoder = await BitmapDecoder.CreateAsync(BitmapDecoder.JpegDecoderId, ras);
                    var provider = await decoder.GetPixelDataAsync();
                    byte[] buffer = provider.DetachPixelData();
                    WriteableBitmap bitmap = new WriteableBitmap((int)decoder.PixelWidth, (int)decoder.PixelHeight);
                    await bitmap.PixelBuffer.AsStream().WriteAsync(buffer, 0, buffer.Length);
                    CampusCardInfoModel.PersonImage = bitmap;
                }
            }
        }
        public async static Task<StorageFile> SaveImageToFile(string fileName, ImageSource imageSource, StorageFolder goalFolder)
        {
            StorageFolder localFolder = goalFolder;
            if (localFolder == null)
                return null;
            var tempFile = await localFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            if (tempFile == null)
                return null;
            using (var ras = await tempFile.OpenAsync(FileAccessMode.ReadWrite, StorageOpenOptions.None))
            {
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, ras);
                WriteableBitmap bitmap = imageSource as WriteableBitmap;
                using (Stream stream = bitmap.PixelBuffer.AsStream())
                {
                    byte[] pixels = new byte[stream.Length];
                    await stream.ReadAsync(pixels, 0, pixels.Length);
                    encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore,
                              (uint)bitmap.PixelWidth,
                              (uint)bitmap.PixelHeight,
                              96.0,
                              96.0,
                              pixels);
                }
                await encoder.FlushAsync();
            }
            return tempFile;
        }
        public void ItemClick(object sender,ItemClickEventArgs e)
        {
            Model.CampusCardNavigationModel ClickedItem = e.ClickedItem as Model.CampusCardNavigationModel;
            if (ClickedItem != null && ClickedItem.PageType != null)
            {
                NavigationVM.DetailFrame.Navigate(ClickedItem.PageType, _httpService);
            }
        }
        public async void LoginInfoCenter( )
        {
            try
            {
                Active = true;
                ProgressMessage = "登录中···";
                if (BJUTInfoCenterUserinfo == null)
                {
                    throw new BJUTDUHelperlException.NullRefUserinfoException("请输入用户名和密码");
                }
                await campusCardService.LoginInfoCenter(_httpService, BJUTInfoCenterUserinfo.Username, BJUTInfoCenterUserinfo.Password);

                ProgressMessage = "获取信息中···";
                await campusCardService.GetCardCenterClient(_httpService);
                //获取一卡通个人信息
                var re = await campusCardService.GetCampusCardBasicInfo(_httpService);
                CampusCardInfoModel = re;
                SaveCampascadInfo();
                //获取一卡通个人信息
                LoadCampusCardSalaryInfo();

            }
            catch(NullReferenceException )
            {
                GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("请输入用户名和密码", messageToken);
                AccountModifyVM.Open = true;
                AccountModifyVM.Saved -= LoginInfoCenter;
                AccountModifyVM.Saved += LoginInfoCenter;
            }
            catch (BJUTDUHelperlException.InvalidUserInfoException userinfoException)
            {
                GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("用户名或密码无效", messageToken);
                AccountModifyVM.Open = true;
                AccountModifyVM.Saved -= LoginInfoCenter;
                AccountModifyVM.Saved += LoginInfoCenter;
            }
            catch(HttpRequestException )
            {
                GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("请检查网络连接", messageToken);
            }
            catch 
            {
                GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("其他错误", messageToken);
            }
            finally
            {
                Active = false;
            }
        }

        public async void LoadCampusCardSalaryInfo()
        {


            try
            {
                if (CampusCardInfoModel == null)
                {
                    CampusCardInfoModel = new Model.CampusCardInfoModel();
                }
                var info = await campusCardService.GetCampusCardSalaryInfo(_httpService);
                CampusCardInfoModel.balance = info.balance;
                CampusCardInfoModel.smtAccounts = info.smtAccounts;
                CampusCardInfoModel.smtEndcodeTxt = info.smtEndcodeTxt;
                CampusCardInfoModel.smtCardid = info.smtCardid;
            }
            catch (HttpRequestException requestException)
            {
                GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("请检查网络连接", messageToken);
            }
            catch
            {

            }

        }
        public async void LostCampusCard()
        {
            Active = true;
            ProgressMessage = "挂失中···";
            try
            {
                var re = await campusCardService.LostCampusCard(_httpService, CampusCardInfoModel.smtCardid);
                LoadCampusCardSalaryInfo();//刷新信息
                GalaSoft.MvvmLight.Messaging.Messenger.Default.Send(re, messageToken);
            }
            catch(HttpRequestException requestException)
            {
                GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("请检查网络连接", messageToken);
            }
            catch
            {
                GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("未知错误", messageToken);
            }
            finally
            {
                Active = false;
            }
            
        }
        public async void FoundCampusCard()
        {
            Active = true;
            ProgressMessage = "解除挂失中···";
            try
            {
                var re = await campusCardService.FundCampusCard(_httpService, CampusCardInfoModel.smtCardid);
                LoadCampusCardSalaryInfo();//刷新信息
                GalaSoft.MvvmLight.Messaging.Messenger.Default.Send(re, messageToken);
            }
            catch (HttpRequestException requestException)
            {
                GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("请检查网络连接", messageToken);
            }
            catch
            {
                GalaSoft.MvvmLight.Messaging.Messenger.Default.Send("未知错误", messageToken);
            }
            finally
            {
                Active = false;
            }
        }

       
        public async void SaveUserinfo()
        {
            if (BJUTInfoCenterUserinfo == null)
            {
                BJUTInfoCenterUserinfo = new Model.BJUTInfoCenterUserinfo();
            }

            BJUTInfoCenterUserinfo.Username = AccountModifyVM.Username;
            BJUTInfoCenterUserinfo.Password = AccountModifyVM.Password;

            Service.DbService.SaveInfoCenterUserinfo(BJUTInfoCenterUserinfo);

            GalaSoft.MvvmLight.Messaging.Messenger.Default.Send<string>("保存成功", messageToken);
           
        }

        #region 进度环
        private bool _active;
        public bool Active
        {
            get { return _active; }
            set { Set(ref _active, value); }
        }

        private string _progressMessage;
        public string ProgressMessage
        {
            get { return _progressMessage; }
            set { Set(ref _progressMessage, value); }
        }
        #endregion
    }
}
