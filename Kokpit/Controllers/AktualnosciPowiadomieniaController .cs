using Kokpit.Models.AktualnosciPowiadomienia;
using Kokpit.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Shared.Models.Autoryzacja;

namespace Kokpit.Controllers
{
    [RoutePrefix("api/kokpit")]
    public class AktualnosciPowiadomieniaController : ApiController
    {
        [Route("aktualnosci")]
        [HttpPost]
        public TopListAktualnosciPowiadomienModel TopNajnowszychAktualnosci([FromBody] ZapytanieTopAktualnosciPowiadomieniaModel model)
        {
            return (new AktualnosciPowiadomieniaService().ZwrocTopNajnowszychAktualnosci(model));
        }
    }
}
