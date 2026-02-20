namespace Dominio.Relatorios.Embarcador.DataSource.Financeiros
{
    public class DREGerencial
    {
        public int Codigo { get; set; }
        public string Plano { get; set; }
        public string Descricao { get; set; }
        public decimal SaldoInicial { get; set; }
        public decimal SaldoAtual { get; set; }
        public int Tipo { get; set; }
        public int ReceitaDespesa { get; set; }
        public int GrupoResultado { get; set; }
        public int GrupoTotalizador { get; set; }
    }
}