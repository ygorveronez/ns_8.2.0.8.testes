using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Italac
{
    public class DadosRespostaItalac
    {
        [JsonProperty(PropertyName = "mensagem")]
        public string Mensagem { get; set; }
    }
}
