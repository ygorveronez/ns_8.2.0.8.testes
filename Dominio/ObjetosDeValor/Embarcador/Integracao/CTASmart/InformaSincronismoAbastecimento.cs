using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.CTASmart
{
    public class InformaSincronismoAbastecimento
    {
        [JsonProperty(PropertyName = "id", Required = Required.Default)]
        public int Codigo { get; set; }

        [JsonProperty(PropertyName = "status", Required = Required.Default)]
        public string Status { get; set; }
    }
}
