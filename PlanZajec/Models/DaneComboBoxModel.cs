using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanZajec.Models
{
    public class DaneComboBoxModel
    {
        public struct Row
        {
            public int value;
            public string visible;
        }
        public int IdDomyslne { get; set; }
        public List<Row> dane { get; set; }
    }
}
