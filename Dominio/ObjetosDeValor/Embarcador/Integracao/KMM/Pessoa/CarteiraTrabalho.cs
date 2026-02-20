using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.KMM
{
    public class CarteiraTrabalho
    {
        [JsonProperty(PropertyName = "numero", Required = Required.Default)]
        public string Numero { get; set; }

        [JsonProperty(PropertyName = "serie", Required = Required.Default)]
        public string Serie { get; set; }

        [JsonProperty(PropertyName = "uf", Required = Required.Default)]
        public string UF { get; set; }
    }
}
