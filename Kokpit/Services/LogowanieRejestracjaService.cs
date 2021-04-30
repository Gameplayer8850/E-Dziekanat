using Kokpit.Dane;
using Kokpit.Models;
using Kokpit.Models.LogowanieRejestracja;
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
                    Kod_roli = model.Kod_roli,
                    ImieNazwisko=Convert.ToString(dt.Rows[0][1])
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

        public InformacjaZwrotnaKodModel AutoryzujWygenerowanieKodu(WygenerujKodModel model)
        {
            int id = SzukajIdUzytkownikaPoLoginie(model.Login);
            if (id > 0)
            {
                string wygenerowany_kod = GenerujUnikatowyKodDlaOperacjiZapomnialemHasla();
                if (wygenerowany_kod != null)
                {
                    if(InsertDoTabeliOperacja_Zapomnialem_hasla(id, wygenerowany_kod))
                    {
                        return new InformacjaZwrotnaKodModel()
                        {
                            Poprawnosc = true,
                            Tresc = null
                        };
                    }else return new InformacjaZwrotnaKodModel()
                    {
                        Poprawnosc = false,
                        Tresc = "Nie umieszczono kodu w tabeli"
                    };

                }
                else return new InformacjaZwrotnaKodModel()
                {
                    Poprawnosc = false,
                    Tresc = "Wygenerowany_kod nie istnieje"
                };
            }else return new InformacjaZwrotnaKodModel()
            {
                Poprawnosc = false,
                Tresc = "Nie znaleziono uzytkownika"
            };
        }

        public InformacjaZwrotnaKodModel AutoryzacjaWygenerowanegoKodu(WygenerowanyKodModel model)
        {
            SqlCommand command = new SqlCommand(LogowanieRejestracjaRes.ResourceManager.GetString("sqlCmdSprawdzPoprawnoscKodu"));
            command.Parameters.Add(new SqlParameter("kod", model.Wygenerowany_kod));
            DataTable dt= BdPolaczenie.ZwrocDane(command);
            if(dt!=null && dt.Rows.Count>0) 
                return new InformacjaZwrotnaKodModel()
                {
                    Poprawnosc = Convert.ToBoolean(dt.Rows[0][0]),
                    Tresc = Convert.ToString(dt.Rows[0][1])
                };
            return new InformacjaZwrotnaKodModel()
            {
                Poprawnosc = false,
                Tresc = "Wystąpił nieoczekiwany błąd."
            };
        }

        public InformacjaZwrotnaKodModel ZmienHaslo(ZmienHasloModel model)
        {
            string passwordHash = KonwertujNaHash(model.Password);
            SqlCommand command = new SqlCommand(LogowanieRejestracjaRes.ResourceManager.GetString("sqlCmdZmienHaslo"));
            command.Parameters.Add(new SqlParameter("hash_hasla", passwordHash));
            command.Parameters.Add(new SqlParameter("wygenerowany_kod", model.Wygenerowany_kod));
            BdPolaczenie.ZwrocDane(command);
            if(SzukajHash_haslaPoWygenerowanymKodzie(model.Wygenerowany_kod) == passwordHash)
            {
                if (UstawDateWykorzystaniaWygenerowanegoKodu(model.Wygenerowany_kod))
                {
                    return new InformacjaZwrotnaKodModel
                    {
                        Poprawnosc = true,
                        Tresc = null
                    };
                }
                else return new InformacjaZwrotnaKodModel
                {
                    Poprawnosc = false,
                    Tresc = "Nie ustawiono daty wykorzystania kodu"
                };
            }
            else return new InformacjaZwrotnaKodModel
            {
                Poprawnosc = false,
                Tresc = "Nie zmieniono hasła"
            };
        }

        public bool InsertDoTabeliOperacja_Zapomnialem_hasla(int id_uzytkownika, string wygenerowany_kod)
        {
            SqlCommand command = new SqlCommand(LogowanieRejestracjaRes.ResourceManager.GetString("sqlCmdInsertDoTabeliOperacjaZapomnialemHasla"));
            command.Parameters.Add(new SqlParameter("id_uzytkownika", id_uzytkownika));
            command.Parameters.Add(new SqlParameter("wygenerowany_kod", wygenerowany_kod));
            command.Parameters.Add(new SqlParameter("data_utworzenia", DateTime.Now.ToString(formatDaty)));
            BdPolaczenie.ZwrocDane(command);
            if (SprawdzCzyWygenerowanyKodIstniejeWBazie(wygenerowany_kod))
            {
                return true;
            }
            else return false;
        }

        public bool UstawDateWykorzystaniaWygenerowanegoKodu(string wygenerowany_kod)
        {
            SqlCommand command = new SqlCommand(LogowanieRejestracjaRes.ResourceManager.GetString("sqlCmdUstawDateWykorzystaniaWygenerowanegoKodu"));
            command.Parameters.Add(new SqlParameter("data_wykorzystania", DateTime.Now.ToString(formatDaty)));
            command.Parameters.Add(new SqlParameter("wygenerowany_kod", wygenerowany_kod));
            BdPolaczenie.ZwrocDane(command);
            if (SprawdzCzyWygenerowanyKodZostalWykorzystany(wygenerowany_kod))
            {
                return true;
            }
            else return false;
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

        public string SzukajHash_haslaPoWygenerowanymKodzie(string wygenerowany_kod)
        {
            SqlCommand command = new SqlCommand(LogowanieRejestracjaRes.ResourceManager.GetString("sqlCmdSzukajHaslaPoWygenerowanymKodzie"));
            command.Parameters.Add(new SqlParameter("wygenerowany_kod", wygenerowany_kod));
            DataTable dt = BdPolaczenie.ZwrocDane(command);
            if (dt != null && dt.Rows.Count > 0)
            {
                System.Diagnostics.Debug.WriteLine("Hash_hasla = " + dt.Rows[0][0].ToString());
                return dt.Rows[0][0].ToString();
            }
            else return null;
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

        public bool SprawdzCzyWygenerowanyKodZostalWykorzystany(string wygenerowany_kod)
        {
            DataTable dt = SzukajDatyUtworzeniaOrazDatyWykorzystaniaWygenerowanegoKodu(wygenerowany_kod);
            if (dt.Rows[0][1] != null)
            {
                System.Diagnostics.Debug.WriteLine("Data wykorzystania = " + dt.Rows[0][1].ToString());
                return true;
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
