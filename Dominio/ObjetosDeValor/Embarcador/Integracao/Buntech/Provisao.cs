using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Buntech
{
    public class Provisao
    {
        [JsonProperty("cxFilial")]
        public string CodigoIntegracaoFilial { get; set; }

        [JsonProperty("cChvNF")]
        public string ChaveNotaFiscal { get; set; }

        [JsonProperty("nPrtPed")]
        public int NumeroPedido { get; set; }

        [JsonProperty("cTpOper")]
        public string CodigoIntegracaoTipoOperacao { get; set; }

        [JsonProperty("nValor")]
        public decimal ValorFreteNotaFiscal { get; set; }
    }
}
