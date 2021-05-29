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
            //return (new DokumentyService().ZwrocTopNajnowszychDokumentow(model));
            return (new DokumentyService().ZwrocTopNajnowszychDokumentow(model));
        }

        [Route("dokumenty/pobierz")]
        [HttpGet]
        public IHttpActionResult PobierzDokument([FromBody] ZapytaniePobierzDokumentModel model)
        {
            PlikModel plik = new DokumentyService().PobierzDokument(model);
            plik.Response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
            {
                FileName = plik.Nazwa_pliku
            };
            plik.Response.Content.Headers.ContentType = new MediaTypeHeaderValue(plik.Content_type);
            return ResponseMessage(plik.Response);
        }

        [Route("dokumenty/pobierz2")]
        [HttpGet]
        public IHttpActionResult Test()
        {
            byte[] plikByte = new DokumentyService().PobierzTabliceByte(14);

            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(plikByte)
            };

            result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
            {
                FileName = "test.txt"
            };
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("text/*");


            var response = ResponseMessage(result);

            return response;
        }
    }
}
