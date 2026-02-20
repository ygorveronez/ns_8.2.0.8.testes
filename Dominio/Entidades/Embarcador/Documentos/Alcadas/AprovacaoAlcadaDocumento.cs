namespace Dominio.Entidades.Embarcador.Documentos.Alcadas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_AUTORIZACAO_ALCADA_DOCUMENTO", EntityName = "AprovacaoAlcadaGestaoDocumento", Name = "Dominio.Entidades.Embarcador.Documentos.Alcadas.AprovacaoAlcadaGestaoDocumento", NameType = typeof(AprovacaoAlcadaGestaoDocumento))]
    public class AprovacaoAlcadaGestaoDocumento : RegraAutorizacao.AprovacaoAlcada<GestaoDocumento, RegraAutorizacaoDocumento>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GestaoDocumento", Column = "GED_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override GestaoDocumento OrigemAprovacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoDocumento", Column = "RAD_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoDocumento RegraAutorizacao { get; set; }

        #endregion
    }
}
