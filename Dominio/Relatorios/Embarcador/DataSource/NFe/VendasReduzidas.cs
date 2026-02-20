namespace Dominio.Relatorios.Embarcador.DataSource.NFe
{
    public class VendasReduzidas
    {
        public int CodigoProduto { get; set; }
        public string Produto { get; set; }
        public string Mes { get; set; }
        public int Ano { get; set; }
        public decimal Quantidade { get; set; }
        public decimal MediaValorUnitario { get; set; }
        public decimal Valor { get; set; }
    }
}
