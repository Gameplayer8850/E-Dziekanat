using Kokpit.Dane;
using Kokpit.Models.Kokpit;
using PlanZajec.Models;
using PlanZajec.Services;
using Shared.BazaDanych;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kokpit.Services
{
    public class KokpitService
    {
        public KokpitModel ZwrocKokpit(int id_uzytkownika, string kod_roli)
        {
            Object[] dane = ZwrocPodstawoweDane(id_uzytkownika);
            
            return new KokpitModel()
            {
                Kierunek = dane!=null? dane[0]!=null&&dane[0]!=DBNull.Value?Convert.ToString(dane[0]):null:null,
                Rok = dane!=null?dane[1]!=null && dane[1] != DBNull.Value ? Convert.ToInt32(dane[1]):0:0,
                plan = new PlanService().PlanKokpit(id_uzytkownika, kod_roli),
                powiadomienia = new AktualnosciPowiadomieniaService().ZwrocPowiadomienia(id_uzytkownika, 4)
            };
        }
        Object[] ZwrocPodstawoweDane(int id_uzytkownika)
        {
            Object[] obj = new Object[2];
            SqlCommand command = new SqlCommand(KokpitRes.ResourceManager.GetString("sqlCmdZwrocKierunekSemestr"));
            command.Parameters.Add(new SqlParameter("id_uzytkownika", id_uzytkownika));
            DataTable dt = BdPolaczenie.ZwrocDane(command);
            if (dt != null && dt.Rows.Count > 0)
            {
                obj[0] = dt.Rows[0][0];
                obj[1] = dt.Rows[0][1];
            }
            else obj = null;
            return obj;
        }


    }
}
