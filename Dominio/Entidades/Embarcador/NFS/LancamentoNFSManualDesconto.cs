namespace Dominio.Entidades.Embarcador.NFS
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_LANCAMENTO_NFS_MANUAL_DESCONTO", EntityName = "LancamentoNFSManualDesconto", Name = "Dominio.Entidades.Embarcador.NFS.LancamentoNFSManualDesconto", NameType = typeof(LancamentoNFSManualDesconto))]
    public class LancamentoNFSManualDesconto : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "LND_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LancamentoNFSManual", Column = "LNM_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual LancamentoNFSManual LancamentoNFSManual { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "LND_CANCELADO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Cancelado { get; set; }
    }
}
