namespace Dominio.Entidades.Embarcador.Pedidos.AlcadasAlteracaoPedido
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_AUTORIZACAO_ALCADA_ALTERACAO_PEDIDO", EntityName = "AprovacaoAlcadaAlteracaoPedido", Name = "Dominio.Entidades.Embarcador.Pedidos.AlcadasAlteracaoPedido.AprovacaoAlcadaAlteracaoPedido", NameType = typeof(AprovacaoAlcadaAlteracaoPedido))]
    public class AprovacaoAlcadaAlteracaoPedido : RegraAutorizacao.AprovacaoAlcada<AlteracaoPedido.AlteracaoPedido, RegraAutorizacaoAlteracaoPedido>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AlteracaoPedido", Column = "ALP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override AlteracaoPedido.AlteracaoPedido OrigemAprovacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoAlteracaoPedido", Column = "RAT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoAlteracaoPedido RegraAutorizacao { get; set; }

        #endregion
    }
}
