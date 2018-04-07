using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BJUTDUHelperXamarin.Models
{
    public class GpaModel
    {
        public string Name { get; set; }

        public List<Section> Sections { get; set; }
        public class Section
        {
            public double Score { get; set; }
            public double Point { get; set; }
        }

        public override string ToString()
        {
            return this.Name;
        }
        public double GetPoint(double score)
        {
            var sctions=Sections.OrderBy(m => m.Score).ToArray();
            for (int i = 0; i < sctions.Length; i++)
            {
                if (sctions[i].Score > score)
                    return sctions[i-1].Point;
            }
            return sctions[sctions.Length - 1].Point;

        }
        public string GetMethodDetail()
        {
            string detail = string.Empty;
            var query= Sections.OrderBy(m => m.Score);
            foreach (var item in query)
            {
                detail += $"{item.Score}-{item.Point} ";
            }
            return detail;
        }
    }
}
