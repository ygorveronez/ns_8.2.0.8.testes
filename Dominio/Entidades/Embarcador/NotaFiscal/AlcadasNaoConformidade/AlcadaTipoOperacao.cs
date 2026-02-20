namespace Dominio.Entidades.Embarcador.NotaFiscal.AlcadasNaoConformidade
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_NAO_CONFORMIDADE_TIPO_OPERACAO", EntityName = "AlcadasNaoConformidade.AlcadaTipoOperacao", Name = "Dominio.Entidades.Embarcador.NotaFiscal.AlcadasNaoConformidade.AlcadaTipoOperacao", NameType = typeof(AlcadaTipoOperacao))]
    public class AlcadaTipoOperacao : RegraAutorizacao.Alcada<RegraAutorizacaoNaoConformidade, Pedidos.TipoOperacao>
    {
        #region Propriedades Sobrescritas

        public override string Descricao
        {
            get { return PropriedadeAlcada.Descricao; }
        }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Pedidos.TipoOperacao PropriedadeAlcada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoNaoConformidade", Column = "RAT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Dominio.Entidades.Embarcador.NotaFiscal.AlcadasNaoConformidade.RegraAutorizacaoNaoConformidade RegrasAutorizacao { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override object ObterValorPropriedadeAlcada()
        {
            return PropriedadeAlcada.Codigo;
        }

        #endregion
    }
}
