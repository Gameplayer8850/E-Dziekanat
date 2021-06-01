using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Formularze.Models.Ankiety
{
    public class AnkietaModel
    {
        public int IdAnkiety { get; set; }
        public string Tresc { get; set; }
        public DateTime DataOd { get; set; }
        public DateTime DataDo { get; set; }
        public bool CzyWlasneOdp { get; set; }
        public DateTime DataUtworzenia { get; set; }
        public string ImieNazwisko { get; set; }
        public List<WyborModel> wybory { get; set; }
        public int IdWyboru { get; set; }
    }
}
