namespace Dominio.Entidades.Embarcador.Frota.AlcadasOrdemServico
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_ORDEM_SERVICO_FORNECEDOR", EntityName = "AlcadasOrdemServico.AlcadaFornecedor", Name = "Dominio.Entidades.Embarcador.Frota.AlcadasOrdemServico.AlcadaFornecedor", NameType = typeof(AlcadaFornecedor))]
    public class AlcadaFornecedor : RegraAutorizacao.Alcada<RegraAutorizacaoOrdemServico, Cliente>
    {
        #region Propriedades Sobrescritas

        public override string Descricao
        {
            get { return PropriedadeAlcada.Descricao; }
        }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Cliente PropriedadeAlcada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoOrdemServico", Column = "RAT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoOrdemServico RegrasAutorizacao { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override object ObterValorPropriedadeAlcada()
        {
            return PropriedadeAlcada.Codigo;
        }

        #endregion
    }
}
