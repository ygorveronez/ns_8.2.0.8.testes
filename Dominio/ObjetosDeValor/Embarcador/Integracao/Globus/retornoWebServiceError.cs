using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Globus
{
    public class retornoWebServiceError
    {
        public bool success { get; set; }
        public List<string> errors { get; set; }

        public string MensagemDeErro { get; set; }
    }
}
