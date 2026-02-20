namespace Dominio.ObjetosDeValor.Embarcador.Frota
{
    public class TabelaPosto
    {
        public int Codigo { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.NotaFiscal.Produto Produto { get; set; }
        public string UnidadeMedida { get; set; }
        public string CodigoIntegracao { get; set; }
        public decimal ValorDe { get; set; }
        public decimal ValorAte { get; set; }
        public decimal PercentualDesconto { get; set; }
        public string DataInicial { get; set; }
        public string DataFinal { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Posto { get; set; }
    }
}
