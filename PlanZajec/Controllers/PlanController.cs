using PlanZajec.Models;
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
        [Route("aktualny_plan/{autoryzacja}")]
        [HttpGet]
        public PlanTygodniaModel AktualnyPlan(AutoryzacjaModel autoryzacja)
        {
            return new PlanTygodniaModel();
        }
    }
}
