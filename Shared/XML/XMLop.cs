using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.XML
{
    public class XMLop
    {
        string lokalizacja_danych = null;
        public XMLop()
        {
            lokalizacja_danych= Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["dane_folder"].ToString());
        }

        public bool Czy_istnieje_config()
        {
            Directory.CreateDirectory(lokalizacja_danych);
            return File.Exists(Path.Combine(lokalizacja_danych, ConfigurationManager.AppSettings["config_plik"].ToString()));
        }

        public Config Pobierz_config()
        {
            Config obj = null;
            try
            {
                using (var stream = new FileStream(Path.Combine(lokalizacja_danych, ConfigurationManager.AppSettings["config_plik"].ToString()), FileMode.Open))
                {
                    var XML = new System.Xml.Serialization.XmlSerializer(typeof(Config));
                    obj = (Config)XML.Deserialize(stream);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return null;
            }
            return obj;
        }

        public void Stworz_szablon_configu()
        {
            Config obj = new Config();
            obj.Stworz_szablon();
            using (var stream = new FileStream(Path.Combine(lokalizacja_danych, ConfigurationManager.AppSettings["config_plik"].ToString()), FileMode.Create))
            {
                var XML = new System.Xml.Serialization.XmlSerializer(typeof(Config));
                XML.Serialize(stream, obj);
            }
        }
    }
}
