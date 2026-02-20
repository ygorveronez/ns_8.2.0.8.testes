namespace Dominio.ObjetosDeValor.Embarcador.Frete
{
    public class ExportacaoTabelaFrete
    {
        public string CodigoIntegracao { get; set; }
        public string DescricaoParametroBase { get; set; }
        public decimal Valor { get; set; }
        public string DataInicial { get; set; }
        public string DataFinal { get; set; }
    }
}
