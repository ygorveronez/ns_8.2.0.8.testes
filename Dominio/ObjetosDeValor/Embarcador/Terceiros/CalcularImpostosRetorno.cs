namespace Dominio.ObjetosDeValor.Embarcador.Terceiros
{
    public class CalcularImpostosRetorno
    {
        public decimal ValorTotalDescontoAdiantamento { get; set; }

        public decimal ValorTotalAcrescimoAdiantamento { get; set; }

        public decimal ValorTotalDescontoSaldo { get; set; }

        public decimal ValorTotalAcrescimoSaldo { get; set; }

        public decimal ValorAbastecimento { get; set; }

        public decimal AliquotaINSS { get; set; }

        public decimal AliquotaIRRF { get; set; }

        public decimal AliquotaSENAT { get; set; }

        public decimal AliquotaSEST { get; set; }

        public decimal AliquotaINSSPatronal { get; set; }

        public decimal BaseCalculoINSS { get; set; }

        public decimal BaseCalculoIRRF { get; set; }

        public decimal BaseCalculoSENAT { get; set; }

        public decimal BaseCalculoSEST { get; set; }

        public decimal ValorINSS { get; set; }

        public decimal ValorINSSPatronal { get; set; }

        public decimal ValorIRRF { get; set; }

        public decimal ValorSENAT { get; set; }

        public decimal ValorSEST { get; set; }

        public decimal AliquotaCOFINS { get; set; }

        public decimal AliquotaPIS { get; set; }

        public string CodigoIntegracaoTributaria { get; set; }

        public decimal ValorIRRFPeriodo { get; set; }

        public decimal BaseCalculoIRRFSemAcumulo { get; set; }

        public decimal BaseCalculoIRRFSemDesconto { get; set; }

        public decimal ValorIRRFSemDesconto { get; set; }

        public int? QuantidadeDependentes { get; set; }

        public decimal ValorPorDependente { get; set; }

        public decimal ValorTotalDependentes { get; set; }
    }
}
