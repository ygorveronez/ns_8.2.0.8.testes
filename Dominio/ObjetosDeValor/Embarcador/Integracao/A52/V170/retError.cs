using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.A52.V170
{
    public class retError
    {
        public string statusCode { get; set; }

        public List<object> message { get; set; }

        public string error { get; set; }
    }
}