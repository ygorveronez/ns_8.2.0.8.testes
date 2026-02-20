namespace Dominio.Relatorios.Embarcador.DataSource.Fretes
{
    public class DocumentoHavan
    {
        public int Codigo { get; set; }
        public string CodigoIntegracao { get; set; }
        public string DataInicial { get; set; }
        public string DataFinal { get; set; }
        public string DescricaoParametroBase { get; set; }
        public decimal ValorTipoCarga { get; set; }
        public decimal ValorComponente1 { get; set; }
    }
}
