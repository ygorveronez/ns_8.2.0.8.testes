namespace Dominio.Entidades.Embarcador.NFS
{

    /// <summary>
    /// Classe que faz o armazenamento das notas que estavam em uma nfs gerada manualmente (apenas para informação/histórico).
    /// </summary>
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_NFE_PARA_EMISSAO_NFS_MANUAL_CANCELADA", EntityName = "CargaDocumentoParaEmissaoNFSManualCancelada", Name = "Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManualCancelada", NameType = typeof(CargaDocumentoParaEmissaoNFSManualCancelada))]
    public class CargaDocumentoParaEmissaoNFSManualCancelada : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "NEC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LancamentoNFSManual", Column = "LNM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual LancamentoNFSManual { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaDocumentoParaEmissaoNFSManual", Column = "NEM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual CargaDocumentoParaEmissaoNFSManual { get; set; }

    }
}
