using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BJUTDUHelperXamarin.Models
{
    
    public class NewsModel
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Uri { get; set; }
        public string Type { get; set; }
        public string Author { get; set; }
        public DateTime PostTime { get; set; }

        public string Content { get; set; }
    }
}
