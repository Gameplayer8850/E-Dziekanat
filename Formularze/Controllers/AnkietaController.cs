using Formularze.Models.Ankiety;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Formularze.Controllers
{
    [RoutePrefix("api/ankieta")]
    public class AnkietaController : ApiController
    {
        public List<AnkietaModel> ListaAnkiet()
        {
            return null;
        }
    }
}
