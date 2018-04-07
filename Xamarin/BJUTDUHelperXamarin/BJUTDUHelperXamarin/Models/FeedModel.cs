using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BJUTDUHelperXamarin.Models
{
    public class FeedModel:Prism.Mvvm.BindableBase
    {
        private string _content;
        public string Content
        {
            get { return _content; }
            set { SetProperty(ref _content, value);}
        }

        private string _version;
        public string Version {
            get { return _version; }
            set { SetProperty(ref _version, value); }
        }
        private string _contact;
        public string Contact {
            get { return _contact; }
            set { SetProperty(ref _contact, value); }
        }
    }
}
