using AdminMultisoftware.Dominio.Enumeradores;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.OrdemCompra
{
    public sealed class AdicionarOrdemCompra
    {
        public string CnpjCpfFornecedor { get; set; }
		public string CnpjCpfTransportador { get; set; }
        public string CpfMotorista { get; set; }
        public string PlacaVeiculo { get; set; }
        public string DataEmissao { get; set; }
        public string DataPrevisaoRetorno { get; set; }
        public string CodigoIntegracaoMotivoCompra { get; set; }
        public string CodigoIntegracaoOperador { get; set; }
        public string Observacao { get; set; }
		public string CondicaoPagamento { get; set; }
        public List<AdicionarOrdemCompraMercadoria> Produtos { get; set; }
    }

    public sealed class AdicionarOrdemCompraMercadoria
    {
        public string CodigoProduto { get; set; }
        public string PlacaVeiculoMercadoria { get; set; }
        public decimal Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }
    }
}
