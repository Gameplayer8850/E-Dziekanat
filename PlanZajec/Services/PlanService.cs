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
                    if (data.DayOfWeek == DayOfWeek.Saturday || data.DayOfWeek == DayOfWeek.Sunday)
                    {
                        do
                        {
                            data = data.AddDays(1);
                        } while (data.DayOfWeek == DayOfWeek.Sunday);
                    }
                    data = data.AddDays(-((int)data.DayOfWeek - 1));
                    return PlanNaTydzienStudent(data, data.AddDays(4), id_uzytkownika);
                }
                else if (tryb == 'Z')
                {
                    if (data.DayOfWeek != DayOfWeek.Saturday && data.DayOfWeek != DayOfWeek.Sunday)
                    {
                        do
                        {
                            data = data.AddDays(1);
                        } while (data.DayOfWeek != DayOfWeek.Saturday);
                    }
                    data = data.AddDays(-((int)data.DayOfWeek - 6));
                    return PlanNaTydzienStudent(data, data.AddDays(1), id_uzytkownika);
                }
                return null;
            }
            else if (kod_roli == "wykladowca")
            {
                if (data.DayOfWeek == DayOfWeek.Saturday || data.DayOfWeek == DayOfWeek.Sunday)
                {
                    do
                    {
                        data = data.AddDays(1);
                    } while (data.DayOfWeek == DayOfWeek.Sunday);
                }
                data = data.AddDays(-((int)data.DayOfWeek - 1));
                return PlanNaTydzienWykladowca(data, data.AddDays(6), id_uzytkownika); 
            }
            return null;
        }
        public PlanTygodniaModel PlanNaTydzienStudent(DateTime dataOd, DateTime dataDo, int id_uzytkownika)
        {
            List<PlanDniaModel> tydzien = new List<PlanDniaModel>();

            GrupaSemestr gs = (new DaneUzytkownikaService()).ZwrocIdGrupyUzytkownika(id_uzytkownika);
            if (gs.id_semestru == 0) return null;

            for (int i = 0; i < (dataDo - dataOd).TotalDays+1; i++)
            {
                tydzien.Add(PlanNaDzienStudent(dataOd.AddDays(i), gs.id_semestru, gs.id_grupy));
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
            List<PlanDniaModel> tydzien = new List<PlanDniaModel>();

            for (int i = 0; i < (dataDo - dataOd).TotalDays + 1; i++)
            {
                tydzien.Add(PlanNaDzienWykladowca(dataOd.AddDays(i), id_uzytkownika));
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
        public PlanDniaModel PlanNaDzienWykladowca(DateTime dzien, int id_uzytkownika)
        {
            SqlCommand command = new SqlCommand(PlanZajecRes.ResourceManager.GetString("sqlCmdZwrocPlanWykladowcy"));
            command.Parameters.Add(new SqlParameter("dzien", dzien.ToString("MM/dd/yyyy")));
            command.Parameters.Add(new SqlParameter("id_uzytkownika", id_uzytkownika));
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

        public PlanTygodniaModel ZwrocPlanUzytkownika(DateTime dataOd, DateTime dataDo, int id_uzytkownika, string kod_roli)
        {
            if (kod_roli == "student") return PlanNaTydzienStudent(dataOd, dataDo, id_uzytkownika);
            else if (kod_roli == "wykladowca") return PlanNaTydzienWykladowca(dataOd, dataDo, id_uzytkownika);
            return null;
        }
    }
}
