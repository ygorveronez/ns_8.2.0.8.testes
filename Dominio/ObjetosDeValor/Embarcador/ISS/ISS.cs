namespace Dominio.ObjetosDeValor.Embarcador.ISS
{
    public class ISS
    {
        public decimal Aliquota { get; set; }
        public decimal PercentualRetencao { get; set; }
        public decimal ValorBaseCalculoISS { get; set; }
        public decimal ValorISS { get; set; }
        public decimal ValorRetencaoISS { get; set; }
        public bool IncluirISSBaseCalculo { get; set; }
    }
}
