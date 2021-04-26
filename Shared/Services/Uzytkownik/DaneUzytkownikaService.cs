using Shared.BazaDanych;
using Shared.Dane.Uzytkownik;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Services.Uzytkownik
{
    public class DaneUzytkownikaService
    {
        public struct GrupaSemestr
        {
            public GrupaSemestr(int idGrupy, int idSemestru)
            {
                id_grupy = idGrupy;
                id_semestru = idSemestru;
            }
            public int id_grupy;
            public int id_semestru;
        }
        public GrupaSemestr ZwrocIdGrupyUzytkownika(int id_uzytkownika)
        {
            SqlCommand command = new SqlCommand(DaneUzytkownikaRes.ResourceManager.GetString("sqlCmdZwrocIdGrupyUzytkownika"));
            command.Parameters.Add(new SqlParameter("id_uzytkownika", id_uzytkownika));
            DataTable dt = BdPolaczenie.ZwrocDane(command);
            if (dt == null || dt.Rows.Count < 1) return new GrupaSemestr(0, 0);
            return new GrupaSemestr(Convert.ToInt32(dt.Rows[0][0]), Convert.ToInt32(dt.Rows[0][1]));
        }

        public int ZwrocSemestrPoGrupie(int id_grupy)
        {
            SqlCommand command = new SqlCommand(DaneUzytkownikaRes.ResourceManager.GetString("sqlCmdZwrocIdSemestruPoGrupie"));
            command.Parameters.Add(new SqlParameter("id_grupy", id_grupy));
            DataTable dt = BdPolaczenie.ZwrocDane(command);
            if (dt == null || dt.Rows.Count < 1) return 0;
            return Convert.ToInt32(dt.Rows[0][0]);
        }

        public int ZwrocIdDanychOsobowych(int id_uzytkownika)
        {
            SqlCommand command = new SqlCommand(DaneUzytkownikaRes.ResourceManager.GetString("sqlCmdZwrocIdDanychOsobowych"));
            command.Parameters.Add(new SqlParameter("id_uzytkownika", id_uzytkownika));
            DataTable dt = BdPolaczenie.ZwrocDane(command);
            if (dt == null || dt.Rows.Count < 1) return 0;
            return Convert.ToInt32(dt.Rows[0][0]);
        }

        public char ZwrocTrybStudiowUzytkownika(int id_uzytkownika)
        {
            SqlCommand command = new SqlCommand(DaneUzytkownikaRes.ResourceManager.GetString("sqlCmdZwrocTrybStudiowUzytkownika"));
            command.Parameters.Add(new SqlParameter("id_uzytkownika", id_uzytkownika));
            DataTable dt = BdPolaczenie.ZwrocDane(command);
            if (dt != null && dt.Rows.Count > 0) return Convert.ToChar(dt.Rows[0][0]);
            return Char.MinValue;
        }

        public DataTable ZwrocListeGrup()
        {
            SqlCommand command = new SqlCommand(DaneUzytkownikaRes.ResourceManager.GetString("sqlCmdZwrocListeGrup"));
            return BdPolaczenie.ZwrocDane(command);
        }

        public DataTable ZwrocListeWykladowcow()
        {
            SqlCommand command = new SqlCommand(DaneUzytkownikaRes.ResourceManager.GetString("sqlCmdZwrocListeWykladowcow"));
            return BdPolaczenie.ZwrocDane(command);
        }
    }
}
