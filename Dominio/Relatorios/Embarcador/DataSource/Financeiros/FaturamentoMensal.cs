namespace Dominio.Relatorios.Embarcador.DataSource.Financeiros
{
    public class FaturamentoMensal
    {
        public string Mes { get; set; }
        public int Ano { get; set; }
        public decimal ValorReceber { get; set; }
        public decimal ValorPagar { get; set; }
        public decimal Total { get; set; }
    }
}
