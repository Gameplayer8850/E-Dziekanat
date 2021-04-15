using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.XML
{
    public class Config
    {
        public bool czy_wykorzystac_connection_string { get; set; } = true;
        public string connection_string { get; set; } = null;
        public string nazwa_serwera { get; set; } = null;
        public string nazwa_bazy { get; set; } = null;
        public string login { get; set; } = null;
        public string haslo { get; set; } = null;
        public string dodatkowe_ustawienia { get; set; } = null;
        public void Stworz_szablon()
        {
            czy_wykorzystac_connection_string = true;
            connection_string = "[CONNECTION STRING]";
            nazwa_serwera = "[NAZWA SERWERA]";
            nazwa_bazy = "[NAZWA BAZY]";
            login = "[LOGIN]";
            haslo = "[HASŁO]";
            dodatkowe_ustawienia = "[DODATKOWE USTAWIENIA]";
        }
    }
}
