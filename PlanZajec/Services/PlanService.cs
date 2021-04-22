using PlanZajec.Models;
using Shared.BazaDanych;
using Shared.Dane.Uzytkownik;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanZajec.Services
{
    public class PlanService
    {
        public PlanTygodniaModel PlanNaTydzienStudent(DateTime dataOd, DateTime dataDo, int id_uzytkownika)
        {
            List<PlanDniaModel> tydzien = new List<PlanDniaModel>();
            SqlCommand command = new SqlCommand(DaneUzytkownikaRes.ResourceManager.GetString("sqlCmdZwrocIdGrupyUzytkownika"));
            command.Parameters.Add(new SqlParameter("id_uzytkownika", id_uzytkownika));
            DataTable dt = BdPolaczenie.ZwrocDane(command);
            if (dt == null || dt.Rows.Count < 1) return null;
            int id_grupy =  Convert.ToInt32(dt.Rows[0][0]);
            for(int i=0; i<(dataDo-dataOd).TotalDays; i++)
            {
                tydzien.Add(PlanNaDzienStudent(dataOd.AddDays(i), id_grupy));
            }
            return new PlanTygodniaModel
            {
                DataOd = dataOd,
                DataDo=dataDo,
                Tydzien=tydzien
            };
        }
        public PlanDniaModel PlanNaDzienStudent(DateTime dzien, int id_grupy)
        {
            return new PlanDniaModel();
        }
    }
}
