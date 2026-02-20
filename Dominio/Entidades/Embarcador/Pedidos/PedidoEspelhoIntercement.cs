using System;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PEDIDO_ESPELHO_INTERCEMENT", DynamicUpdate = true, EntityName = "PedidoEspelhoIntercement", Name = "Dominio.Entidades.Embarcador.Pedidos.PedidoEspelhoIntercement", NameType = typeof(PedidoEspelhoIntercement))]
    public class PedidoEspelhoIntercement : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Pedidos.PedidoEspelhoIntercement>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PEI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaPedido", Column = "CPE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaPedido CargaPedido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "EspelhoIntercement", Column = "EIN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual EspelhoIntercement EspelhoIntercement { get; set; }

        public virtual Dominio.Entidades.Embarcador.Pedidos.PedidoEspelhoIntercement Clonar()
        {
            return (Dominio.Entidades.Embarcador.Pedidos.PedidoEspelhoIntercement)this.MemberwiseClone();
        }

        public virtual bool Equals(PedidoEspelhoIntercement other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual string Descricao
        {
            get
            {
                return this.Codigo.ToString();
            }
        }
    }
}
