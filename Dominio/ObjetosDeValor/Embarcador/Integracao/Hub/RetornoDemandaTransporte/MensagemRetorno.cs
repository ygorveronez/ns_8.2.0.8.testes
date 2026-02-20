using Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.RetornoDemandaTransporte
{
    public class MensagemRetorno
    {
        [JsonProperty("OriginMessageId")]
        public string IdMensagemOrigem { get; set; }

        [JsonProperty("Id")]
        public string Id { get; set; }

        [JsonProperty("transportDemand")]
        public DemandaTransporte DemandaTransporte { get; set; }

        [JsonProperty("MessageType")]
        public int TipoMensagem { get; set; }

        [JsonProperty("Status")]
        public int Situacao { get; set; }

        [JsonProperty("MessageDescription")]
        public string DescricaoMensagem { get; set; }

        [JsonProperty("Errors")]
        public List<string> Erros { get; set; }
    }
}
