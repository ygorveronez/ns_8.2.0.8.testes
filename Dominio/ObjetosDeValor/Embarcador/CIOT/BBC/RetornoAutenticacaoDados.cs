using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.BBC
{
    public class RetornoAutenticacaoDados
    {
        [JsonProperty(PropertyName = "token", Required = Required.Default)]
        public string Token { get; set; }
    }
}
