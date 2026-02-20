using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Cassol
{
    public class RequestIntegracaoItensCarga
    {
        [JsonProperty("id")]
        public int NumeroCarga { get; set; }

        [JsonProperty("filial_saida")]
        public int CodigoFilial { get; set; }

        [JsonProperty("pedven_numpedven")]
        public long NumeroPedido { get; set; }

        [JsonProperty("fl_ped_coleta")]
        public string TipoPedidoColeta { get; set; }

        [JsonProperty("pedven_coditprod")]
        public int CodigoProduto { get; set; }

        [JsonProperty("endereco_codcli")]
        public int EnderecoCodigoCliente { get; set; }

        [JsonProperty("endereco_codend")]
        public int CodigoEndereco { get; set; }

        [JsonProperty("qtcomp")]
        public decimal QuantidadeItens { get; set; }
    }
}
