using Kokpit.Dane;
using Kokpit.Models;
using Shared.BazaDanych;
using Shared.Models.Autoryzacja;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Kokpit.Services
{
    public class LogowanieRejestracjaService
    {
        private static Random random = new Random();
        private static string formatDaty = "yyyy-MM-dd HH:mm:ss:fff";
        public AutoryzacjaModel Autoryzuj(LogowanieModel model)
        {
            string passwordHash = KonwertujNaHash(model.Password);

            SqlCommand command = new SqlCommand(LogowanieRejestracjaRes.ResourceManager.GetString("sqlCmdAutoryzacja"));
            command.Parameters.Add(new SqlParameter("login", model.Login));
            command.Parameters.Add(new SqlParameter("password", passwordHash));
            command.Parameters.Add(new SqlParameter("kod_roli", model.Kod_roli));
            DataTable dt=BdPolaczenie.ZwrocDane(command);
            if (dt != null && dt.Rows.Count > 0) 
                return new AutoryzacjaModel() 
                { 
                    Id_uzytkownika = Convert.ToInt32(dt.Rows[0][0]), 
                    Kod_roli = model.Kod_roli 
                };
            else return null;
        }
        public string KonwertujNaHash(string slowo)
        {
            byte[] hashByte = new SHA256Managed().ComputeHash(Encoding.UTF8.GetBytes(slowo));
            var hash = new System.Text.StringBuilder();
            foreach (byte x in hashByte)
            {
                hash.Append(x.ToString("x2"));
            }
            return hash.ToString();
        }

        public AutoryzacjaModel AutoryzujWygenerowanieKodu(WygenerujKodModel model)
        {
            int id = SzukajIdUzytkownikaPoLoginie(model.login);
            if (id > 0)
            {
                string wygenerowany_kod = GenerujUnikatowyKodDlaOperacjiZapomnialemHasla();
                if (wygenerowany_kod != null)
                {
                    InsertDoTabeliOperacja_Zapomnialem_hasla(id, wygenerowany_kod);
                    return new AutoryzacjaModel()
                    {
                        Id_uzytkownika = id,
                        Kod_roli = "test"
                    };
                }
            }
            return null;
            
        }

        public AutoryzacjaModel AutoryzujPoprawnoscWygenerowanegoKodu(WygenerowanyKodModel model)
        {
            if (SprawdzCzyWygenerowanyKodIstniejeWBazie(model.wygenerowany_kod) && SprawdzCzyGenerowanyKodJestWazny(model.wygenerowany_kod))
            {
                return new AutoryzacjaModel()
                {
                    Id_uzytkownika = 1,
                    Kod_roli = "test"
                };
            }
            else return null; 
        }

        public AutoryzacjaModel ZmienHaslo(ZmienHasloModel model)
        {
            string passwordHash = KonwertujNaHash(model.Password);
        }

        public void InsertDoTabeliOperacja_Zapomnialem_hasla(int id_uzytkownika, string wygenerowany_kod)
        {
            SqlCommand command = new SqlCommand(LogowanieRejestracjaRes.ResourceManager.GetString("sqlCmdInsertDoTabeliOperacjaZapomnialemHasla"));
            command.Parameters.Add(new SqlParameter("id_uzytkownika", id_uzytkownika));
            command.Parameters.Add(new SqlParameter("wygenerowany_kod", wygenerowany_kod));
            command.Parameters.Add(new SqlParameter("data_utworzenia", DateTime.Now.ToString(formatDaty)));
            BdPolaczenie.ZwrocDane(command);
        }
        public int SzukajIdUzytkownikaPoLoginie(string login)
        {
            SqlCommand command = new SqlCommand(LogowanieRejestracjaRes.ResourceManager.GetString("sqlCmdSzukajIdUzytkownikaPoLoginie"));
            command.Parameters.Add(new SqlParameter("login", login));
            DataTable dt = BdPolaczenie.ZwrocDane(command);
            if (dt != null && dt.Rows.Count > 0)
            {
                System.Diagnostics.Debug.WriteLine("Id uzytkownika = " + dt.Rows[0][0].ToString());
                return Convert.ToInt32(dt.Rows[0][0]);
            }
            else return 0;
        }
        public DataTable SzukajDatyUtworzeniaOrazDatyWykorzystaniaWygenerowanegoKodu(string wygenerowany_kod)
        {
            SqlCommand command = new SqlCommand(LogowanieRejestracjaRes.ResourceManager.GetString("sqlCmdSzukajDatyUtworzeniaOrazDatyWykorzystaniaWygenerowanegoKodu"));
            command.Parameters.Add(new SqlParameter("wygenerowany_kod", wygenerowany_kod));
            DataTable dt = BdPolaczenie.ZwrocDane(command);
            if (dt != null && dt.Rows.Count > 0)
                return dt;
            else return null;
        }

        public string GenerujUnikatowyKodDlaOperacjiZapomnialemHasla()
        {
            string wygenerowany_kod = GenerujLosowyCiagZnakow(32);
            if (SprawdzCzyWygenerowanyKodIstniejeWBazie(wygenerowany_kod) == true) GenerujUnikatowyKodDlaOperacjiZapomnialemHasla();
            System.Diagnostics.Debug.WriteLine("wygenerowany kod = " + wygenerowany_kod);
            return wygenerowany_kod; 
        }
        public string GenerujLosowyCiagZnakow(int dlugosc)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()";
            return new string(Enumerable.Repeat(chars, dlugosc).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public bool SprawdzCzyGenerowanyKodJestWazny(string wygenerowany_kod)
        {
            DataTable dt = SzukajDatyUtworzeniaOrazDatyWykorzystaniaWygenerowanegoKodu(wygenerowany_kod);
            if (dt != null && dt.Rows.Count > 0)
            {
                TimeSpan roznicaCzasu = DateTime.Now.Subtract(DateTime.ParseExact(dt.Rows[0][0].ToString(), formatDaty, null));
                if (roznicaCzasu.TotalSeconds <= 7200) return true;
                else return false;
            }
            else return false;
        }
        public bool SprawdzCzyWygenerowanyKodIstniejeWBazie(string wygenerowany_kod)
        {
            SqlCommand command = new SqlCommand(LogowanieRejestracjaRes.ResourceManager.GetString("sqlCmdSprawdzCzyWygenerowanyKodIstniejeWBazie"));
            command.Parameters.Add(new SqlParameter("wygenerowany_kod", wygenerowany_kod));
            DataTable dt = BdPolaczenie.ZwrocDane(command);
            if (dt != null && dt.Rows.Count > 0)
                return true;
            else return false;
        }
    }
}
