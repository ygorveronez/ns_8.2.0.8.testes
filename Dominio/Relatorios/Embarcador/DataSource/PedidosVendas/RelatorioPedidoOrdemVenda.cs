using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.PedidosVendas
{
    public class RelatorioPedidoOrdemVenda
    {
        #region Propriedades

        public int Codigo { get; set; }
        public int Numero { get; set; }
        public DateTime DataEmissao { get;  set; }
        public DateTime DataEntrega { get; set; }
        public string Pessoa { get; set; }
        public string Vendedor { get; set; }
        public TipoPedidoVenda Tipo { get; set; }
        public StatusPedidoVenda Status { get; set; }
        public string Veiculo { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal ValorProdutos { get; set; }
        public decimal ValorServicos { get; set; }
        public string CodigoItem { get; set; }
        public string DescricaoItem { get; set; }
        public decimal QuantidadeItem { get; set; }
        public decimal ValorUnitarioItem { get; set; }
        public decimal ValorTotalItem { get; set; }
        public string FornecedorServico { get; set; }
        public string FuncionarioServico { get; set; }
        public string NotasFiscais { get; set; }
        public string Referencia { get; set; }
        public string Observacao { get; set; }
        public int NumeroInterno { get; set; }

        public int KMTotal { get; set; }
        public string HotaTotal { get; set; }

        #endregion

        #region Propriedades com Regras

        public string DataEmissaoFormatada
        {
            get { return DataEmissao != DateTime.MinValue ? DataEmissao.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataEntregaFormatada
        {
            get { return DataEntrega != DateTime.MinValue ? DataEntrega.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DescricaoTipo
        {
            get { return Tipo.ObterDescricao(); }
        }

        public string DescricaoStatus
        {
            get { return Status.ObterDescricao(); }
        }

        #endregion
    }
}
