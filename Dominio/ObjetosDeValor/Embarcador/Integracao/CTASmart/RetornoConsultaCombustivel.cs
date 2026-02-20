using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.CTASmart
{
    public class RetornoConsultaCombustivel
    {
        [JsonProperty(PropertyName = "codigo", Required = Required.Default)]
        public string Codigo { get; set; }

        [JsonProperty(PropertyName = "descricao", Required = Required.Default)]
        public string Descricao { get; set; }
    }
}
