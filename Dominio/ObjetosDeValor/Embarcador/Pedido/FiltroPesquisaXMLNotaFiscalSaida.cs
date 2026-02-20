namespace Dominio.ObjetosDeValor.Embarcador.Pedido
{
    public class FiltroPesquisaXMLNotaFiscalSaida
    {
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal TipoOperacaoNotaFiscal { get; set; }
        public int NumeroNotaFiscal { get; set; }
        public System.DateTime DataEmissaoInicial { get; set; }
        public System.DateTime DataEmissaoFinal { get; set; }
        public string Chave { get; set; }
        public string Serie { get; set; }
        public double CodigoDestinatario { get; set; }
        public double CodigoRemetente { get; set; }
        public int Empresa { get; set; }
    }
}
