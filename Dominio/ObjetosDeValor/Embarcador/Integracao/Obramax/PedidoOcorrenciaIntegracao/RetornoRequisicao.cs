using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax
{
    public class RetornoRequisicao
    {
        [JsonProperty("offset")]
        public string Offset { get; set; }

    }
}
