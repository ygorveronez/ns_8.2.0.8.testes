using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Buntech
{
    public class ResultadoIntegracaoProvisao
    {
        [JsonProperty("status")]
        public bool StatusIntegracao { get; set; }

        [JsonProperty("cChvNF")]
        public string ChaveNotaFiscal { get; set; }

        [JsonProperty("msg")]
        public string Mensagem { get; set; }
    }

}
