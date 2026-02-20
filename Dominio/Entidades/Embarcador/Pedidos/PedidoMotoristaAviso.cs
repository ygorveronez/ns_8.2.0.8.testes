using System;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PEDIDO_MOTORISTA_AVISO", EntityName = "PedidoMotoristaAviso", Name = "Dominio.Entidades.Embarcador.Pedidos.PedidoMotoristaAviso", NameType = typeof(PedidoMotoristaAviso))]
    public class PedidoMotoristaAviso : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PMA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroSequencia", Column = "PMA_NUMERO_SEQUENCIA", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroSequencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "PMA_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ciente", Column = "PMA_CIENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ciente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pedido", Column = "PED_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedido Pedido { get; set; }
    }
}
