namespace Dominio.Entidades.Embarcador.Financeiro.AlcadasPagamentoEletronico
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_AUTORIZACAO_ALCADA_PAGAMENTO_ELETRONICO", EntityName = "AprovacaoAlcadaPagamentoEletronico", Name = "Dominio.Entidades.Embarcador.Financeiro.AlcadasPagamentoEletronico.AprovacaoAlcadaPagamentoEletronico", NameType = typeof(AprovacaoAlcadaPagamentoEletronico))]
    public class AprovacaoAlcadaPagamentoEletronico : RegraAutorizacao.AprovacaoAlcada<PagamentoEletronico, RegraAutorizacaoPagamentoEletronico>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PagamentoEletronico", Column = "OSE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override PagamentoEletronico OrigemAprovacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoPagamentoEletronico", Column = "RAT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoPagamentoEletronico RegraAutorizacao { get; set; }

        #endregion
    }
}
