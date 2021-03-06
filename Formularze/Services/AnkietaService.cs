using Formularze.Dane;
using Formularze.Models.Ankiety;
using Shared.BazaDanych;
using Shared.Services.Uzytkownik;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Shared.Services.Uzytkownik.DaneUzytkownikaService;

namespace Formularze.Services
{
    public class AnkietaService
    {
        public List<AnkietaModel> ListaAnkiet(int id_uzytkownika)
        {
            GrupaSemestr gs = (new DaneUzytkownikaService()).ZwrocIdGrupyUzytkownika(id_uzytkownika);
            int id_danych_osobowych = (new DaneUzytkownikaService()).ZwrocIdDanychOsobowych(id_uzytkownika);
            SqlCommand command = new SqlCommand(AnkietaRes.ResourceManager.GetString("sqlCmdListaAnkietDlaUzytkownika"));
            command.Parameters.Add(new SqlParameter("id_osoby", id_danych_osobowych));
            command.Parameters.Add(new SqlParameter("id_uzytkownika", id_uzytkownika));
            command.Parameters.Add(new SqlParameter("id_grupy", gs.id_grupy));
            DataTable dt = BdPolaczenie.ZwrocDane(command);
            List<AnkietaModel> lista = new List<AnkietaModel>();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AnkietaModel ankieta = Zwroc_Obiekt_Ankiety(dr);
                    ankieta.IdWyboru = Zwroc_twoj_glos(id_uzytkownika, ankieta.IdAnkiety);
                    lista.Add(ankieta);
                }
            }
            else lista = null;

            return lista;
        }
        public AnkietaModel Zwroc_ankiete(int id_uzytkownika, int id_ankiety)
        {
            GrupaSemestr gs = (new DaneUzytkownikaService()).ZwrocIdGrupyUzytkownika(id_uzytkownika);
            int id_danych_osobowych = (new DaneUzytkownikaService()).ZwrocIdDanychOsobowych(id_uzytkownika);
            SqlCommand command = new SqlCommand(AnkietaRes.ResourceManager.GetString("sqlCmdAnkietaPoId"));
            command.Parameters.Add(new SqlParameter("id_osoby", id_danych_osobowych));
            command.Parameters.Add(new SqlParameter("id_uzytkownika", id_uzytkownika));
            command.Parameters.Add(new SqlParameter("id_grupy", gs.id_grupy));
            command.Parameters.Add(new SqlParameter("id_ankiety", id_ankiety));
            DataTable dt = BdPolaczenie.ZwrocDane(command);
            AnkietaModel ankieta = null;
            if (dt != null && dt.Rows.Count > 0)
            {
                ankieta = Zwroc_Obiekt_Ankiety(dt.Rows[0]);
                ankieta.wybory= Zwroc_wybory_ankiety(id_ankiety);
                ankieta.IdWyboru= Zwroc_twoj_glos(id_uzytkownika, id_ankiety);
            }
            return ankieta;
        }

        public AnkietaModel Zwroc_Obiekt_Ankiety(DataRow dr)
        {
            return new AnkietaModel()
            {
                IdAnkiety = Convert.ToInt32(dr[0]!=DBNull.Value ? dr[0] : 0),
                Tresc = Convert.ToString(dr[1] != DBNull.Value ? dr[1] : ""),
                DataOd = Convert.ToDateTime(dr[2] != DBNull.Value ? dr[2] : null),
                DataDo = Convert.ToDateTime(dr[3] != DBNull.Value ?  dr[3] : null),
                CzyWlasneOdp = Convert.ToBoolean(dr[4] != DBNull.Value ? dr[4] : false),
                DataUtworzenia = Convert.ToDateTime(dr[5] != DBNull.Value ? dr[5] : null),
                ImieNazwisko = Convert.ToString(dr[6] != DBNull.Value ? dr[6] : ""),
                wybory = null,
                IdWyboru = 0
            };
        }

        public int Zwroc_twoj_glos(int id_uzytkownika, int id_ankiety)
        {
            SqlCommand command = new SqlCommand(AnkietaRes.ResourceManager.GetString("sqlCmdTwojGlosAnkieta"));
            command.Parameters.Add(new SqlParameter("id_ankiety", id_ankiety));
            command.Parameters.Add(new SqlParameter("id_uzytkownika", id_uzytkownika));
            DataTable dt = BdPolaczenie.ZwrocDane(command);
            int result = 0;
            if (dt != null && dt.Rows.Count > 0) result = Convert.ToInt32(dt.Rows[0][0]);
            return result;
        }

        public List<WyborModel> Zwroc_wybory_ankiety(int id_ankiety)
        {
            SqlCommand command = new SqlCommand(AnkietaRes.ResourceManager.GetString("sqlCmdWyboryDoAnkiety"));
            command.Parameters.Add(new SqlParameter("id_ankiety", id_ankiety));
            DataTable dt = BdPolaczenie.ZwrocDane(command);
            List<WyborModel> wybory = null;
            if (dt != null && dt.Rows.Count > 0)
            {
                wybory = new List<WyborModel>();
                foreach (DataRow dr in dt.Rows) wybory.Add(new WyborModel()
                {
                    IdWyboru = Convert.ToInt32(dr[0]),
                    Tresc = Convert.ToString(dr[1])
                });
            }
            return wybory;
        }

        public int Wlasna_Odp(int id_uzytkownika, int id_ankiety, string tresc)
        {
            int result = 0;
            if(Czy_moze_glosowac(id_uzytkownika, id_ankiety) && Czy_mozna_dodawac_wlasne_odp(id_ankiety))
            {
                SqlCommand command = new SqlCommand(AnkietaRes.ResourceManager.GetString("sqlCmdDodajWlasnaOdp"));
                command.Parameters.Add(new SqlParameter("tresc", tresc));
                command.Parameters.Add(new SqlParameter("id_ankiety", id_ankiety));
                command.Parameters.Add(new SqlParameter("id_uzytkownika", id_uzytkownika));
                DataTable dt = BdPolaczenie.ZwrocDane(command);
                if (dt != null && dt.Rows.Count > 0) result = Convert.ToInt32(dt.Rows[0][0]!=DBNull.Value ? dt.Rows[0][0] : 0);
                if (result > 0) Zaglosuj(id_uzytkownika, id_ankiety, result);
            }
            return result;
        }
        public bool Zaglosuj(int id_uzytkownika, int id_ankiety, int id_wyboru)
        {
            if (Czy_moze_glosowac(id_uzytkownika, id_ankiety))
            {
                SqlCommand command = new SqlCommand(AnkietaRes.ResourceManager.GetString("sqlCmdZaglosuj"));
                command.Parameters.Add(new SqlParameter("id_ankiety", id_ankiety));
                command.Parameters.Add(new SqlParameter("id_uzytkownika", id_uzytkownika));
                command.Parameters.Add(new SqlParameter("id_wyboru", id_wyboru));
                BdPolaczenie.ZwrocDane(command);
                return true;
            }
            return false;
        }

        public bool Czy_moze_glosowac(int id_uzytkownika, int id_ankiety)
        {
            int id_grupy = (new DaneUzytkownikaService()).ZwrocIdGrupyUzytkownika(id_uzytkownika).id_grupy;
            int id_danych_osobowych = (new DaneUzytkownikaService()).ZwrocIdDanychOsobowych(id_uzytkownika);
            SqlCommand command = new SqlCommand(AnkietaRes.ResourceManager.GetString("sqlCmdCzyUprawnienieDoAnkiety"));
            command.Parameters.Add(new SqlParameter("id_ankiety", id_ankiety));
            command.Parameters.Add(new SqlParameter("id_osoby", id_danych_osobowych));
            command.Parameters.Add(new SqlParameter("id_grupy", id_grupy));
            DataTable dt = BdPolaczenie.ZwrocDane(command);
            if (dt != null && dt.Rows.Count > 0) return true;
            else return false;
        }

        public bool Czy_mozna_dodawac_wlasne_odp(int id_ankiety)
        {
            bool result = false;
            SqlCommand command = new SqlCommand(AnkietaRes.ResourceManager.GetString("sqlCmdCzyMoznaDodawacOdp"));
            command.Parameters.Add(new SqlParameter("id_ankiety", id_ankiety));
            DataTable dt = BdPolaczenie.ZwrocDane(command);
            if (dt != null && dt.Rows.Count > 0) result = Convert.ToBoolean(dt.Rows[0][0]);
            return result;
        }
    }
}
