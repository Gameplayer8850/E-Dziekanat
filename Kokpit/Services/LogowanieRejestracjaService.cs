using Kokpit.Dane;
using Kokpit.Models;
using Shared.BazaDanych;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Kokpit.Services
{
    public class LogowanieRejestracjaService
    {
        public AutoryzacjaModel Autoryzuj(LogowanieModel model)
        {
            byte[] hashByte = new SHA256Managed().ComputeHash(Encoding.UTF8.GetBytes(model.password));
            var hash = new System.Text.StringBuilder();
            foreach (byte x in hashByte)
            {
                hash.Append(x.ToString("x2"));
            }
            string passwordHash = hash.ToString();

            SqlCommand command = new SqlCommand(LogowanieRejestracjaRes.ResourceManager.GetString("sqlCmdAutoryzacja"));
            command.Parameters.Add(new SqlParameter("login", model.login));
            command.Parameters.Add(new SqlParameter("password", passwordHash));
            DataTable dt=BdPolaczenie.ZwrocDane(command);
            if (dt != null && dt.Rows.Count > 0) 
                return new AutoryzacjaModel() 
                { 
                    id_uzytkownika = Convert.ToInt32(dt.Rows[0][0]), 
                    kod_roli = dt.Rows[0][1].ToString() 
                };
            else return null;
        }
    }
}
