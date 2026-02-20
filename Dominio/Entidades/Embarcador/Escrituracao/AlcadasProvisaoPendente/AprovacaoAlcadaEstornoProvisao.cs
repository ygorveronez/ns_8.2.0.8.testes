using Dominio.Entidades.Embarcador.Financeiro;

namespace Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_APROVACAO_ALCADA_ESTORNO_PROVISAO", EntityName = "AprovacaoAlcadaEstornoProvisao", Name = "Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.AprovacaoAlcadaEstornoProvisao", NameType = typeof(AprovacaoAlcadaEstornoProvisao))]
    public class AprovacaoAlcadaEstornoProvisao : RegraAutorizacao.AprovacaoAlcada<CancelamentoProvisao, RegraAutorizacaoProvisaoPendente>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CancelamentoProvisao", Column = "EPS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override CancelamentoProvisao OrigemAprovacao { get; set; }


        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TermoQuitacaoFinanceiro", Column = "TQU_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TermoQuitacaoFinanceiro TermoQuitacaoFinanceiro { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoProvisaoPendente", Column = "RAT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoProvisaoPendente RegraAutorizacao { get; set; }

        #endregion
    }
}
