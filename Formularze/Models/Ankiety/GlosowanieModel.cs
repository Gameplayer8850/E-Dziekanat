using Shared.Models.Autoryzacja;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Formularze.Models.Ankiety
{
    public class GlosowanieModel : AutoryzacjaModel
    {
        public int IdAnkiety { get; set; }
        public int IdWyboru { get; set; }
        public string Tresc { get; set; }
    }
}
