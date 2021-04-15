using Shared.BazaDanych;
using Shared.XML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class Zarzadzanie
    {
        public static bool NawiazPolaczenieBaza()
        {
            XMLop opxml = new XMLop();
            Config conf = null;
            if (opxml.Czy_istnieje_config()) conf = opxml.Pobierz_config();
            if(conf==null)
            {
                opxml.Stworz_szablon_configu();
                return false;
            }
            return conf.czy_wykorzystac_connection_string ? BdPolaczenie.TestujPolaczenie(conf.connection_string, true) : BdPolaczenie.TestujPolaczenie(conf.nazwa_serwera, conf.nazwa_bazy, conf.login, conf.haslo, conf.dodatkowe_ustawienia, true);
        }
    }
}
