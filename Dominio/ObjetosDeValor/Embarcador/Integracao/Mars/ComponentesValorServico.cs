using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Mars
{
    public class ComponentesValorServico
    {
        [JsonProperty("freightValue")]
        public string ValorFrete { get; set; }

        [JsonProperty("taxes")]
        public string Impostos { get; set; }

        [JsonProperty("toll")]
        public string Pedagio { get; set; }

        [JsonProperty("unloading")]
        public string Descarga { get; set; }

        [JsonProperty("totalCTe")]
        public string TotalCTe { get; set; }
    }
}
