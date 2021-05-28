using PlanZajec.Dane;
using PlanZajec.Models;
using Shared.BazaDanych;
using Shared.Dane.Uzytkownik;
using Shared.Services.Uzytkownik;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Shared.Services.Uzytkownik.DaneUzytkownikaService;

namespace PlanZajec.Services
{
    public class PlanService
    {
        public PlanTygodniaModel AktualnyPlan(int id_uzytkownika, string kod_roli)
        {
            DateTime data = DateTime.Now;
            if (kod_roli == "student")
            {
                char tryb = (new DaneUzytkownikaService()).ZwrocTrybStudiowUzytkownika(id_uzytkownika);
                if (tryb == 'S')
                {
                    data = ZwrocNajblizszaData(data, tryb);
                    data = data.AddDays(-((int)data.DayOfWeek - 1));
                    return PlanNaTydzienWybranegoStudenta(data, data.AddDays(6), id_uzytkownika);
                }
                else if (tryb == 'Z')
                {
                    data = ZwrocNajblizszaData(data, tryb);
                    data = data.AddDays(-((int)data.DayOfWeek - 6));
                    return PlanNaTydzienWybranegoStudenta(data.AddDays(-5), data.AddDays(1), id_uzytkownika);
                }
                return null;
            }
            else if (kod_roli == "wykladowca")
            {
                data = ZwrocNajblizszaData(data, 'S');
                data = data.AddDays(-((int)data.DayOfWeek - 1));
                return PlanNaTydzienWykladowca(data, data.AddDays(6), id_uzytkownika); 
            }
            return null;
        }

        public List<PlanDniaModel> PlanKokpit(int id_uzytkownika, string kod_roli)
        {
            DateTime data = DateTime.Now;
            DateTime data_do;
            List<PlanDniaModel> plan = new List<PlanDniaModel>();
            if (kod_roli == "student")
            {
                char tryb = (new DaneUzytkownikaService()).ZwrocTrybStudiowUzytkownika(id_uzytkownika);
                if (tryb != 'S' && tryb != 'Z') return null;
                data = ZwrocNajblizszaData(data, tryb);
                data_do = ZwrocNajblizszaData(data.AddDays(1), tryb);
                GrupaSemestr gs = (new DaneUzytkownikaService()).ZwrocIdGrupyUzytkownika(id_uzytkownika);
                plan.Add(PlanNaDzienStudent(data, gs.id_semestru, gs.id_grupy));
                plan.Add(PlanNaDzienStudent(data_do, gs.id_semestru, gs.id_grupy));
            }
            else if (kod_roli == "wykladowca")
            {
                data_do = data.AddDays(1);
                int id_danych_osobowych= (new DaneUzytkownikaService()).ZwrocIdDanychOsobowych(id_uzytkownika);
                if (id_danych_osobowych < 1) return null;
                plan.Add(PlanNaDzienWykladowca(data, id_danych_osobowych));
                plan.Add(PlanNaDzienWykladowca(data_do, id_danych_osobowych));
            }
            return plan;
        }

        DateTime ZwrocNajblizszaData(DateTime data, char tryb)
        {
            DateTime data_wynik = data;
            if (tryb == 'S')
            {
                if (data_wynik.DayOfWeek == DayOfWeek.Saturday || data_wynik.DayOfWeek == DayOfWeek.Sunday)
                {
                    do
                    {
                        data_wynik = data_wynik.AddDays(1);
                    } while (data_wynik.DayOfWeek == DayOfWeek.Sunday);
                }
            }
            else if (tryb == 'Z')
            {
                if (data_wynik.DayOfWeek != DayOfWeek.Saturday && data_wynik.DayOfWeek != DayOfWeek.Sunday)
                {
                    do
                    {
                        data_wynik = data_wynik.AddDays(1);
                    } while (data_wynik.DayOfWeek != DayOfWeek.Saturday);
                }
            }
            return data_wynik;
        }

        public PlanTygodniaModel PlanNaTydzienWybranegoStudenta(DateTime dataOd, DateTime dataDo, int id_uzytkownika)
        {
            GrupaSemestr gs = (new DaneUzytkownikaService()).ZwrocIdGrupyUzytkownika(id_uzytkownika);
            if (gs.id_semestru == 0) return null;
            return PlanNaTydzienStudent(dataOd, dataDo, gs.id_semestru, gs.id_grupy);
        }

        public PlanTygodniaModel PlanNaTydzienWybranejGrupy(DateTime dataOd, DateTime dataDo, int id_grupy)
        {
            int id_semestru = (new DaneUzytkownikaService()).ZwrocSemestrPoGrupie(id_grupy);
            if (id_semestru == 0) return null;
            return PlanNaTydzienStudent(dataOd, dataDo, id_semestru, id_grupy);
        }

        public PlanTygodniaModel PlanNaTydzienStudent(DateTime dataOd, DateTime dataDo, int id_semestru, int id_grupy)
        {
            List<PlanDniaModel> tydzien = new List<PlanDniaModel>();
            for (int i = 0; i < (dataDo - dataOd).TotalDays + 1; i++)
            {
                tydzien.Add(PlanNaDzienStudent(dataOd.AddDays(i), id_semestru, id_grupy));
            }
            return new PlanTygodniaModel
            {
                DataOd = dataOd,
                DataDo = dataDo,
                Tydzien = tydzien
            };
        }

        public PlanTygodniaModel PlanNaTydzienWykladowca(DateTime dataOd, DateTime dataDo, int id_uzytkownika)
        {        
            int id_dane_osobowe = (new DaneUzytkownikaService()).ZwrocIdDanychOsobowych(id_uzytkownika);
            if (id_dane_osobowe == 0) return null;
            return PlanNaTydzienWybranegoWykladowcy(dataOd, dataDo, id_dane_osobowe);
        }

        public PlanTygodniaModel PlanNaTydzienWybranegoWykladowcy(DateTime dataOd, DateTime dataDo, int id_dane_osobowe)
        {
            List<PlanDniaModel> tydzien = new List<PlanDniaModel>();

            for (int i = 0; i < (dataDo - dataOd).TotalDays + 1; i++)
            {
                tydzien.Add(PlanNaDzienWykladowca(dataOd.AddDays(i), id_dane_osobowe));
            }
            return new PlanTygodniaModel
            {
                DataOd = dataOd,
                DataDo = dataDo,
                Tydzien = tydzien
            };
        }

        public PlanDniaModel PlanNaDzienStudent(DateTime dzien, int id_semestru, int id_grupy)
        {
            SqlCommand command = new SqlCommand(PlanZajecRes.ResourceManager.GetString("sqlCmdZwrocPlanDlaGrupy"));
            command.Parameters.Add(new SqlParameter("id_semestru", id_semestru));
            command.Parameters.Add(new SqlParameter("id_grupy", id_grupy));
            command.Parameters.Add(new SqlParameter("dzien", dzien.ToString("MM/dd/yyyy")));
            return KonwertujDoObiektuPlanDnia(BdPolaczenie.ZwrocDane(command), dzien);
        }

        public PlanDniaModel PlanNaDzienWykladowca(DateTime dzien, int id_dane_osobowe)
        {
            SqlCommand command = new SqlCommand(PlanZajecRes.ResourceManager.GetString("sqlCmdZwrocPlanWykladowcy"));
            command.Parameters.Add(new SqlParameter("dzien", dzien.ToString("MM/dd/yyyy")));
            command.Parameters.Add(new SqlParameter("id_uzytkownika", id_dane_osobowe));
            return KonwertujDoObiektuPlanDnia(BdPolaczenie.ZwrocDane(command), dzien);
        }

        public PlanDniaModel KonwertujDoObiektuPlanDnia(DataTable dt, DateTime dzien)
        {
            CultureInfo polska = new CultureInfo("pl-PL");
            string dzienTygodnia = null;
            if (dzien != null) dzienTygodnia = polska.DateTimeFormat.GetDayName(dzien.DayOfWeek);
            if (dzienTygodnia != null && dzienTygodnia.Length > 0) dzienTygodnia = char.ToUpper(dzienTygodnia[0]) + dzienTygodnia.Substring(1);
            if (dt != null && dt.Rows.Count > 0)
            {
                List<ZajecieModel> zajecia = new List<ZajecieModel>();
                DateTime data = new DateTime();
                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        zajecia.Add(new ZajecieModel()
                        {
                            GodzRozp = data + (TimeSpan)dr[0],
                            GodzZakon = data + (TimeSpan)dr[1],
                            Nazwa = Convert.ToString(dr[2]),
                            Typ = Convert.ToChar(dr[3]) == 'W' ? "Wykład" : Convert.ToChar(dr[3]) == 'L' ? "Laboratoria" : "Ćwiczenia",
                            ImieWykladowcy = Convert.ToString(dr[4]),
                            NazwiskoWykladowcy = Convert.ToString(dr[5]),
                            Opis = Convert.ToString(dr[6]),
                            LinkDoKursu = Convert.ToString(dr[7]),
                            DodatkoweMaterialy = Convert.ToString(dr[8]),
                            IloscGodzin = Convert.ToInt32(dr[9]),
                            Ects = Convert.ToInt32(dr[10]),
                            NumerSemestru = Convert.ToInt32(dr[11]),
                            NazwaGrupy = Convert.ToString(dr[12])
                        });
                    }
                    catch (Exception)
                    {
                        zajecia.Add(null);
                    }
                }
                return new PlanDniaModel()
                {
                    Dzien = dzien,
                    DzienTygodnia = dzienTygodnia,
                    Zajecia = zajecia
                };
            }
            return new PlanDniaModel()
            {
                Dzien = dzien,
                DzienTygodnia= dzienTygodnia,
                Zajecia = null
            };
        }

        public PlanTygodniaModel ZwrocPlanUzytkownika(DateTime dataOd, DateTime dataDo, int id_pola, string kod_roli)
        {
            if (kod_roli == "student") return PlanNaTydzienWybranejGrupy(dataOd, dataDo, id_pola);
            else if (kod_roli == "wykladowca") return PlanNaTydzienWybranegoWykladowcy(dataOd, dataDo, id_pola);
            return null;
        }
        public DaneComboBoxModel WypelnijDaneComboBox(int id_uzytkownika, string kod_roli)
        {
            int domyslna_wartosc = 0;
            List<DaneComboBoxModel.Row> lista = new List<DaneComboBoxModel.Row>();
            DaneUzytkownikaService dus = new DaneUzytkownikaService();
            DataTable dt=null;
            if (kod_roli == "student")
            {
                domyslna_wartosc = dus.ZwrocIdGrupyUzytkownika(id_uzytkownika).id_grupy;
                dt = dus.ZwrocListeGrup();
            }
            else if (kod_roli == "wykladowca")
            {
                domyslna_wartosc = dus.ZwrocIdDanychOsobowych(id_uzytkownika);
                dt = dus.ZwrocListeWykladowcow();
            }
            if (dt == null || dt.Rows.Count==0) return null;
            foreach (DataRow dr in dt.Rows) 
                lista.Add(new DaneComboBoxModel.Row() 
                { 
                    value = Convert.ToInt32(dr[0]), 
                    visible = Convert.ToString(dr[1]) 
                });
            return new DaneComboBoxModel() 
            { 
                IdDomyslne = domyslna_wartosc, 
                dane = lista 
            };
        }
    }
}
