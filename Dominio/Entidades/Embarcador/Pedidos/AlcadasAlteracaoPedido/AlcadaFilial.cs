namespace Dominio.Entidades.Embarcador.Pedidos.AlcadasAlteracaoPedido
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_ALTERACAO_PEDIDO_FILIAL", EntityName = "AlcadasAlteracaoPedido.AlcadaFilial", Name = "Dominio.Entidades.Embarcador.Pedidos.AlcadasAlteracaoPedido.AlcadaFilial", NameType = typeof(AlcadaFilial))]
    public class AlcadaFilial : RegraAutorizacao.Alcada<RegraAutorizacaoAlteracaoPedido, Filiais.Filial>
    {
        #region Propriedades Sobrescritas

        public override string Descricao
        {
            get { return PropriedadeAlcada.Descricao; }
        }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Filiais.Filial PropriedadeAlcada { get; set; }

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
