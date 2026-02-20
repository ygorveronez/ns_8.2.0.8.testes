using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Frete
{
    public class FreteSubContratacao
    {
        public decimal ValorFreteSubcontratacao { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido TipoTabelaFrete { get; set; }
        public decimal ValorPedagio { get; set; }
        public decimal ValorFreteSubContratacaoTabelaDeFrete { get; set; }
        public string PercentualDescontoTerceiro { get; set; }
        public decimal PercentualAdiantamento { get; set; }
        public decimal PercentualAbastecimento { get; set; }
        public decimal PercentualSaldo { get; set; }
        public decimal ValorAbastecimento { get; set; }
        public decimal ValorAdiantamento { get; set; }
        public int DiasVencimentoAdiantamento { get; set; }
        public string DataVencimentoAdiantamento { get; set; }
        public decimal ValorSaldo { get; set; }
        public decimal Desconto { get; set; }
        public int DiasVencimentoSaldo { get; set; }
        public string DataVencimentoSaldo { get; set; }
        public string TabelaFrete { get; set; }
        public string TabelaFreteCliente { get; set; }
        public List<FreteSubContratacaoValorAdicional> ValoresAdicionais { get; set; }
        public decimal ValorTotalPrestacao { get; set; }
        public decimal ValorTotalDescontos { get; set; }
        public decimal ValorTotalAcrescimos { get; set; }
        public string ObservacaoManual { get; set; }
        public bool? IntegrouValoresAcrescimoDesconto { get; set; }
        public bool? PermiteAlterarValor { get; set; }

        /// <summary>
        /// ValorFreteSubcontratacao - ValorTotalAcrescimos + ValorTotalDescontos
        /// </summary>
        public decimal ValorLiquidoFreteTerceiro
        {
            get
            {
                return ValorFreteSubcontratacao - ValorTotalAcrescimos + ValorTotalDescontos;
            }
        }
    }
}
