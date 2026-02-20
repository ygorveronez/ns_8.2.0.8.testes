using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem
{
    public class RetornoConciliacaoFinanceira
    {
        public bool isSucesso { get; set; }
        public List<ErrosIntegracao> erros { get; set; }
        public List<ErrosIntegracao> avisos { get; set; }
        public List<ResultadoConciliacaoFinanceira> resultado { get; set; }
    }
}
