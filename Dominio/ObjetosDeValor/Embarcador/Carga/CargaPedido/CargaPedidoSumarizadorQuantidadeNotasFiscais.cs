namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class CargaPedidoSumarizadorQuantidadeNotasFiscais
    {
        public int CodigoCargaPedido { get; set; }
        public int CodigoPedidoXMLNotaFiscal { get; set; }
        public int CodigoXMLNotaFiscal { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscalIntegrada? TipoNotaFiscalIntegrada { get; set; }
        public decimal Peso { get; set; }
        public decimal PesoCubado { get; set; }
        public decimal PesoLiquido { get; set; }
        public decimal MetrosCubicos { get; set; }
        public int Volumes { get; set; }
        public bool TipoFatura { get; set; }
    }
}
