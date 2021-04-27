using Shared.Models.Autoryzacja;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanZajec.Models
{
    public class FiltrPlanuModel : FiltryComboBoxModel
    {
        public DateTime DataOd { get; set; }
        public DateTime DataDo { get; set; }
    }
}
