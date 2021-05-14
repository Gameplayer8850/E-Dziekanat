using Formularze.Models.Dokumenty;
using Formularze.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Shared.Models.Autoryzacja;

namespace Kokpit.Controllers
{
    [RoutePrefix("api")]
    public class DokumentyController : ApiController
    {
        [Route("dokumenty")]
        [HttpPost]
        public void DodajDokument()
        {
            new DokumentyService().DodajDokument();
        }
    }
}
