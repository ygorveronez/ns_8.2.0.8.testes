namespace Dominio.ObjetosDeValor.Embarcador.ISS
{
    public class RegraISS
    {
        public decimal AliquotaISS { get; set; }
        public decimal PercentualRetencaoISS { get; set; }
        public decimal ValorBaseCalculoISS { get; set; }
        public decimal ValorISS { get; set; }
        public decimal ValorRetencaoISS { get; set; }
        public bool IncluirISSBaseCalculo { get; set; }
        public bool ReterIR { get; set; }
        public decimal AliquotaIR { get; set; }
        public decimal BaseCalculoIR { get; set; }
        public decimal ValorIR { get; set; }

        public string NBS { get; set; }
    }
}
