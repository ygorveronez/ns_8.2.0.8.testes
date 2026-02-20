namespace Dominio.Entidades.Embarcador.Frete.AlcadasTaxaDescarga
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_TAXA_DESCARGA_TIPO_OPERACAO", EntityName = "AlcadasTaxaDescarga.AlcadaTipoOperacao", Name = "Dominio.Entidades.Embarcador.Frete.AlcadasTaxaDescarga.AlcadaTipoOperacao", NameType = typeof(AlcadaTipoOperacao))]
    public class AlcadaTipoOperacao : RegraAutorizacao.Alcada<RegraAutorizacaoTaxaDescarga, Pedidos.TipoOperacao>
    {
        #region Propriedades Sobrescritas

        public override string Descricao
        {
            get { return PropriedadeAlcada.Descricao; }
        }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Pedidos.TipoOperacao PropriedadeAlcada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoTaxaDescarga", Column = "RAT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoTaxaDescarga RegrasAutorizacao { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override object ObterValorPropriedadeAlcada()
        {
            return PropriedadeAlcada.Codigo;
        }

        #endregion
    }
}
