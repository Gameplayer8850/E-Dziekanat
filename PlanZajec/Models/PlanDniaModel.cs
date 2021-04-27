using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanZajec.Models
{
    public class PlanDniaModel
    {
        public DateTime Dzien { get; set; }
        public string DzienTygodnia { get; set; }
        public List<ZajecieModel> Zajecia { get; set; }
    }
}
