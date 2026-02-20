using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Hub
{
    public class Cidade
    {
        [JsonProperty("name")]
        public string NomeCidade { get; set; }

        [JsonProperty("code")]
        public string CodigoIBGE { get; set; }

        [JsonProperty("provinceState")]
        public Estado Estado { get; set; }
    }
}
