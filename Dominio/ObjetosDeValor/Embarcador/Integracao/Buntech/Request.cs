using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Buntech
{
    public class Request
    {
        [JsonProperty("cxFilial")]
        public string Filial { get; set; }

        [JsonProperty("cxNota")]
        public string Nota{ get; set; }

        [JsonProperty("cxSerie")]
        public string Serie { get; set; }

        [JsonProperty("dxEntrega")]
        public string DataEntrega { get; set; }
    }
}
