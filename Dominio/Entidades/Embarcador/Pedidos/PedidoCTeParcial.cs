using System;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PEDIDO_CTE_PARCIAL", EntityName = "PedidoCTeParcial", Name = "Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParcial", NameType = typeof(PedidoCTeParcial))]
    public class PedidoCTeParcial : EntidadeBase, IEquatable<PedidoCTeParcial>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PCP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pedido", Column = "PED_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedido Pedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "PCP_NUMERO", TypeType = typeof(int), NotNull = true)]
        public virtual int Numero { get; set; }

        public virtual bool Equals(PedidoCTeParcial other)
        {
            return (this.Codigo == other.Codigo);
        }
    }
}
