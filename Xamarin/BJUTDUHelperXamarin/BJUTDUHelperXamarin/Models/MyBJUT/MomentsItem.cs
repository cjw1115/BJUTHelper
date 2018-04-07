using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace BJUTDUHelperXamarin.Models.MyBJUT
{
    public class MomentItem : INotifyPropertyChanged
    {
        public int ID { get; set; }
        public string Content { get; set; }
        public DateTime PostTime { get; set; }

        private string[] _imgUri;
        public string[] ImgUri
        {
            get { return _imgUri; }
            set
            {
                _imgUri = value;
                OnPropertyChanged();
            }
        }

        public string Username { get; set; }
        public string Nickname { get; set; }

        public IList<MomentCommentsViewModel> Comments { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


    public class MomentViewModel
    {
        public int ID { get; set; }
        public string Content { get; set; }
        public DateTime PostTime { get; set; }
        public IList<string> ImgUri { get; set; }

        public string Username { get; set; }
        public string Nickname { get; set; }
        public string Avatar { get; set; }

        public int CommentCount { get; set; }

        public IList<MomentCommentsViewModel> Comments { get; set; }
        public void FilterNicknameAndConent()
        {
            var moment = this;

            if (string.IsNullOrEmpty(moment.Nickname) || moment.Nickname == moment.Username)
            {
                moment.Nickname = Services.UserService.FilterUsername(moment.Username);
            }
            if (this.Comments != null)
            {
                foreach (var item in Comments)
                {
                    item.FilterNicknameAndContent();
                }
            }
        }
    }

    public class MomentCommentsViewModel
    {
        public int ID { get; set; }
        public string Content { get; set; }
        public DateTime PostedTime { get; set; }

        public string Username { get; set; }
        public string Nickname { get; set; }
        public string Avatar { get; set; }
        public int MomentID { get; set; }
        public int CommentTo { get; set; }

        public string CommentToUsername { get; set; }
        public string CommentToNickname { get; set; }

        public string CommentToAvatar { get; set; }
        public void FilterNicknameAndContent()
        {
            var comment = this;

            if (string.IsNullOrEmpty(comment.Nickname) || comment.Nickname == comment.Username)
            {
                comment.Nickname = Services.UserService.FilterUsername(comment.Username);
            }
            if (!string.IsNullOrEmpty(comment.CommentToUsername))
            {
                if (string.IsNullOrEmpty(comment.CommentToNickname) || comment.CommentToNickname == comment.CommentToUsername)
                {
                    comment.CommentToNickname = Services.UserService.FilterUsername(comment.CommentToUsername);
                }
                comment.Content = $"回复@{comment.CommentToNickname}:{comment.Content}";
            }
            
        }
    }
}
