using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Auditoria
{
    public class FiltroPesquisaRelatorioAuditoria
    {
        public List<int> CodigosUsuario { get; set; }
        public DateTime? DataFinal { get; set; }
        public DateTime? DataInicial { get; set; }
        public List<string> Menus { get; set; }
        public string AcaoRealizada { get; set; }
    }
}
