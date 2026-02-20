namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class CargaPedidoXMLNotaFiscalConsulta
    {
        public bool Ativa { get; set; }

        public int Codigo { get; set; }

        public int CodigoCargaPedido { get; set; }

        public decimal PesoCubado { get; set; }

        public decimal Peso { get; set; }

        public Enumeradores.TipoNotaFiscalIntegrada? TipoNotaFiscalIntegrada { get; set; }

        public decimal Valor { get; set; }

        public Enumeradores.ClassificacaoNFe? ClassificacaoNFe { get; set; }

        public bool TipoFatura { get; set; }
    }
}
