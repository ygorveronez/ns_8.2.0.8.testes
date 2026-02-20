using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.CTASmart
{
    public class RetornoConsultaMotorista
    {
        [JsonProperty(PropertyName = "codigo", Required = Required.Default)]
        public string Codigo { get; set; }

        [JsonProperty(PropertyName = "nome", Required = Required.Default)]
        public string Nome { get; set; }

        [JsonProperty(PropertyName = "cpf", Required = Required.Default)]
        public string Cpf { get; set; }
    }
}
