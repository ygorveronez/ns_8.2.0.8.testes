using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar
{
    public class RetornoConsultaIdVpo
    {
        [JsonProperty("dados")]
        public RetornoDados Dados { get; set; }
    }

    public class RetornoDados
    {
        [JsonProperty("idVpos")]
        public List<string> CodigosVpos { get; set; }
    }
}
