using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BJUTDUHelperXamarin.Models
{
    public class EduItemModel
    {
        public int ID { get; set; }
        public string IconUri { get; set; }
        public string Name { get; set; }
        public Type PageType { get; set; }
    }
}
