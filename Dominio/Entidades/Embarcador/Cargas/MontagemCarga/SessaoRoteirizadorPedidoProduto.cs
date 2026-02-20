using System;

namespace Dominio.Entidades.Embarcador.Cargas.MontagemCarga
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_SESSAO_ROTEIRIZADOR_PEDIDO_PRODUTO", EntityName = "SessaoRoteirizadorPedidoProduto", Name = "Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedidoProduto", NameType = typeof(SessaoRoteirizadorPedidoProduto))]
    public class SessaoRoteirizadorPedidoProduto : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedidoProduto>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "SPP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "SessaoRoteirizadorPedido", Column = "SRP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual SessaoRoteirizadorPedido SessaoRoteirizadorPedido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoProduto", Column = "PRP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.PedidoProduto PedidoProduto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeProdutoSessao", Column = "SPP_QUANTIDADE_PRODUTO_SESSAO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal QuantidadeProdutoSessao { get; set; }

        public virtual bool Equals(SessaoRoteirizadorPedidoProduto other)
        {
            return (this.Codigo == other.Codigo);
        }
    }
}
