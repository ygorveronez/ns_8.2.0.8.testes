namespace Dominio.Entidades.Embarcador.ProdutorRural
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PEDIDO_COLETA_PRODUTOR", EntityName = "PedidoColetaProdutor", Name = "Dominio.Entidades.Embarcador.ProdutorRural.PedidoColetaProdutor", NameType = typeof(PedidoColetaProdutor))]
    public class PedidoColetaProdutor : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PRO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pedido", Column = "PED_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Pedido Pedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PRO_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedidoColetaProdutor), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedidoColetaProdutor Situacao { get; set; }

    }
}
