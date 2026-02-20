using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.NovoApp.ColetaEntrega
{
    public partial class ResponseCriarOcorrencia
    {
        [JsonProperty("Codigo")]
        public int? Codigo { get; set; }
        public int? Numero { get; set; }
    }
}
