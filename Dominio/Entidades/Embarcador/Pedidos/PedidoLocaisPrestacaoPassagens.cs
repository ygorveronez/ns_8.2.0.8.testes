using System;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PEDIDO_LOCAIS_PRESTACAO_PASSAGENS", EntityName = "PedidoLocaisPrestacaoPassagens", Name = "Dominio.Entidades.Embarcador.Pedidos.PedidoLocaisPrestacaoPassagens", NameType = typeof(PedidoLocaisPrestacaoPassagens))]
    public class PedidoLocaisPrestacaoPassagens : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Pedidos.PedidoLocaisPrestacaoPassagens>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CPP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pedido", Column = "PED_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedido Pedido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Estado", Column = "UF_PASSAGEM", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Estado EstadoDePassagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Posicao", Column = "PPP_POSICAO", TypeType = typeof(int), NotNull = true)]
        public virtual int Posicao { get; set; }

        public virtual bool Equals(PedidoLocaisPrestacaoPassagens other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
