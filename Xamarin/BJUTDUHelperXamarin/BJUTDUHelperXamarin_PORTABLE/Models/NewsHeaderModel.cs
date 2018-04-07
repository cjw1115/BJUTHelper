using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BJUTDUHelperXamarin.Models
{
    public class NewsHeaderModel
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string ImageUri { get; set; }

        public int NewsID { get; set; }
        public string ContentUri { get; set;}
    }
}
