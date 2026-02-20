using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Olfar
{
    public class NotaDebito
    {
        [JsonProperty("param")]
        public string Parametro { get; set; }

        [JsonProperty("codEmp")]
        public int CodigoEmpresa { get; set; }

        [JsonProperty("codFil")]
        public string CodigoFilial { get; set; }

        [JsonProperty("codFor")]
        public string CodigoTransportador { get; set; }

        [JsonProperty("numTit")]
        public string NumeroTitulo { get; set; }

        [JsonProperty("datVct")]
        public string DataVencimento { get; set; }

        [JsonProperty("vlrND")]
        public string Valor { get; set; }

        [JsonProperty("obsND")]
        public string Observacao { get; set; }

        [JsonProperty("chvCte")]
        public string ChaveCTeReferenciado { get; set; }

        [JsonProperty("usuGer")]
        public int CodigoUsuarioGerador { get; set; }
    }
}
