using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.DigitalCom
{
    public class DadosRetornoCancelamentoViagem
    {
        [JsonProperty("nroTransporte")]
        public string NumeroTransporte { get; set; }

        [JsonProperty("erro")]
        public string Erro { get; set; }

        [JsonProperty("status")]
        public Status Status { get; set; }
    }
}
