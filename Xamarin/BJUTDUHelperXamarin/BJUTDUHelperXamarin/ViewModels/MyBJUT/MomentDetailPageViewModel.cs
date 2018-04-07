using BJUTDUHelperXamarin.Models.MyBJUT;
using Plugin.Media.Abstractions;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Qiniu.IO;
using Qiniu.IO.Model;
using Qiniu.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BJUTDUHelperXamarin.ViewModels.MyBJUT
{
    public class MomentDetailPageViewModel : BindableBase, INavigatedAware
    {
        private readonly string momentsCommentUri = "https://bjuthelper.cjw1115.com/api/moments/comment";
        INavigationService _navigationService;
        Services.HttpBaseService _httpService = new Services.HttpBaseService();

        private int pageSize = 5;
        private int pageIndex = 0;

        public MomentDetailPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            CommentCommand = new DelegateCommand(Comment);
            RefreshCommand = new DelegateCommand(Refresh);
            LoadMoreCommand = new DelegateCommand(LoadMore);
            ItemTappedCommand = new DelegateCommand<object>(ItemTapped);
        }
        public void Load()
        {
            var localList = new ObservableCollection<MomentCommentsViewModel>(Moment.Comments);
            MomentComments = localList;
            LoadAllComments();
        }

        #region Navigation Region

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
        }
        public void OnNavigatedTo(NavigationParameters parameters)
        {
            var re = parameters.Keys.Contains("IsLoad");
            if (re == true)
            {
                var value = (bool)parameters["IsLoad"];
                if (value == true)
                {
                    Moment = parameters["Moment"] as MomentViewModel;
                    Load();
                }
            }
        }

        #endregion

        #region Comment & GetComments

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { SetProperty(ref _isLoading, value); }
        }

        public DelegateCommand CommentCommand { get; set; }
        public async void Comment()
        {
            if (string.IsNullOrEmpty(CommentContent))
            {
                Services.NotityService.Notify("写点什么呗");
                return;
            }
            var user = Services.UserService.Instance.UserInfo;
            if (user == null || user.Username == null || user.Token == null)
            {
                await _navigationService.NavigateAsync(typeof(Views.MyBJUT.LoginPage).Name);
                return;
            }

            IsLoading = true;
            try
            {
                var dic = new Dictionary<string, string>(4);
                dic.Add("MomentID", Moment.ID.ToString());
                dic.Add("CommentToID", _selectedComment==null?string.Empty: _selectedComment.ID.ToString());
                dic.Add("Content", CommentContent);
                dic.Add("Username", user.Username);

                var resultStr = await _httpService.SendRequst(momentsCommentUri, HttpMethod.Post, dic, Authorization: Services.UserService.Instance.Token);
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.MyBJUT.MomentCommentResult>(resultStr);

                if (result.Code == 200)
                {
                    //insert into top of the comment list;
                    var comment=new MomentCommentsViewModel();
                    comment.Content = CommentContent;
                    comment.MomentID = Moment.ID;
                    comment.Nickname = user.NickName;
                    comment.Username = user.Username;
                    comment.PostedTime = DateTime.Now;
                    if (_selectedComment != null)
                    {
                        comment.CommentTo = _selectedComment.ID;
                        comment.CommentToUsername = _selectedComment.Username;
                        comment.CommentToNickname = _selectedComment.Nickname;
                    }
                    comment.FilterNicknameAndContent();

                    MomentComments.Insert(0, comment);

                    CommentContent = string.Empty;
                    _selectedComment = null;
                    SelectedUser = null;

                    Services.NotityService.Notify("评论成功");
                }
                else
                {
                    Services.NotityService.Notify(result.Msg);
                }
            }
            catch (InvalidUserInfoException)
            {
                Services.NotityService.Notify("登录信息失效");
                await _navigationService.NavigateAsync(typeof(Views.MyBJUT.LoginPage).Name);
            }
            catch
            {
                Services.NotityService.Notify("遇到未知错误");
            }
            finally
            {
                IsLoading = false;
            }
        }
        public async Task<MomentCommentsViewModel[]> GetComments(int pageIndex, int pageSize, int id)
        {
            try
            {
                var uri = $"{momentsCommentUri}?momentid={id}&pageIndex={pageIndex}&pageSize={pageSize}";
                var msg = await _httpService.SendRequst(uri, HttpMethod.Get);
                if (msg != null)
                {
                    var result = Newtonsoft.Json.JsonConvert.DeserializeObject<MomentCommentResult>(msg);
                    var list = result.Data.ToArray();
                    foreach (var comment in list)
                    {
                        comment.FilterNicknameAndContent();
                    }
                    return result.Data.ToArray();
                }
                else
                {
                    Services.NotityService.Notify("请求服务器错误");
                    return null;

                }
            }
            catch (Exception e)
            {
#if DEBUG
                Services.NotityService.DisplayAlert("Error", e.Message, "OK");
#else
                Services.NotityService.Notify("获取数据遇到错误");
#endif
            }
            return null;
        }
        #endregion


        public async void LoadAllComments()
        {
            try
            {
                pageIndex = 0;
                var re = await GetComments(pageIndex,pageSize,Moment.ID);
                MomentComments.Clear();
                foreach (var item in re)
                {
                    MomentComments.Add(item);
                }
            }
            catch (Exception e)
            {
#if DEBUG
                Services.NotityService.DisplayAlert("Error", e.Message, "OK");
#else
                Services.NotityService.Notify("获取数据遇到错误");
#endif
            }
            finally
            {
            }

        }

        #region Refreshing
        private bool _isRefreshing;
        public bool IsRefreshing
        {
            get { return _isRefreshing; }
            set { SetProperty(ref _isRefreshing, value); }
        }
        public DelegateCommand RefreshCommand { get; set; }
        public async void Refresh()
        {
            try
            {
                IsRefreshing = true;

                pageIndex = 0;
                var re = await GetComments(pageIndex, pageSize,Moment.ID);
                if (re == null || re.Length == 0)
                {
                    this.CanLoadMore = false;
                }
                else
                {
                    MomentComments.Clear();
                    
                    foreach (var item in re)
                    {
                        MomentComments.Add(item);
                    }
                }

            }
            catch (Exception e)
            {
#if DEBUG
                Services.NotityService.DisplayAlert("Error", e.Message, "OK");
#else
                Services.NotityService.Notify("获取数据遇到错误");
#endif
            }
            finally
            {
                IsRefreshing = false;
            }

        }

        #endregion

        #region LoadMore

        private bool _isLoadingMore = false;
        public bool IsLoadingMore
        {
            get { return _isLoadingMore; }
            set { SetProperty(ref _isLoadingMore, value); }
        }

        private bool _canLoadMore = true;
        public bool CanLoadMore
        {
            get { return _canLoadMore; }
            set { SetProperty(ref _canLoadMore, value); }
        }
        public DelegateCommand LoadMoreCommand { get; set; }
        public async void LoadMore()
        {
            try
            {
                IsLoadingMore = true;
                pageIndex++;
                var newMoments = await GetComments(pageIndex, pageSize,Moment.ID);
                if (newMoments == null || newMoments.Length == 0)
                {
                    CanLoadMore = false;
                }
                else
                {
                    foreach (var item in newMoments)
                    {
                        MomentComments.Add(item);
                    }
                }
            }
            catch (Exception e)
            {
#if DEBUG
                Services.NotityService.DisplayAlert("Error", e.Message, "OK");
#else
                Services.NotityService.Notify("获取数据遇到错误");
#endif
            }
            finally
            {
                IsLoadingMore = false;
            }

        }

#endregion

        private Models.MyBJUT.MomentViewModel _moment;
        public Models.MyBJUT.MomentViewModel Moment
        {
            get => _moment;
            set => SetProperty(ref _moment, value);
        }

        private ObservableCollection<Models.MyBJUT.MomentCommentsViewModel> _momentComments;
        public ObservableCollection<Models.MyBJUT.MomentCommentsViewModel> MomentComments
        {
            get => _momentComments;
            set => SetProperty(ref _momentComments, value);
        }

        

        private string _commentContent;
        public string CommentContent
        {
            get { return _commentContent; }
            set { SetProperty(ref _commentContent , value); }
        }


        private string _selectedUser;
        public string SelectedUser
        {
            get { return _selectedUser; }
            set { SetProperty(ref _selectedUser, value); }
        }

        private MomentCommentsViewModel _selectedComment;
        public ICommand ItemTappedCommand { get; set; }
        public async void ItemTapped(object model)
        {
            _selectedComment = model as MomentCommentsViewModel;
            SelectedUser = _selectedComment.Nickname;//_selectedComment.CommentTo.ToString();
        }
    }
}
