﻿using Kokpit.Dane;
using Kokpit.Models;
using Shared.BazaDanych;
using Shared.Models.Autoryzacja;
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
    }
}
