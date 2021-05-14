using Formularze.Dane;
using Formularze.Models.Dokumenty;
using Shared.BazaDanych;
using Shared.Models.Autoryzacja;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Formularze.Services
{
    class DokumentyService
    {
        private static string formatDaty = "yyyy-MM-dd HH:mm:ss:fff";
        public void DodajDokument()
        {
            string nazwa_dokumentu = "Dokument_1";
            DateTime data_modyfikacji_pliku = DateTime.Now;
            DateTime data_wrzucenia_pliku = DateTime.Now;
            int id_przesylajacego = 1;

            FileStream stream = File.OpenRead(@"C:\Users\Pszemek\Desktop\E-dziekanat\Projekt github\E-Dziekanat\Formularze\Services\test.txt");
            byte[] plik = new byte[stream.Length];
            SqlCommand command = new SqlCommand(DokumentyRes.ResourceManager.GetString("SqlCmdDodajDokument"));
            command.Parameters.Add(new SqlParameter("nazwa_dokumentu", nazwa_dokumentu));
            command.Parameters.Add(new SqlParameter("data_modyfikacji_pliku", data_modyfikacji_pliku));
            command.Parameters.Add(new SqlParameter("data_wrzucenia_pliku", data_wrzucenia_pliku));
            command.Parameters.Add(new SqlParameter("plik", plik));
            command.Parameters.Add(new SqlParameter("id_przesylajacego", id_przesylajacego));
            BdPolaczenie.ZwrocDane(command);
        }
    }
}
