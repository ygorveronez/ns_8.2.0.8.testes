using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Produtos
{
    public class ProdutoEmbarcador
    {
        public int Codigo { get; set; }
        public string Filial { get; set; }
        public string FilialIntegracao { get; set; }
        public string UFOrigem { get; set; }
        public string PedidoEmbarcador { get; set; }
        public DateTime DataHoraPedido { get; set; }
        public string StatusPedido { get; set; }
        public string SessaoRoteirizador { get; set; }
        public string NotasFiscais { get; set; }
        public int QtdCaixasPorPalete { get; set; }
        public string GrupoPessoa { get; set; }
        public string NumeroCarga { get; set; }
        public string LinhaSeparacao { get; set; }
        public string RemetenteIntegracao { get; set; }
        public string DestinatarioIntegracao { get; set; }
        public string TipoCarga { get; set; }
        public string CanalEntrega { get; set; }
        public string TipoOperacao { get; set; }
        public string GrupoProduto { get; set; }
        public string ProdutoIntegracao { get; set; }
        public string Produto { get; set; }
        public decimal QuantidadeEmbalagem { get; set; }
        public decimal Quantidade { get; set; }
        public decimal QuantidadeCarregada { get; set; }
        public decimal Saldo { get { return (this.StatusPedido == "Cancelado" ? 0 : this.Quantidade - this.QuantidadeCarregada); } }
        public decimal PesoUnitario { get; set; }
        public decimal QuantidadePallet { get; set; }
        public string PalletFechado { get; set; }
        public decimal MetroCubico { get; set; }
        public decimal PesoTotalToneladas { get; set; }
        public decimal PesoTotalQuilogramas { get; set; }
        public string IDDemanda { get; set; }
        public string EnderecoProduto { get; set; }
        public bool MontagemCarregamentoPedidoProduto { get; set; }
        public string Placa { get; set; }

        public string DataHoraPedidoFormatada
        {
            get { return DataHoraPedido != DateTime.MinValue ? DataHoraPedido.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public virtual ProdutoEmbarcador Clonar()
        {
            ProdutoEmbarcador produtoEmbarcadorClonado = (ProdutoEmbarcador)this.MemberwiseClone();
            produtoEmbarcadorClonado.StatusPedido = "Em Aberto";
            produtoEmbarcadorClonado.SessaoRoteirizador = string.Empty;
            produtoEmbarcadorClonado.Quantidade = produtoEmbarcadorClonado.Saldo;
            produtoEmbarcadorClonado.QuantidadeCarregada = 0;
            produtoEmbarcadorClonado.NumeroCarga = string.Empty;
            produtoEmbarcadorClonado.NotasFiscais = string.Empty;
            return produtoEmbarcadorClonado;
        }
    }
}
