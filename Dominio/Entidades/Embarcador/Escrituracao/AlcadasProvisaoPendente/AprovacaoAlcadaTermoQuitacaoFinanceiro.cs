using Dominio.Entidades.Embarcador.Financeiro;

namespace Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_APROVACAO_ALCADA_TERMO_QUITACAO_FINANCEIRO", EntityName = "AprovacaoAlcadaTermoQuitacaoFinanceiro", Name = "Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.AprovacaoAlcadaTermoQuitacaoFinanceiro", NameType = typeof(AprovacaoAlcadaTermoQuitacaoFinanceiro))]
    public class AprovacaoAlcadaTermoQuitacaoFinanceiro : RegraAutorizacao.AprovacaoAlcada<TermoQuitacaoFinanceiro, RegraAutorizacaoProvisaoPendente>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TermoQuitacaoFinanceiro", Column = "TQU_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override TermoQuitacaoFinanceiro OrigemAprovacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoProvisaoPendente", Column = "RAT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoProvisaoPendente RegraAutorizacao { get; set; }

        #endregion
    }
}
