using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.BazaDanych
{
    public class BdPolaczenie
    {
        static SqlConnection connection = null;

        /// <summary>
        /// Konfiguruje i testuje połączenie. Należy wywołać przy uruchomieniu systemu
        /// </summary>
        /// <param name="connection_string"></param>
        /// <returns></returns>
        public static bool TestujPolaczenie(string connection_string, bool czy_zapisac_polaczenie=false)
        {
            bool result = true;
            try
            {
                SqlConnection con = new SqlConnection(connection_string);
                con.Open();
                con.Close();
                if(czy_zapisac_polaczenie) connection = con;
            }
            catch (Exception)
            {
                result = false;
                if (czy_zapisac_polaczenie) connection = null;
            }
            return result;
        }
        public static bool TestujPolaczenie(string nazwa_serwera, string nazwa_bazy, string login, string haslo, string dodatkowe_dane, bool czy_zapisac_polaczenie = false)
        {
            return TestujPolaczenie($"data source='{nazwa_serwera}';Connect Timeout=30;persist security info=False;initial catalog='{nazwa_bazy}';User Id='{login}';Password='{haslo}';{dodatkowe_dane};", czy_zapisac_polaczenie);
        }

        /// <summary>
        /// Queruje i zwraca tabele. W celu prawidłowego działania, należy wcześniej wywołać metodę "TestujPolaczenie", która skonfiguruje połączenie z bazą.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static DataTable ZwrocDane(SqlCommand command)
        {
            command.Connection = connection;
            if (connection == null) return null;
            connection.Open();
            SqlDataAdapter adapter = new SqlDataAdapter((SqlCommand)command);
            adapter.SelectCommand.Connection = connection;
            DataSet ds = new DataSet();
            int ilosc_prob = 3;
            bool result = false;
            do
            {
                try
                {
                    adapter.Fill(ds);
                    result = true;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    System.Threading.Thread.Sleep(200);
                }
                ilosc_prob--;
            } while (ilosc_prob > 0 && !result) ;
                
            connection.Close();

            return (result && ds != null && ds.Tables != null && ds.Tables.Count > 0) ? ds.Tables[0] : null;
        }
    }
}
