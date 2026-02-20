using System.Collections.Generic;

namespace SGT.WebAdmin.Models.Grid
{
    public class Row
    {
        public string Codigo { get; set; }
        public string CorFundo { get; set; }

        public List<Column> Colunas { get; set; }
    }
}
