namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public class DadosRegraEntradaDocumento
    {
        public decimal BaseCalculoPIS { get; set; }
        public decimal PercentualReducaoPIS { get; set; }
        public decimal AliquotaPIS { get; set; }
        public decimal ValorPIS { get; set; }
        public string CSTPIS { get; set; }

        public decimal BaseCalculoCOFINS { get; set; }
        public decimal PercentualReducaoCOFINS { get; set; }
        public decimal AliquotaCOFINS { get; set; }
        public decimal ValorCOFINS { get; set; }
        public string CSTCOFINS { get; set; }

        public decimal BaseCalculoIPI { get; set; }
        public decimal PercentualReducaoIPI { get; set; }
        public decimal AliquotaIPI { get; set; }
        public decimal ValorIPICFOP { get; set; }
        public string CSTIPI { get; set; }

        public string CSTICMS { get; set; }
        public decimal AliquotaICMS { get; set; }
        public decimal BaseICMS { get; set; }
        public decimal ValorICMS { get; set; }
        public decimal AliquotaCreditoPresumido { get; set; }
        public decimal BaseCalculoCreditoPresumido { get; set; }
        public decimal ValorCreditoPresumido { get; set; }
        public decimal AliquotaDiferencial { get; set; }
        public decimal BaseCalculoDiferencial { get; set; }
        public decimal ValorDiferencial { get; set; }

        public decimal ValorRetencaoPIS { get; set; }
        public decimal ValorRetencaoCOFINS { get; set; }
        public decimal ValorRetencaoINSS { get; set; }
        public decimal ValorRetencaoIPI { get; set; }
        public decimal ValorRetencaoCSLL { get; set; }
        public decimal ValorRetencaoOutras { get; set; }
        public decimal ValorRetencaoIR { get; set; }
        public decimal ValorRetencaoISS { get; set; }
    }
}
