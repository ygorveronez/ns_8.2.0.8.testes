using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.DigitalCom
{
    public class Posicao
    {
        [JsonProperty("country")]
        public string Pais { get; set; }

        [JsonProperty("state")]
        public string Estado { get; set; }

        [JsonProperty("city")]
        public string Cidade { get; set; }

        [JsonProperty("cep")]
        public string CEP { get; set; }

        [JsonProperty("District")]
        public string Bairro { get; set; }

        [JsonProperty("Street")]
        public string Rua { get; set; }

        [JsonProperty("Number")]
        public int? Numero { get; set; }

        [JsonProperty("codIbge")]
        public int CodigoIBGE { get; set; }

        [JsonProperty("eixoSuspenso")]
        public bool EixoSuspenso { get; set; }

    }
}
