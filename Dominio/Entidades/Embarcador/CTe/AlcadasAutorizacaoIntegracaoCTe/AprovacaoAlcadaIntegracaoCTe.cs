namespace Dominio.Entidades.Embarcador.CTe.AlcadasAutorizacaoIntegracaoCTe
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_AUTORIZACAO_ALCADA_INTEGRACAO_CTE", EntityName = "AprovacaoAlcadaIntegracaoCTe", Name = "Dominio.Entidades.Embarcador.CTe.AlcadasAutorizacaoIntegracaoCTe.AprovacaoAlcadaIntegracaoCTe", NameType = typeof(AprovacaoAlcadaIntegracaoCTe))]
    public class AprovacaoAlcadaIntegracaoCTe : RegraAutorizacao.AprovacaoAlcada<Cargas.Carga, RegraAutorizacaoIntegracaoCTe>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Cargas.Carga OrigemAprovacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoIntegracaoCTe", Column = "RAT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoIntegracaoCTe RegraAutorizacao { get; set; }

        #endregion
    }
}
