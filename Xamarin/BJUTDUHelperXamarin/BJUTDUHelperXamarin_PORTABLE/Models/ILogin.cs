using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BJUTDUHelperXamarin.Models
{
    interface ILogin
    {
        Task<ImageSource> GetCheckCode(Services.HttpBaseService httpService);
    }
}
