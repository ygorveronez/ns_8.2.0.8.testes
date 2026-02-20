namespace Dominio.Entidades.Embarcador.ICMS.AlcadasAlteracaoRegraICMS
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_AUTORIZACAO_ALCADA_ALTERACAO_REGRA_ICMS", EntityName = "AprovacaoAlcadaAlteracaoRegraICMS", Name = "Dominio.Entidades.Embarcador.ICMS.AlcadasAlteracaoRegraICMS.AprovacaoAlcadaAlteracaoRegraICMS", NameType = typeof(AprovacaoAlcadaAlteracaoRegraICMS))]
    public class AprovacaoAlcadaAlteracaoRegraICMS : RegraAutorizacao.AprovacaoAlcada<RegraICMS, RegraAutorizacaoAlteracaoRegraICMS>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraICMS", Column = "RIC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraICMS OrigemAprovacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoAlteracaoRegraICMS", Column = "RAT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoAlteracaoRegraICMS RegraAutorizacao { get; set; }

        #endregion
    }
}
