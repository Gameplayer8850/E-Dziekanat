using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanZajec.Models
{
    public class ZajecieModel : PrzedmiotModel
    {
        public DateTime GodzRozp { get; set; }
        public DateTime GodzZakon { get; set; }
    }
}
