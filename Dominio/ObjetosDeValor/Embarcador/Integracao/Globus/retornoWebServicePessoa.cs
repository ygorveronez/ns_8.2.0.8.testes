using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Globus
{
    public class retornoWebServicePessoa
    {
        public bool success { get; set; }
        public DataResponse data { get; set; }
        public List<string> errors { get; set; }
    }
}
