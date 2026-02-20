namespace Dominio.Entidades.Embarcador.Pedidos.AlcadasAlteracaoPedido
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_ALTERACAO_PEDIDO_TIPO_CARGA", EntityName = "AlcadasAlteracaoPedido.AlcadaTipoCarga", Name = "Dominio.Entidades.Embarcador.Pedidos.AlcadasAlteracaoPedido.AlcadaTipoCarga", NameType = typeof(AlcadaTipoCarga))]
    public class AlcadaTipoCarga : RegraAutorizacao.Alcada<RegraAutorizacaoAlteracaoPedido, Cargas.TipoDeCarga>
    {
        #region Propriedades Sobrescritas

        public override string Descricao
        {
            get { return PropriedadeAlcada.Descricao; }
        }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeCarga", Column = "TCG_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Cargas.TipoDeCarga PropriedadeAlcada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoAlteracaoPedido", Column = "RAT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoAlteracaoPedido RegrasAutorizacao { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override object ObterValorPropriedadeAlcada()
        {
            return PropriedadeAlcada.Codigo;
        }

        #endregion
    }
}
