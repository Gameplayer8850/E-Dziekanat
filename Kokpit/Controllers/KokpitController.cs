using Kokpit.Models.Kokpit;
using Kokpit.Services;
using Shared.Models.Autoryzacja;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Kokpit.Controllers
{
    [RoutePrefix("api/kokpit")]
    public class KokpitController : ApiController
    {
        [Route("kokpit")]
        [HttpPost]
        public KokpitModel Kokpit([FromBody] AutoryzacjaModel autoryzacja)
        {
            return new KokpitService().ZwrocKokpit(autoryzacja.Id_uzytkownika, autoryzacja.Kod_roli);
        }
    }
}
