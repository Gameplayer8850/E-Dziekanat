using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Formularze.Models.Dokumenty
{
    public class DokumentModel
    {
        public string Nazwa_dokumentu { get; set; }
        public DateTime Data_modyfikacji_pliku { get; set; }
        public DateTime Data_wrzuceniu_pliku { get; set; }
        public byte Plik { get; set; }
    }
}
