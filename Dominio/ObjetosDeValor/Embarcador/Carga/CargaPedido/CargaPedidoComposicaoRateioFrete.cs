using System;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public sealed class CargaPedidoComposicaoRateioFrete
    {
        #region Propriedades

        public bool DisponibilizarComposicaoRateioCarga { get; set; }

        public string NumeroPedido { get; set; }

        public int CodigoPedido { get; set; }

        public string DescricaoRateio { get; set; }

        public decimal Peso { get; set; }

        public decimal PesoPedido { get; set; }

        public decimal DistanciaPedido { get; set; }

        public decimal FatorPonderacao { get; set; }

        public decimal TaxaElemento { get; set; }

        public decimal ValorPedido { get; set; }

        public decimal ValorCalculado { get; set; }

        public string CodigoTabela { get; set; }

        public string Origem { get; set; }

        public string Destino { get; set; }

        #endregion Propriedades
    }
}