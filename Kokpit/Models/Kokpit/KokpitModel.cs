using Kokpit.Models.AktualnosciPowiadomienia;
using PlanZajec.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kokpit.Models.Kokpit
{
    public class KokpitModel
    {
        public string Kierunek { get; set; }
        public int Rok { get; set; }
        public List<PlanDniaModel> plan { get; set; }
        public List<AktualnoscPowiadomienieModel> powiadomienia { get; set; }
    }
}
