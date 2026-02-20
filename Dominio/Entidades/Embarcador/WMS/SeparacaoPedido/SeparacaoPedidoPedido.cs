namespace Dominio.Entidades.Embarcador.WMS
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_SEPARACAO_PEDIDO_PEDIDO", EntityName = "SeparacaoPedidoPedido", Name = "Dominio.Entidades.Embarcador.WMS.SeparacaoPedidoPedido", NameType = typeof(SeparacaoPedidoPedido))]
    public class SeparacaoPedidoPedido : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "SPP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "SeparacaoPedido", Column = "SPE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual SeparacaoPedido SeparacaoPedido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pedido", Column = "PED_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.Pedido Pedido { get; set; }

        public virtual string Descricao
        {
            get
            {
                return SeparacaoPedido?.Descricao ?? string.Empty;
            }
        }
    }
}
