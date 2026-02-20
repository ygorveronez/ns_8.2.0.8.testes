namespace Dominio.Entidades.Embarcador.Documentos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TRACKING_DOCUMENTACAO_REGISTRO_CARGA", EntityName = "TrackingDocumentacaoRegistroCarga", Name = "Dominio.Entidades.Embarcador.Documentos.TrackingDocumentacaoRegistroCarga", NameType = typeof(TrackingDocumentacaoRegistroCarga))]

    public class TrackingDocumentacaoRegistroCarga : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "TRC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual long Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TrackingDocumentacaoRegistro", Column = "TDR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TrackingDocumentacaoRegistro TrackingDocumentacaoRegistro { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.Carga Carga { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Carga.RetornarCodigoCargaParaVisualizacao;
            }
        }
    }
}
