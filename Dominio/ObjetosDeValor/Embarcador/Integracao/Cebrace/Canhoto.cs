using System;
using System.Collections.Generic;
using Newtonsoft.Json;


namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Cebrace
{
    public class Canhoto
    {
        [JsonProperty(PropertyName = "DATA_FIM")]
        public string DataFim { get; set; }

        [JsonProperty(PropertyName = "HORA_FIM")]
        public string HoraFim { get; set; }

        [JsonProperty(PropertyName = "PROTOCOLO_NFE")]
        public string ProtocoloNFe { get; set; }

        [JsonProperty(PropertyName = "PROTOCOLO_REMESSA")]
        public string ProtocoloRemessa { get; set; }

        [JsonProperty(PropertyName = "PROTOCOLO_TRANSPORTE")]
        public string ProtocoloTransporte { get; set; }

        [JsonProperty(PropertyName = "REMESSA")]
        public string Remessa { get; set; }

        [JsonProperty(PropertyName = "TRANSPORTE")]
        public string Transporte { get; set; }
    }
}
