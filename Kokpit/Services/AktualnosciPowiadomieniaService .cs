using Kokpit.Dane;
using Kokpit.Models.AktualnosciPowiadomienia;
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

namespace Kokpit.Services
{
    class AktualnosciPowiadomieniaService
    {
        public TopListAktualnosciPowiadomienModel ZwrocTopNajnowszychAktualnosci(ZapytanieTopAktualnosciPowiadomieniaModel model)
        {
            DataTable dt = PobierzTopIloscNajnowszychAktualnosciZBazy(model.Ilosc);
            if (dt != null && dt.Rows.Count > 0)
            {
                return new TopListAktualnosciPowiadomienModel
                {
                    TopListAktualnosciPowiadomien = KonwertujDataTableNaAktualnoscPowiadomienieListModel(dt)
                };
            }
            else return null;
        }

        public List<AktualnoscPowiadomienieModel> KonwertujDataTableNaAktualnoscPowiadomienieListModel(DataTable dt)
        {
            List<AktualnoscPowiadomienieModel> list = new List<AktualnoscPowiadomienieModel>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new AktualnoscPowiadomienieModel
                {
                    Id_aktualnosci = Convert.ToInt32(row["id_aktualnosci"]),
                    Tytul = row["Tytul"].ToString(),
                    Tresc = row["Tresc"].ToString(),
                    Zdjecie = ObjectToByteArray(row["Zdjecie"]),
                    Tworca = ZnajdzImieINazwiskoTworcyPoId_tworcy(row["id_tworcy"].ToString()),
                    Data_wystawienia = Convert.ToDateTime(row["Data_wystawienia"])
                });
            }
            return list;
        }
        byte[] ObjectToByteArray(object obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public string ZnajdzImieINazwiskoTworcyPoId_tworcy(string id_tworcy)
        {
            SqlCommand command = new SqlCommand(AktualnosciPowiadomieniaRes.ResourceManager.GetString("sqlCmdZnajdzImieINazwiskoTworcyPoId_tworcy"));
            command.Parameters.Add(new SqlParameter("id_tworcy", id_tworcy));
            DataTable dt = BdPolaczenie.ZwrocDane(command);
            if (dt != null && dt.Rows.Count > 0)
            {
                return dt.Rows[0][0].ToString() + " " + dt.Rows[0][1].ToString();
            }
            else return null;
        }

        public DataTable PobierzTopIloscNajnowszychAktualnosciZBazy(int ilosc)
        {
            SqlCommand command = new SqlCommand(AktualnosciPowiadomieniaRes.ResourceManager.GetString("sqlCmdPobierzTopIloscNajnowszychAktualnosci"));
            command.Parameters.Add(new SqlParameter("ilosc", ilosc));
            DataTable dt = BdPolaczenie.ZwrocDane(command);
            if (dt != null && dt.Rows.Count == ilosc)
            {
                return dt;
            }
            else return null;
        }
    }
}
