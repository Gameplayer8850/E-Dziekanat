using PlanZajec.Models;
using PlanZajec.Services;
using Shared.Models.Autoryzacja;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace PlanZajec.Controllers
{
    [RoutePrefix("api/plan")]
    public class PlanController : ApiController
    {
        //[Route("aktualny_plan/{autoryzacja}")]
        //[HttpGet]
        [Route("aktualny_plan")]
        [HttpPost]
        public PlanTygodniaModel AktualnyPlan([FromBody] AutoryzacjaModel autoryzacja)
        {
            return (new  PlanService()).AktualnyPlan(autoryzacja.Id_uzytkownika, autoryzacja.Kod_roli);
        }
    }
}
