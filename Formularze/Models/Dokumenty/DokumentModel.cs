using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Net.Http;

namespace Formularze.Models.Dokumenty
{
    public class DokumentModel
    {
        public int Id_dokumentu { get; set; }
        public string Nazwa_dokumentu { get; set; }
        public DateTime Data_modyfikacji_pliku { get; set; }
        public DateTime Data_wrzuceniu_pliku { get; set; }
        
        public string Przesylajacy { get; set; }
    }
}
