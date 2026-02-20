namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PEDIDO_RESTRICAO_DIA_ENTREGA", EntityName = "PedidoRestricaoDiaEntrega", Name = "Dominio.Entidades.Embarcador.Pedidos.PedidoRestricaoDiaEntrega", NameType = typeof(PedidoRestricaoDiaEntrega))]
    public class PedidoRestricaoDiaEntrega : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PRE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pedido", Column = "PED_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Pedido Pedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Dia", Column = "PRE_DIA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana Dia { get; set; }
    }
}
