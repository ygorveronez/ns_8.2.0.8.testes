namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public class ConciliacaoBancaria
    {
        public int Codigo { get; set; }
        public string DataInicial { get; set; }
        public string DataFinal { get; set; }
        public string DataGeracaoMovimento { get; set; }
        public string Colaborador { get; set; }
        public string RealizarConciliacaoAutomatica { get; set; }
        public string SituacaoConciliacaoBancaria { get; set; }
        public decimal ValorTotalDebitoExtrato { get; set; }
        public decimal ValorTotalCreditoExtrato { get; set; }
        public decimal ValorTotalExtrato { get; set; }
        public decimal ValorTotalDebitoMovimento { get; set; }
        public decimal ValorTotalCreditoMovimento { get; set; }
        public decimal ValorTotalMovimento { get; set; }
        public string PlanoConta { get; set; }
    }

}
