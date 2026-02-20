using System.Text.Json.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.ABFSat
{
    public class RespostaErroABFSat
    {
        [JsonPropertyName("Message")]
        public string Message { get; set; }
    }
}
