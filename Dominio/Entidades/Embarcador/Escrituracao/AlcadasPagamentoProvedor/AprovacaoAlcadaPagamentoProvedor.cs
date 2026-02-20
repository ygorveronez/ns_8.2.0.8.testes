using Dominio.Entidades.Embarcador.Financeiro;

namespace Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_AUTORIZACAO_ALCADA_PAGAMENTO_PROVEDOR", EntityName = "AprovacaoAlcadaPagamentoProvedor", Name = "Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.AprovacaoAlcadaPagamentoProvedor", NameType = typeof(AprovacaoAlcadaPagamentoProvedor))]
    public class AprovacaoAlcadaPagamentoProvedor : RegraAutorizacao.AprovacaoAlcada<PagamentoProvedor, RegraPagamentoProvedor>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PagamentoProvedor", Column = "PRO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override PagamentoProvedor OrigemAprovacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraPagamentoProvedor", Column = "RAT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraPagamentoProvedor RegraAutorizacao { get; set; }

        #endregion
    }
}
