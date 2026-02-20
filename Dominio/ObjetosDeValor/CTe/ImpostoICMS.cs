namespace Dominio.ObjetosDeValor.CTe
{
    public class ImpostoICMS
    {
        public string CST { get; set; }

        public decimal BaseCalculo { get; set; }

        public decimal Aliquota { get; set; }

        public decimal Valor { get; set; }

        public decimal ValorIncluso { get; set; }

        public decimal PercentualReducaoBaseCalculo { get; set; }

        public decimal ValorCreditoPresumido { get; set; }

        public decimal ValorDevido { get; set; }

        public decimal ValorDesoneracao { get; set; }
        public string CodigoBeneficio { get; set; }
    }
}
