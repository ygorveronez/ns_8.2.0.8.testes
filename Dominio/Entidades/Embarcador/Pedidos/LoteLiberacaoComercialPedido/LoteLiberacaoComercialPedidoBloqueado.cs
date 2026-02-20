namespace Dominio.Entidades.Embarcador.Pedidos.LoteLiberacaoComercialPedido
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_LOTE_LIBERACAO_COMERCIAL_PEDIDO_BLOQUEADO", EntityName = "LoteLiberacaoComercialPedidoBloqueado", Name = "Dominio.Entidades.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedidoBloqueado", NameType = typeof(LoteLiberacaoComercialPedidoBloqueado))]
    public class LoteLiberacaoComercialPedidoBloqueado : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "LLB_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LoteLiberacaoComercialPedido", Column = "LLC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual LoteLiberacaoComercialPedido LoteLiberacaoComercialPedido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pedido", Column = "PED_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedido Pedido { get; set; }
    }
}
