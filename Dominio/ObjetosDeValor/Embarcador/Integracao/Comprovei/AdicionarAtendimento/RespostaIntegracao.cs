using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Comprovei.AdicionarAtendimento
{
    public class RetornoIntegracao
    {
        [JsonProperty("message")]
        public string Mensagem { get; set; }
    }
}
