using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Formularze.Models.Dokumenty
{
    public class PlikModel
    {
        public HttpResponseMessage Response { get; set; }
        public string Nazwa_pliku { get; set; }
        public string Content_type { get; set; }
    }
}
