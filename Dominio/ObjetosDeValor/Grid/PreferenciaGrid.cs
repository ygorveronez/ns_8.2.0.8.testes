using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Grid
{
    public class Column
    {
        public string name { get; set; }
        public bool visible { get; set; }
        public string width { get; set; }
        public int position { get; set; }
    }

    public class PreferenciaGrid
    {
        public List<Column> columns { get; set; }
        public bool scrollHorizontal { get; set; }
    }
}
