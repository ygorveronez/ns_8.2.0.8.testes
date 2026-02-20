using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.NovoApp.Comum
{
    public partial class ResponseBool
    {
        [JsonProperty("Sucesso")]
        public bool Sucesso { get; set; }

    }
}
