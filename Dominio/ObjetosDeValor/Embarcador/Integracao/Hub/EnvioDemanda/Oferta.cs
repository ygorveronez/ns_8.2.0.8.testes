using Newtonsoft.Json;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda
{
    public partial class Oferta
    {
        [JsonProperty("releaseDateStart")]
        public DateTimeOffset? DataInicio { get; set; }

        [JsonProperty("releaseDateEnd")]
        public DateTimeOffset? DataFim { get; set; }

        [JsonProperty("freightPriceNegotiated")]
        public long PrecoFreteNegociado { get; set; }

        [JsonProperty("carrier")]
        public Transportadora Transportadora { get; set; }

        [JsonProperty("vehicle")]
        public Veiculo Veiculo { get; set; }

        [JsonProperty("validation")]
        public Validacao Validacao { get; set; }
    }
}
