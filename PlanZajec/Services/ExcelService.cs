using ExcelDataReader;
using PlanZajec.Dane;
using PlanZajec.Models;
using Shared.BazaDanych;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanZajec.Services
{
    public class ExcelService
    {
        public bool Zapisz_w_bazie(string nazwa_pliku, int numer_grupy)
        {
            List<PlanDniaModel> plan = Zwroc_plan(nazwa_pliku, numer_grupy);
            if(plan!=null && plan.Count > 0)
            {
                SqlCommand command = new SqlCommand(PlanZajecRes.ResourceManager.GetString("sqlCmdPodmienWersjePlanu"));
                command.Parameters.Add(new SqlParameter("nazwa", nazwa_pliku));
                command.Parameters.Add(new SqlParameter("data", DateTime.Now));
                DataTable dt = BdPolaczenie.ZwrocDane(command);
                if(dt!=null && dt.Rows.Count > 0)
                {
                    int id_wersji = Convert.ToInt32(dt.Rows[0][0]);
                    foreach (PlanDniaModel dzien in plan)
                    {
                        foreach (ZajecieModel zajecia in dzien.Zajecia)
                        {
                            SqlCommand command1 = new SqlCommand(PlanZajecRes.ResourceManager.GetString("sqlCmdDodajZajecie"));
                            command1.Parameters.Add(new SqlParameter("id_planu", id_wersji));
                            command1.Parameters.Add(new SqlParameter("id_przedmiotu", zajecia.Id_przedmiotu));
                            command1.Parameters.Add(new SqlParameter("dzien", dzien.Dzien));
                            command1.Parameters.Add(new SqlParameter("godz_rozp", zajecia.GodzRozp));
                            command1.Parameters.Add(new SqlParameter("godz_zakon", zajecia.GodzZakon));
                            BdPolaczenie.ZwrocDane(command1);
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        public List<PlanDniaModel> Zwroc_plan(string nazwa_pliku, int numer_semestru)
        {
            List<PlanDniaModel> result = new List<PlanDniaModel>();
            try
            {
                using (var stream = File.Open(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,nazwa_pliku), FileMode.Open, FileAccess.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        do
                        {
                            reader.Read();
                            int col_start = -1;
                            int col_stop = -1;
                            foreach (CellRange cell in reader.MergeCells)
                            {
                                if (cell.FromRow == 0)
                                {
                                    if (reader.GetValue(cell.FromColumn).ToString().ToLower() == "semestr " + numer_semestru)
                                    {
                                        col_start = cell.FromColumn;
                                        col_stop = cell.ToColumn;
                                        break;
                                    }
                                }
                            }
                            if (col_start == -1) continue;
                            PlanDniaModel dzien = null;
                            
                            while (reader.Read())
                            {
                                ZajecieModel zajecie = null;
                                for (int column = col_start; column < col_stop + 1; column++)
                                {
                                    string wartosc = reader.GetValue(column) == null ? "" : reader.GetValue(column).ToString();
                                    if (column == col_start)
                                    {
                                        zajecie = null;
                                        if (wartosc.Contains("Dzien"))
                                        {
                                            if (dzien != null)
                                            {
                                                result.Add(dzien);
                                            }
                                            int start = wartosc.IndexOf("[") + 1;
                                            dzien = new PlanDniaModel()
                                            {
                                                Dzien = Convert.ToDateTime(wartosc.Substring(start, wartosc.IndexOf("]") - start)),
                                                Zajecia = new List<ZajecieModel>()
                                            };
                                            break;
                                        }
                                        else if(wartosc.Contains("-") && wartosc.Contains(":")) {
                                            zajecie = new ZajecieModel();
                                            zajecie.GodzRozp = Convert.ToDateTime(wartosc.Substring(0, wartosc.IndexOf("-")).Trim());
                                            zajecie.GodzZakon= Convert.ToDateTime(wartosc.Substring(wartosc.IndexOf("-")+1).Trim());
                                        }
                                        else
                                        {
                                            zajecie = null;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (zajecie != null && wartosc!="")
                                        {
                                            ZajecieModel zajecie1 = new ZajecieModel() { 
                                                GodzRozp=zajecie.GodzRozp,
                                                GodzZakon=zajecie.GodzZakon,
                                                Id_przedmiotu = Convert.ToInt32(wartosc.Substring(1, wartosc.IndexOf("]") - 1))
                                            };
                                            dzien.Zajecia.Add(zajecie1);
                                        }
                                    }
                                }

                            }
                            if(dzien!=null) result.Add(dzien);

                        } while (reader.NextResult());
                    }
                }
            }
            catch
            {
                return null;
            }
            return result;
        }
    }
}
