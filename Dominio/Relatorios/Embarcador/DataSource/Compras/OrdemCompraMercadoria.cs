namespace Dominio.Relatorios.Embarcador.DataSource.Compras
{
    public class OrdemCompraMercadoria
    {
        public string Numero { get; set; }

        public string Produto { get; set; }

        public decimal Quantidade { get; set; }

        public decimal ValorUnitario { get; set; }

        public decimal ValorTotal { get; set; }

        public string VeiculoMercadoria { get; set; }
    }
}
