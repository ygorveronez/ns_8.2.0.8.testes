using System;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public sealed class CargaPedidoDadosRateioFrete
    {
        #region Propriedades

        public DateTime? DataEmissaoCte { get; set; }

        public DateTime? DataPedidoAdicionado { get; set; }

        public int NumeroCte { get; set; }

        public string NumeroPedido { get; set; }

        public string NumeroStage { get; set; }

        public long Ordem { get; set; }

        public Enumeradores.ParametroRateioFormula ParametroRateio { get; set; }

        public decimal Peso { get; set; }

        public decimal PesoLiquido { get; set; }

        public decimal PesoLiquidoPedido { get; set; }

        public decimal PesoPedido { get; set; }

        public decimal ValorCte { get; set; }

        public decimal ValorFrete { get; set; }

        #endregion Propriedades

        #region Propriedades com Regras

        public decimal PesoParaCalculo
        {
            get
            {
                return (ParametroRateio == Enumeradores.ParametroRateioFormula.PesoLiquido) ? PesoLiquido : Peso;
            }
        }

        public decimal PesoPedidoParaCalculo
        {
            get
            {
                return (ParametroRateio == Enumeradores.ParametroRateioFormula.PesoLiquido) ? PesoLiquidoPedido : PesoPedido;
            }
        }

        #endregion Propriedades com Regras
    }
}