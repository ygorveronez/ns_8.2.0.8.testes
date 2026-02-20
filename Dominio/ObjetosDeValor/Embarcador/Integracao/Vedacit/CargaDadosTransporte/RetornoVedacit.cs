using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Vedacit.CargaDadosTransporte
{
    public class RetornoVedacit
    {
        [JsonProperty("error")]
        public ErroVedacit Erro { get; set; }
    }

    public class ErroVedacit
    {
        [JsonProperty("message")]
        public string Mensagem { get; set; }

        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("source")]
        public string Caminho { get; set; }
    }
}
