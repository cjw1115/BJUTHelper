using System;
using System.Collections.Generic;
using System.Text;

namespace BJUTDUHelperXamarin.Models.MyBJUT
{
    public class Result
    {
        public int Code { get; set; }
        public string Msg { get; set; }
    }
    public class MomentResult : Result
    {
        public IList<MomentViewModel> Data { get; set; }
    }

    public class MomentCommentResult : Result
    {
        public IList<MomentCommentsViewModel> Data { get; set; }
    }
}
