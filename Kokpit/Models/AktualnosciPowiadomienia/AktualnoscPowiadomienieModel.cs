using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kokpit.Models.AktualnosciPowiadomienia
{
    public class AktualnoscPowiadomienieModel
    {
        public int Id_aktualnosci { get; set; }
        public string Tytul { get; set; }
        public string Tresc { get; set; }
        public byte[] Zdjecie { get; set; }
        public string Tworca { get; set; }
        public DateTime Data_wystawienia { get; set; }
    }
}
