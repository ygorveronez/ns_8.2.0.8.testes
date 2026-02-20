namespace Dominio.Entidades.Embarcador.Logistica.AlcadasTermoQuitacao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_AUTORIZACAO_ALCADA_TERMO_QUITACAO", EntityName = "AprovacaoAlcadaTermoQuitacao", Name = "Dominio.Entidades.Embarcador.Logistica.AlcadasTermoQuitacao.AprovacaoAlcadaTermoQuitacao", NameType = typeof(AprovacaoAlcadaTermoQuitacao))]
    public class AprovacaoAlcadaTermoQuitacao : RegraAutorizacao.AprovacaoAlcada<TermoQuitacao.TermoQuitacao, RegraAutorizacaoTermoQuitacao>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TermoQuitacao", Column = "TEQ_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override TermoQuitacao.TermoQuitacao OrigemAprovacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoTermoQuitacao", Column = "RAT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoTermoQuitacao RegraAutorizacao { get; set; }

        #endregion
    }
}
