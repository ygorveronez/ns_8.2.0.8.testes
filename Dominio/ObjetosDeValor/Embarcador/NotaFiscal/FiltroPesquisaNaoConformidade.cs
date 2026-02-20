namespace Dominio.ObjetosDeValor.Embarcador.NotaFiscal
{
    public sealed class FiltroPesquisaNaoConformidade
    {
        public int CodigoCargaPedido { get; set; }

        public int CodigoItemNaoConformidade { get; set; }
        
        public int CodigoXMLNotaFiscal { get; set; }
        
        public Enumeradores.TipoParticipante? TipoParticipante { get; set; }
    }
}

