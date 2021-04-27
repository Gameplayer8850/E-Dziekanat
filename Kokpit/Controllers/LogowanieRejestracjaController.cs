﻿using Kokpit.Models;
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
    [RoutePrefix("api/logowanie")]
    public class LogowanieRejestracjaController : ApiController
    {
        /// <summary>
        /// Ta metoda zostanie zastąpiona tworzeniem się tokena, który w sobie już będzie miał pewne informacje
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("zaloguj")]
        [HttpPost]
        public AutoryzacjaModel zaloguj([FromBody] LogowanieModel model)
        {
            return (new LogowanieRejestracjaService()).Autoryzuj(model);
        }

        [Route("przypomnij_haslo")]
        [HttpPost]
        public AutoryzacjaModel Generuj_kod([FromBody] WygenerujKodModel model)
        {
            return new LogowanieRejestracjaService().AutoryzujWygenerowanieKodu(model);
        }
        [Route("sprawdz_kod")]
        [HttpPost]
        public AutoryzacjaModel Sprawdz_kod([FromBody] WygenerowanyKodModel model)
        {
            return new LogowanieRejestracjaService().AutoryzujPoprawnoscWygenerowanegoKodu(model);
        }
        [Route("zmien_haslo")]
        [HttpPost]
        public AutoryzacjaModel Zmien_haslo([FromBody] ZmienHasloModel model)
        {
            return new LogowanieRejestracjaService().ZmienHaslo(model);
        }
        
    }
}
