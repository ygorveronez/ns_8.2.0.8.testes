using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Neokohm
{
    public class Destinos
    {
        [JsonProperty("id_destino")]
        public int IDUnidadeDestino { get; set; }

        [JsonProperty("latdestino")]
        public decimal LatitudeDestino { get; set; }

        [JsonProperty("longdestino")]
        public decimal LongitudeDestino { get; set; }

        [JsonProperty("descdestino")]
        public string NomeDestino { get; set; }

        [JsonProperty("municipio")]
        public string MunicipioDestino { get; set; }

        [JsonProperty("chegadadestino")]
        public string DataFinalDestino { get; set; }

        [JsonProperty("seq")]
        public int OrdemDestino { get; set; }

    }
}
