namespace Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_TABELA_FRETE_TIPO_OPERACAO", EntityName = "AlcadasTabelaFrete.AlcadaTipoOperacao", Name = "Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.AlcadaTipoOperacao", NameType = typeof(AlcadaTipoOperacao))]
    public class AlcadaTipoOperacao : RegraAutorizacao.Alcada<RegraAutorizacaoTabelaFrete, Pedidos.TipoOperacao>
    {
        #region Propriedades Sobrescritas

        public override string Descricao
        {
            get { return PropriedadeAlcada.Descricao; }
        }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Pedidos.TipoOperacao PropriedadeAlcada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoTabelaFrete", Column = "RAT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoTabelaFrete RegrasAutorizacao { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override object ObterValorPropriedadeAlcada()
        {
            return PropriedadeAlcada.Codigo;
        }

        #endregion
    }
}
