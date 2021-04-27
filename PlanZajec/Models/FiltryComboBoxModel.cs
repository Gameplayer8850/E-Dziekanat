using Shared.Models.Autoryzacja;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanZajec.Models
{
    public class FiltryComboBoxModel : AutoryzacjaModel
    {
        public string KodPlanu { get; set; }
        public int IdPola { get; set; }
    }
}
