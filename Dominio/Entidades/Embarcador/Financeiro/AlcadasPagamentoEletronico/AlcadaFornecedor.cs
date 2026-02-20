namespace Dominio.Entidades.Embarcador.Financeiro.AlcadasPagamentoEletronico
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_PAGAMENTO_ELETRONICO_FORNECEDOR", EntityName = "AlcadasPagamentoEletronico.AlcadaFornecedor", Name = "Dominio.Entidades.Embarcador.Financeiro.AlcadasPagamentoEletronico.AlcadaFornecedor", NameType = typeof(AlcadaFornecedor))]
    public class AlcadaFornecedor : RegraAutorizacao.Alcada<RegraAutorizacaoPagamentoEletronico, Cliente>
    {
        #region Propriedades Sobrescritas

        public override string Descricao
        {
            get { return PropriedadeAlcada.Descricao; }
        }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Cliente PropriedadeAlcada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoPagamentoEletronico", Column = "RAT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoPagamentoEletronico RegrasAutorizacao { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override object ObterValorPropriedadeAlcada()
        {
            return PropriedadeAlcada.Codigo;
        }

        #endregion
    }
}
