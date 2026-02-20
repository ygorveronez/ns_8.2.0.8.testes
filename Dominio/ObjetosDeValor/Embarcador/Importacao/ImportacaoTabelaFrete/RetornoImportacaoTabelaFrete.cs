namespace Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete
{
    public sealed class RetornoImportacaoTabelaFrete
    {
        public int CodigoErro { get; set; }
        public string LinhaErro { get; set;}
        public string DescricaoErro { get; set;}
    }
}
