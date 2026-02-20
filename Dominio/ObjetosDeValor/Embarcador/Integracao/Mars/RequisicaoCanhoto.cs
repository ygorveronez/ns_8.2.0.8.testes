using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Mars
{
    public class RequisicaoCanhoto
    {
        [JsonProperty("ShipmentDocument")]
        public string CodigoCargaEmbarcador { get; set; }

        [JsonProperty("CteData")]
        public List<CTe> CTes { get; set; }

        [JsonProperty("Release")]
        public bool Liberado { get; set; }

        [JsonProperty("ProcessDate")]
        public DateTime Data { get; set; }
    }
}
