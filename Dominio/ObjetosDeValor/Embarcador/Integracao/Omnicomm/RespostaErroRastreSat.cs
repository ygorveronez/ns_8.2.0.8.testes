using System.Text.Json.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Omnicomm
{
    public class RespostaErroRastreSat
    {
        [JsonPropertyName("error")]
        public string Mensagem { get; set; }

        public override string ToString()
        {
            return Mensagem;
        }
    }
}
