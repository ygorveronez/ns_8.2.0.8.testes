using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.NovoApp.Comum
{
    public partial class ResponseImagem
    {
        [JsonProperty("Imagem")]
        public string Imagem { get; set; }

    }
}
