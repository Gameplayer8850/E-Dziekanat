using Formularze.Models.Ankiety;
using Formularze.Services;
using Shared.Models.Autoryzacja;
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
        [Route("lista_ankiet")]
        [HttpPost]
        public List<AnkietaModel> ListaAnkiet([FromBody] AutoryzacjaModel autoryzacja)
        {
            return new AnkietaService().ListaAnkiet(autoryzacja.Id_uzytkownika);
        }

        [Route("ankieta")]
        [HttpPost]
        public AnkietaModel ZwrocAnkiete([FromBody] ZwrocAnkieteModel autoryzacja)
        {
            return new AnkietaService().Zwroc_ankiete(autoryzacja.Id_uzytkownika, autoryzacja.IdAnkiety);
        }
    }
}
