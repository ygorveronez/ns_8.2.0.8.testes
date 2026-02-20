using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.KMM
{
    public class TituloEleitor
    {
        [JsonProperty(PropertyName = "numero", Required = Required.Default)]
        public string Numero { get; set; }

        [JsonProperty(PropertyName = "dv", Required = Required.Default)]
        public string DV { get; set; }

        [JsonProperty(PropertyName = "zona", Required = Required.Default)]
        public string Zona { get; set; }

        [JsonProperty(PropertyName = "secao", Required = Required.Default)]
        public string Secao { get; set; }
    }
}
