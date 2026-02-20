namespace Dominio.Entidades.Embarcador.Frota.AlcadasInfracao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_AUTORIZACAO_ALCADA_INFRACAO", EntityName = "AprovacaoAlcadaInfracao", Name = "Dominio.Entidades.Embarcador.Frota.AlcadasInfracao.AprovacaoAlcadaInfracao", NameType = typeof(AprovacaoAlcadaInfracao))]
    public class AprovacaoAlcadaInfracao : RegraAutorizacao.AprovacaoAlcada<Infracao, RegraAutorizacaoInfracao>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Infracao", Column = "INF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Infracao OrigemAprovacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoInfracao", Column = "RAT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoInfracao RegraAutorizacao { get; set; }

        #endregion
    }
}
