namespace Dominio.Entidades.Embarcador.NFS
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ANEXO_LANCAMENTO_NFS_MANUAL", EntityName = "AnexoLancamentoNFSManual", Name = "Dominio.Entidades.Embarcador.NFS.AnexoLancamentoNFSManual", NameType = typeof(AnexoLancamentoNFSManual))]
    public class AnexoLancamentoNFSManual : Anexo.Anexo<LancamentoNFSManual>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LancamentoNFSManual", Column = "LNM_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override LancamentoNFSManual EntidadeAnexo { get; set; }

        #endregion
    }
}
