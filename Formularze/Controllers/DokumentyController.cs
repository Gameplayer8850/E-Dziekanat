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
using System.Net;
using System.IO;
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
            //return (new DokumentyService().ZwrocTopNajnowszychDokumentow(model));
            return (new DokumentyService().ZwrocTopNajnowszychDokumentow(model));
        }

        [Route("dokumenty/pobierz")]
        [HttpGet]
        public System.Web.Mvc.ActionResult PobierzDokument([FromBody] ZapytaniePobierzDokumentModel model)
        {
            return (new DokumentyService().PobierzDokument2(model));
        }
        [Route("dokumenty/pobierz2")]
        [HttpGet]
        public IHttpActionResult Test()
        {
            var stream = new MemoryStream();

            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(stream.GetBuffer())
            };
            result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
            {
                FileName = "test.pdf"
            };
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            var response = ResponseMessage(result);

            return response;
        }
    }
}
