using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta
{
    public class RetornoIntegrarViagem
    {
        [JsonProperty(PropertyName = "Sucesso", Required = Required.Default)]
        public bool Sucesso { get; set; }

        [JsonProperty(PropertyName = "Mensagem", Required = Required.Default)]
        public string Mensagem { get; set; }

        [JsonProperty(PropertyName = "Objeto", Required = Required.Default)]
        public RetornoIntegrarViagemObjeto Objeto { get; set; }
    }
}
