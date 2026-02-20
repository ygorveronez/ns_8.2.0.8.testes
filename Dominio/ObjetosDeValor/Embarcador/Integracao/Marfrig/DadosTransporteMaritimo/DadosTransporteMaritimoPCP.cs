using Newtonsoft.Json;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.DadosTransporteMaritimo
{
    public sealed class DadosTransporteMaritimoPCP
    {

        [JsonProperty(PropertyName = "halal", Order = 1, Required = Required.Default)]
        public string Halal { get; set; }

        [JsonProperty(PropertyName = "statusExportacao", Order = 2, Required = Required.Default)]
        public string statusExportacao { get; set; }

        [JsonProperty(PropertyName = "dataEstufaPcp", Order = 3, Required = Required.Default)]
        [JsonConverter(typeof(Utilidades.Json.DateTimeConverter), new object[] { "yyyy-MM-ddTHH:mm:ss" })]
        public DateTime? DataEstufagem { get; set; }

        [JsonProperty(PropertyName = "remetente", Order = 4, Required = Required.Default)]
        public string Remetente { get; set; }

        [JsonProperty(PropertyName = "observacao", Order = 5, Required = Required.Default)]
        public string Observacao { get; set; }
    }
}
