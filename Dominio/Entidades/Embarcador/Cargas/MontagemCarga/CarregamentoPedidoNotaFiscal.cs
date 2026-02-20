using Dominio.Entidades.Embarcador.Pedidos;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Cargas.MontagemCarga
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARREGAMENTO_PEDIDO_NOTA_FISCAL", EntityName = "CarregamentoPedidoNotaFiscal", DynamicUpdate = true, Name = "Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal", NameType = typeof(CarregamentoPedidoNotaFiscal))]
    public class CarregamentoPedidoNotaFiscal : EntidadeBase, IEquatable<CarregamentoPedidoNotaFiscal>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CPN_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CarregamentoPedido", Column = "CRP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CarregamentoPedido CarregamentoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "NotasFiscais", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PEDIDO_CARREGAMENTO_NOTAS_FISCAIS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CPN_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "XMLNotaFiscal", Column = "NFX_CODIGO")]
        public virtual ICollection<XMLNotaFiscal> NotasFiscais { get; set; }

        public virtual bool Equals(CarregamentoPedidoNotaFiscal other)
        {
            return (this.Codigo == other.Codigo);
        }

        public virtual string Descricao()
        {
            return this.CarregamentoPedido.Descricao;
        }
    }
}
