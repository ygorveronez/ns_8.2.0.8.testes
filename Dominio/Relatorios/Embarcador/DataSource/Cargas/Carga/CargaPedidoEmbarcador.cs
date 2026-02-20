using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga
{
    public class CargaPedidoEmbarcador
    {
        public string FilialDescricao { get; set; }
        public string NumeroEmbarcador { get; set; }
        public DateTime DataPedido { get; set; }
        public string CodigoIntegracao { get; set; }
        public string CodigoProdutoEmbarcador { get; set; }
        public string NomeProdutoEmbarcador { get; set; }
        public int QuantidadeEmbalagem { get; set; }
        public decimal PesoUnitario { get; set; }
        public decimal Quantidade { get; set; }
        public decimal PesoTotalKG { get; set; }
        public decimal MetroCubico { get; set; }
        public decimal QuantidadePallet { get; set; }
        public int QuantidadeCaixaPorPallet { get; set; }
        public string PalletFechado { get; set; }
        public string NumeroCarga { get; set; }
        public string NotasFiscais { get; set; }
        public string SessaoRoteirizador { get; set; }
        public string GrupoPessoas { get; set; }
        public string SituacaoPedidoProduto { get; set; }
        public string TipoCargaDescricao { get; set; }
        public string CanalEntregaDescricao { get; set; }
        public string TipoOperacaoDescricao { get; set; }
        public string GrupoProdutoDescricao { get; set; }
        public string LinhaSeparacaoDescricao { get; set; }
    }
}
