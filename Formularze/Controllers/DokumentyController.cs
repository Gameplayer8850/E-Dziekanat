using Formularze.Models.Dokumenty;
using Formularze.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Shared.Models.Autoryzacja;
using System.Net.Http;
using System.IO;
using System.Net;
using System.Net.Http.Headers;

namespace Kokpit.Controllers
{
    [RoutePrefix("api")]
    public class DokumentyController : ApiController
    {
        [Route("dokumenty")]
        [HttpPost]
        public TopListDokumentowModel TopNajnowszychDokumentow([FromBody] ZapytanieTopNajnowszychDokumentowModel model)
        {
            return (new DokumentyService().ZwrocTopNajnowszychDokumentow(model));
        }
        [Route("dokumenty/pobierz/{id?}")]
        [HttpGet]
        public IHttpActionResult PobierzDokument(int id)
        {
            return ResponseMessage(new DokumentyService().PobierzDokument(id));
        }
    }
}
