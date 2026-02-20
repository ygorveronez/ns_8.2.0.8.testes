using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.CTASmart
{
    public class RetornoConsultaVeiculo
    {
        [JsonProperty(PropertyName = "codigo", Required = Required.Default)]
        public string Codigo { get; set; }

        [JsonProperty(PropertyName = "placa", Required = Required.Default)]
        public string Placa { get; set; }

        [JsonProperty(PropertyName = "frota", Required = Required.Default)]
        public string NumeroFrota { get; set; }
    }
}
