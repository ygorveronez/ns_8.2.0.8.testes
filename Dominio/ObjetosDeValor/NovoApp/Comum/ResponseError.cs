using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.NovoApp.Comum
{
    public partial class ResponseError
    {
        [JsonProperty("Mensagem")]
        public string Mensagem { get; set; }
    }

}
