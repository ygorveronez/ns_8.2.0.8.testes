namespace Dominio.ObjetosDeValor.WebServiceCarrefour.ICMS
{
    public sealed class ICMS
    {
        public ICMS()
        {
            this.ObservacaoCTe = "";
            this.SimplesNacional = false;
            this.Aliquota = 0;
            this.ValorICMS = 0;
            this.ValorBaseCalculoICMS = 0;
            this.PercentualReducaoBC = 0;
            this.PercentualInclusaoBC = 0;
            this.IncluirICMSBC = false;
            this.CST = "";
            this.ValorCreditoPresumido = 0;
            this.ExibirNaDacte = true;
            this.ValorTotalTributos = 0;
        }

        public decimal Aliquota { get; set; }

        public decimal ValorICMS { get; set; }

        public decimal ValorCreditoPresumido { get; set; }

        public decimal ValorBaseCalculoICMS { get; set; }

        public decimal PercentualReducaoBC { get; set; }

        public bool IncluirICMSBC { get; set; }

        public decimal PercentualInclusaoBC { get; set; }

        public string CST { get; set; }

        public string ObservacaoCTe { get; set; }

        public bool SimplesNacional { get; set; }

        public bool ExibirNaDacte { get; set; }

        public decimal ValorTotalTributos { get; set; }
    }
}
