namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PEDIDO_STAGE", EntityName = "PedidoStage", Name = "Dominio.Entidades.Embarcador.Pedidos.PedidoStage", NameType = typeof(PedidoStage))]
    public class PedidoStage : EntidadeBase
    {

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "PES_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual long Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pedido", Column = "PED_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Pedido Pedido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Stage", Column = "STA_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Stage Stage { get; set; }

    }

}
