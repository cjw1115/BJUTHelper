using System;
using System.Collections.Generic;
using System.Text;

namespace BJUTDUHelperXamarin.Models.MyBJUT
{
    public class BookModel
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Dsc { get; set; }
        public DateTime PostTime { get; set; }
        public DateTime EndTime { get; set; }
        public string ImageUri { get; set; }
    }
}
