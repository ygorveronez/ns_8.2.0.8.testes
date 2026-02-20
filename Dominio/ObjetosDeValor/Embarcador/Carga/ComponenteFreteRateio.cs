namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class ComponenteFreteRateio
    {
        public double CPFCNPJRemetente { get; set; }
        public double CPFCNPJDestinatario { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal TipoOperacaoNotaFiscal { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete TipoComponente { get; set; }
        public int CodigoCargaPedido { get; set; }
        public int CodigoComponenteFrete { get; set; }
        public decimal Valor { get; set; }
        public decimal ValorMoeda { get; set; }
    }
}
