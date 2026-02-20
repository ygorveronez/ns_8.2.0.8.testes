namespace Dominio.Entidades.Embarcador.Documentos.Alcadas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_AUTORIZACAO_ALCADA_CONTROLE_DOCUMENTO", EntityName = "AprovacaoAlcadaControleDocumento", Name = "Dominio.Entidades.Embarcador.Documentos.Alcadas.AprovacaoAlcadaControleDocumento", NameType = typeof(AprovacaoAlcadaControleDocumento))]
    public class AprovacaoAlcadaControleDocumento : RegraAutorizacao.AprovacaoAlcada<ControleDocumento, RegraAutorizacaoDocumento>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ControleDocumento", Column = "COD_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override ControleDocumento OrigemAprovacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoDocumento", Column = "RAD_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoDocumento RegraAutorizacao { get; set; }

        #endregion
    }
}
