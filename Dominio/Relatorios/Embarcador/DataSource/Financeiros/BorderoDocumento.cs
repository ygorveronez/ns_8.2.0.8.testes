namespace Dominio.Relatorios.Embarcador.DataSource.Financeiros
{
    public class BorderoDocumento
    {
        public string Numero { get; set; }
        public decimal ValorACobrar { get; set; }
        public decimal ValorTotalACobrar { get; set; }
        public decimal ValorTotalAcrescimo { get; set; }
        public decimal ValorTotalDesconto { get; set; }
    }
}
