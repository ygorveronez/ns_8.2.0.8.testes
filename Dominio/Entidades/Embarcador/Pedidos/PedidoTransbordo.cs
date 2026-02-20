using System;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PEDIDO_TRANSBORDO", EntityName = "PedidoTransbordo", Name = "Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo", NameType = typeof(PedidoTransbordo))]
    public class PedidoTransbordo : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PET_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pedido", Column = "PED_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedido Pedido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Navio", Column = "NAV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Navio Navio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Porto", Column = "POT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Porto Porto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoTerminalImportacao", Column = "TTI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoTerminalImportacao Terminal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PET_SEQUENCIA", TypeType = typeof(int), NotNull = false)]
        public virtual int Sequencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoViagemNavio", Column = "PVN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PedidoViagemNavio PedidoViagemNavio { get; set; }

        public virtual bool Equals(PedidoTransbordo other)
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
                return this.Sequencia.ToString("D") + " - " + this.PedidoViagemNavio?.Descricao + " - " + this.Porto?.Descricao;
            }
        }

        public virtual PedidoTransbordo Clonar()
        {
            return (PedidoTransbordo)this.MemberwiseClone();
        }
    }
}
