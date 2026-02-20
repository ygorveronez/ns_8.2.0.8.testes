using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.CTASmart
{
    public class InformaSincronismo
    {
        [JsonProperty(PropertyName = "abastecimentos", Required = Required.Default)]
        public InformaSincronismoAbastecimento[] Abastecimentos { get; set; }
    }
}
