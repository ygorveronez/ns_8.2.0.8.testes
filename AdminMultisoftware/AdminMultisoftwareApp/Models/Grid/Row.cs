using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminMultisoftwareApp.Models.Grid
{
    public class Row
    {
        public string Codigo { get; set; }
        public string CorFundo { get; set; }

        public List<Column> Colunas { get; set; }
    }
}
