namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public class DocumentoEntradaCentroResultadoTipoDespesa
    {
        public CentroResultado CentroResultado { get; set; }
        public TipoDespesaFinanceira TipoDespesaFinanceira { get; set; }
        public decimal Percentual { get; set; }
    }
}
