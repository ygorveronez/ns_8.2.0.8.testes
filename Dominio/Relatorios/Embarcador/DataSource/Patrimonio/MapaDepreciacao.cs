namespace Dominio.Relatorios.Embarcador.DataSource.Patrimonio
{
    public class MapaDepreciacao
    {
        public int Codigo { get; set; }
        public string Descricao { get; set; }
        public string NumeroSerie { get; set; }
        public string GrupoProduto { get; set; }
        public string CentroResultado { get; set; }
        public string Almoxarifado { get; set; }
        public decimal ValorBem { get; set; }
        public decimal PercentualDepreciacao { get; set; }
        public string MesAno { get; set; }
        public decimal ValorDepreciacao { get; set; }
        int Ano { get; set; }
        int Mes { get; set; }
    }
}
