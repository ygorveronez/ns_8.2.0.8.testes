using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Cebrace
{
    public struct CancelamentoCarga
    {
        [JsonProperty(PropertyName = "protocolointegracaoCarga")]
        public string ProtocoloIntegracaoCarga { get; set; }

        [JsonProperty(PropertyName = "cancelamentoRealizado")]
        public bool CancelamentoRealizado { get; set; }

        [JsonProperty(PropertyName = "duplicarCarga")]
        public bool DuplicarCarga { get; set; }
    }
}
