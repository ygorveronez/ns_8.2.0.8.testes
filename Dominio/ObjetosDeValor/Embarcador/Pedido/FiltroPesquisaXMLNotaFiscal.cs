namespace Dominio.ObjetosDeValor.Embarcador.Pedido
{
    public class FiltroPesquisaXMLNotaFiscal
    {
        public int NumeroNotaFiscal { get; set; }
        public string Serie { get; set; }
        public System.DateTime DataEmissao { get; set; }
        public double CodigoEmitente { get; set; }
        public string Chave { get; set; }
        public int CodigoCarga { get; set; }
        public int CodigoCargaEntrega { get; set; }
        public double CodigoCliente { get; set; }
        public int CodigoPedido { get; set; }
    }
}
