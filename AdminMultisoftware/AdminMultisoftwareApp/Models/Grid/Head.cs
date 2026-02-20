using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminMultisoftwareApp.Models.Grid
{
    public class Head
    {

        public string title { get; set; }
        public string data { get; set; }
        public string width { get; set; }
        public bool orderable { get; set; }
        public bool visible { get; set; }
        public string className { get; set; }
        public bool tabletHide { get; set; }
        public bool phoneHide { get; set; }
        public int position { get; set; }
        public AdminMultisoftware.Dominio.Enumeradores.TipoSumarizacao sumary { get; set; }
        public bool enableGroup { get; set; }
        public int dynamicCode { get; set; }
        
    }
}
