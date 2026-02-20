using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Eship
{
    public class RetornoMontarCargaErrosErro
    {
        [JsonProperty(PropertyName = "mensagem", Required = Required.Default)]
        public string Mensagem { get; set; }

        [JsonProperty(PropertyName = "codigo", Required = Required.Default)]
        public string Codigo { get; set; }
    }
}
