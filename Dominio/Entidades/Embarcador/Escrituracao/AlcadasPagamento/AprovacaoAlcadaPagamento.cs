namespace Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamento
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_AUTORIZACAO_ALCADA_PAGAMENTO", EntityName = "AprovacaoAlcadaPagamento", Name = "Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamento.AprovacaoAlcadaPagamento", NameType = typeof(AprovacaoAlcadaPagamento))]
    public class AprovacaoAlcadaPagamento : RegraAutorizacao.AprovacaoAlcada<Pagamento, RegraAutorizacaoPagamento>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pagamento", Column = "PAG_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Pagamento OrigemAprovacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoPagamento", Column = "RAT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoPagamento RegraAutorizacao { get; set; }

        #endregion
    }
}
