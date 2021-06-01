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
using System.Web;
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
        public HttpResponseMessage PobierzDokument2(ZapytaniePobierzDokumentModel model)
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
        public HttpResponseMessage PobierzDokument(int id)
        {
            DataTable dt = WczytajDokumentPoId_dokumentu(id);
            if (dt != null && dt.Rows.Count > 0)
            {
                byte[] plikByte = File.ReadAllBytes(dt.Rows[0][4].ToString());
                var result = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(plikByte)
                };
                result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                {
                    FileName = dt.Rows[0][1].ToString()
                };
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("text/*");
                return result;
            }
            else return null;
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
                    Plik_path = HttpContext.Current.Request.Url.AbsoluteUri + @"pobierz/" + Convert.ToInt32(row["id_dokumentu"]),
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
    }
}
