using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem
{
    public class RetornoIntegracao
    {
        public bool isSucesso { get; set; }
        public List<ErrosIntegracao> erros { get; set; }
        public List<ErrosIntegracao> avisos { get; set; }
        public Resultado resultado { get; set; }
    }
}