namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public class BaixaTituloReceberAcrescimoDesconto
    {
        public Dominio.Entidades.Embarcador.Fatura.Justificativa Justificativa { get; set; }
        public string Observacao { get; set; }
        public decimal Valor { get; set; }
        public decimal ValorTotalRateado { get; set; }
    }
}
