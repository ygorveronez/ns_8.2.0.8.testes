using System;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    /*
     * Entidade N:M entre Pedido de Fronteiras (Que são Clientes, mas com a flag Fronteira ativada)
     */

    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PEDIDO_FRONTEIRA", EntityName = "PedidoFronteira", Name = "Dominio.Entidades.Embarcador.Pedidos.PedidoFronteira", NameType = typeof(PedidoFronteira))]
    public class PedidoFronteira : EntidadeBase, IEquatable<PedidoFronteira>
    {

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PEF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pedido", Column = "PED_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Pedido Pedido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Fronteira { get; set; }


        public virtual string Descricao
        {
            get
            {
                return this.Pedido.Descricao + " | " + Fronteira.Descricao;
            }
        }

        public virtual bool Equals(PedidoFronteira other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public override int GetHashCode()
        {
            return Fronteira.CPF_CNPJ.GetHashCode();
        }

        public virtual Dominio.Entidades.Embarcador.Pedidos.PedidoFronteira Clonar()
        {
            return (Dominio.Entidades.Embarcador.Pedidos.PedidoFronteira)this.MemberwiseClone();
        }
    }
}
