namespace Dominio.Entidades.Embarcador.Pedidos.AlcadasAlteracaoPedido
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_ALTERACAO_PEDIDO_TRANSPORTADOR", EntityName = "AlcadasAlteracaoPedido.AlcadaTransportador", Name = "Dominio.Entidades.Embarcador.Pedidos.AlcadasAlteracaoPedido.AlcadaTransportador", NameType = typeof(AlcadaTransportador))]
    public class AlcadaTransportador : RegraAutorizacao.Alcada<RegraAutorizacaoAlteracaoPedido, Empresa>
    {
        #region Propriedades Sobrescritas

        public override string Descricao
        {
            get { return PropriedadeAlcada.Descricao; }
        }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Empresa PropriedadeAlcada { get; set; }

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
