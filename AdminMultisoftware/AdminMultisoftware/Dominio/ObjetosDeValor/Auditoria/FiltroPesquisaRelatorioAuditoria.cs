using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminMultisoftware.Dominio.ObjetosDeValor.Auditoria
{
    public class FiltroPesquisaRelatorioAuditoria
    {

        public List<int> CodigosUsuario { get; set; }

        public DateTime? DataFinal { get; set; }

        public DateTime? DataInicial { get; set; }
    }
}