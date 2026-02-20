using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioTransportador
{
    public class DadosEnvioTransportador
    {
        [JsonProperty("carriers")]
        public List<DadosTransportador> Transportadores { get; set; }
    }
}
