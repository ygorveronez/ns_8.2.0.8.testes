using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.RoutEasy
{
    public class CondicoesVeiculo
    {
        [JsonProperty(PropertyName = "field_key")]
        public string ChaveCampo { get; set; }

        [JsonProperty(PropertyName = "field_type")]
        public string TipoCampo { get; set; }

        [JsonProperty(PropertyName = "operator")]
        public string Operador { get; set; }

        [JsonProperty(PropertyName = "value")]
        public string DescricaoModeloVeicular { get; set; }
    }
}
