namespace Dominio.Relatorios.Embarcador.DataSource.Compras
{
    public class SugestaoCompra
    {
        public string Produto { get; set; }
        public string GrupoProduto { get; set; }
        public string Empresa { get; set; }
        public decimal Estoque { get; set; }
        public decimal QtdMinima { get; set; }
        public decimal QtdMaximo { get; set; }
        public decimal QtdSugestao { get; set; }
        public decimal UltimoCusto { get; set; }
    }
}
