using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem
{
    public class RetornoConsulta
    {
        public bool isSucesso { get; set; }
        public List<ResultadoConsulta> resultado { get; set; }
    }
}