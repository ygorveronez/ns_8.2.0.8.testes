namespace Dominio.ObjetosDeValor.Embarcador.NotaFiscal
{
    public sealed class NaoConformidade
    {
        public int Codigo { get; set; }

        public int CodigoCFOP{ get; set; }

        public int CodigoFilial { get; set; }

        public int CodigoItemNaoConformidade { get; set; }

        public int CodigoTipoOperacao { get; set; }

        public int NumeroNotaFiscal { get; set; }

        public Enumeradores.SituacaoNaoConformidade Situacao { get; set; }

        public Enumeradores.GrupoNC Grupo { get; set; }

        public Enumeradores.SubGrupoNC SubGrupo { get; set; }

        public Enumeradores.AreaNC Area { get; set; }
    }
}
