using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.CTASmart
{
    public class RetornoConsulta
    {
        [JsonProperty(PropertyName = "status", Required = Required.Default)]
        public RetornoConsultaStatus Status { get; set; }

        [JsonProperty(PropertyName = "abastecimentos", Required = Required.Default)]
        public RetornoConsultaAbastecimento[] Abastecimentos { get; set; }
    }
}
