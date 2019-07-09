using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTaskMariusBudrauskas
{
    public enum TaxType
    {
        Daily,
        Weekly,
        Monthly,
        Yearly
    }

    public class Tax
    {
        public DateTime FromPeriod { get; set; }

        public DateTime ToPeriod { get; set; }

        public DateTime AtDay { get; set; }

        public string Municipality { get; set; }

        public TaxType Type { get; set; }

        public double TaxPrice { get; set; }
    }
}
