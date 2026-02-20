using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Neokohm
{
    public class Origens
    {
        [JsonProperty("id_origem")]
        public int IDUnidadeOrigem { get; set; }

        [JsonProperty("latorigem")]
        public decimal? LatitudeOrigem { get; set; }

        [JsonProperty("longorigem")]
        public decimal? LongitudeOrigem { get; set; }

        [JsonProperty("descorigem")]
        public string NomeOrigem { get; set; }

        [JsonProperty("municipio")]
        public string MunicipioOrigem { get; set; }

        [JsonProperty("prevorigem")]
        public string DataInicialOrigem { get; set; }

        [JsonProperty("chegadaorigem")]
        public string PrevisaoChegadaOrigem { get; set; }

        [JsonProperty("seq")]
        public int OrdemOrigem { get; set; }

    }
}
