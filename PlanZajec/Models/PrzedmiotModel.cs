using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanZajec.Models
{
    public class PrzedmiotModel
    {
        public string Nazwa { get; set; }
        public char Typ { get; set; }
        public string ImieWykladowcy { get; set; }
        public string NazwiskoWykladowcy { get; set; }
        public string Opis { get; set; }
        public string LinkDoKursu { get; set; }
        public string DodatkoweMaterialy { get; set; }
        public int IloscGodzin { get; set; }
        public int Ects { get; set; }
    }
}
