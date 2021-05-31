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
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Xml.Serialization;



namespace Formularze.Services
{
    class DokumentyService
    {
        private static string formatDaty = "yyyy-MM-dd HH:mm:ss:fff";
        
        public TopListDokumentowModel ZwrocTopNajnowszychDokumentow(ZapytanieTopNajnowszychDokumentowModel model)
        {
            DataTable dt = WczytajTopDokumentowZBazyDanych(model.Ilosc);
            if (dt != null && dt.Rows.Count > 0)
            {
                return new TopListDokumentowModel
                {
                    TopListDokumentow = KonwertujDataTableNaDokumentListModel(dt)
                };
            }
            else return null;
        }
        public HttpResponseMessage PobierzDokument(ZapytaniePobierzDokumentModel model)
        {
            DataTable dt = WczytajDokumentPoId_dokumentu(model.Id_dokumentu);
            if (dt != null && dt.Rows.Count > 0){

                var result = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent((byte[])dt.Rows[0][4])
                };
                result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                {
                    FileName = dt.Rows[0][1].ToString()
                };
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("text/*");
                return result;
            }else return null;
        }

        public byte[] PobierzTabliceByte(int id_dokumentu)
        {
            DataTable dt = WczytajDokumentPoId_dokumentu(id_dokumentu);
            return (byte[])dt.Rows[0][4];
        }
        public List<DokumentModel> KonwertujDataTableNaDokumentListModel(DataTable dt)
        {
            List<DokumentModel> list = new List<DokumentModel>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new DokumentModel
                {
                    Id_dokumentu = Convert.ToInt32(row["id_dokumentu"]),
                    Nazwa_dokumentu = row["Nazwa_dokumentu"].ToString(),
                    Data_modyfikacji_pliku = Convert.ToDateTime(row["data_modyfikacji_pliku"]),
                    Data_wrzuceniu_pliku = Convert.ToDateTime(row["data_wrzucenia_pliku"]),
                    Plik_path = row["plik_path"].ToString() + @"\" + row["Nazwa_dokumentu"].ToString(),
                    Przesylajacy = ZnajdzPrzesylajacegoPoId_przesylajacego(Convert.ToInt32(row["id_przesylajacego"])),
                });
            }
            return list;
        }
        public DataTable WczytajDokumentPoId_dokumentu(int id_dokumentu)
        {
            SqlCommand command = new SqlCommand(DokumentyRes.ResourceManager.GetString("SqlCmdPobierzDokument"));
            command.Parameters.Add(new SqlParameter("id_dokumentu", id_dokumentu));
            DataTable dt = BdPolaczenie.ZwrocDane(command);
            if (dt != null && dt.Rows.Count > 0)
            {
                return dt;
            }
            else return null;
        }
        public DataTable WczytajTopDokumentowZBazyDanych(int ilosc)
        {
            SqlCommand command = new SqlCommand(DokumentyRes.ResourceManager.GetString("SqlCmdPobierzTopIloscDokumentow"));
            command.Parameters.Add(new SqlParameter("ilosc", ilosc));
            DataTable dt = BdPolaczenie.ZwrocDane(command);
            if (dt != null && dt.Rows.Count > 0)
            {
                return dt;
            }else return null;
        }
        public string ZnajdzPrzesylajacegoPoId_przesylajacego(int id_przesylajacego)
        {
            SqlCommand command = new SqlCommand(DokumentyRes.ResourceManager.GetString("SqlCmdZnajdzPrzesylajacegoPoId_przesylajacego"));
            command.Parameters.Add(new SqlParameter("id_przesylajacego", id_przesylajacego));
            DataTable dt = BdPolaczenie.ZwrocDane(command);
            if (dt != null && dt.Rows.Count > 0)
            {
                return dt.Rows[0][0].ToString() + " " + dt.Rows[0][1].ToString();
            }
            else return null;
        }
        public void StworzDokumentZByteArray(byte[] plikByte)
        {
            string destPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "/temp/", "test.txt");
            WriteByteArray(plikByte, "xD");
            File.WriteAllBytes(destPath,plikByte);
        }
        
        
        public static void WriteByteArray(byte[] bytes, string name)
        {
            const string underLine = "--------------------------------";

            System.Diagnostics.Debug.WriteLine(name);
            System.Diagnostics.Debug.WriteLine(underLine.Substring(0,
                Math.Min(name.Length, underLine.Length)));
            System.Diagnostics.Debug.WriteLine(BitConverter.ToString(bytes));
        }

        public void DodajDokument()
        {
            string nazwa_dokumentu = "Laboratorium 1 Bazy danych.doc";
            DateTime data_modyfikacji_pliku = DateTime.Now;
            DateTime data_wrzucenia_pliku = DateTime.Now;
            int id_przesylajacego = 1;

            byte[] plik = File.ReadAllBytes(@"C:\Users\Pszemek\Desktop\E-dziekanat\Projekt github\E-Dziekanat\Formularze\Services\Laboratorium 1 Bazy danych.doc");

            //byte[] plik = new byte[stream.Length];

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
