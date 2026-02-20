using System;

namespace Dominio.Entidades.Embarcador.PedidoVenda
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PEDIDO_VENDA_PARCELA", EntityName = "PedidoVendaParcela", Name = "Dominio.Entidades.Embarcador.PedidoVenda.PedidoVendaParcela", NameType = typeof(PedidoVendaParcela))]
    public class PedidoVendaParcela : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.PedidoVenda.PedidoVendaParcela>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PVP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "PVP_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 13, NotNull = false)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Desconto", Column = "PVP_DESCONTO", TypeType = typeof(decimal), Scale = 2, Precision = 13, NotNull = false)]
        public virtual decimal Desconto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataVencimento", Column = "PVP_DATA_VENCIMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataVencimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Sequencia", Column = "PVP_SEQUENCIA", TypeType = typeof(int), NotNull = false)]
        public virtual int Sequencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Forma", Column = "PVP_FORMA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo Forma { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoVenda", Column = "PEV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.PedidoVenda.PedidoVenda PedidoVenda { get; set; }

        public virtual bool Equals(PedidoVendaParcela other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}