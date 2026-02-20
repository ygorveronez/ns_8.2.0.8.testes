using System;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PEDIDO_PRODUTO_ONU", EntityName = "PedidoProdutoONU", Name = "Dominio.Entidades.Embarcador.Pedidos.PedidoProdutoONU", NameType = typeof(PedidoProdutoONU))]

    public class PedidoProdutoONU : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Pedidos.PedidoProdutoONU>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PPO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoProduto", Column = "PRP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.PedidoProduto PedidoProduto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ClassificacaoRiscoONU", Column = "CRO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.ClassificacaoRiscoONU ClassificacaoRiscoONU { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "PPO_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }

        public virtual Dominio.Entidades.Embarcador.Pedidos.PedidoProdutoONU Clonar()
        {
            return (Dominio.Entidades.Embarcador.Pedidos.PedidoProdutoONU)this.MemberwiseClone();
        }

        public virtual bool Equals(PedidoProdutoONU other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
