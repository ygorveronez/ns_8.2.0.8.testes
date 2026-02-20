using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever
{
    public class Envio
    {
        [JsonProperty("driverID")]
        public string IdentificaoMotorista { get; set; }

        [JsonProperty("edoc")]
        public string Edoc { get; set; }

        [JsonProperty("shipmentID")]
        public string ShipmentID { get; set; }


        [JsonProperty("vpDetails")]
        public List<DetalhesVp> DetalhesVp { get; set; }

        [JsonProperty("MS_FREIGHT")]
        public List<MSFRETE> MSFRETE { get; set; }

        [JsonProperty("MS_Carrier")]
        public List<MSPortadora> MSPortadora { get; set; }

        [JsonProperty("MS_Precheckin")]
        public List<PreCheckin> MSPrecheckin { get; set; }
    }
}
