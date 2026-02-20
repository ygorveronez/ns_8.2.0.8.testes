using System;

namespace Dominio.Entidades.Embarcador.Cargas.MontagemCarga
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PEDIDO_PRODUTO_CANCELAMENTO_RESERVA_INTEGRACAO", EntityName = "PedidoProdutoCancelamentoReservaIntegracao", Name = "Dominio.Entidades.Embarcador.Cargas.MontagemCarga.PedidoProdutoCancelamentoReservaIntegracao", NameType = typeof(PedidoProdutoCancelamentoReservaIntegracao))]
    public class PedidoProdutoCancelamentoReservaIntegracao : EntidadeBase, IEquatable<PedidoProdutoCancelamentoReservaIntegracao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CRI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoProduto", Column = "PRP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.PedidoProduto PedidoProduto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoCancelamentoReservaIntegracao", Column = "CPI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PedidoCancelamentoReservaIntegracao PedidoCancelamentoReservaIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Quantidade", Column = "CPP_QUANTIDADE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Quantidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Peso", Column = "CPP_PESO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Peso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadePallet", Column = "CPP_QUANTIDADE_PALLET", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal QuantidadePallet { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MetroCubico", Column = "CPP_METRO_CUBICO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal MetroCubico { get; set; }

        public virtual bool Equals(PedidoProdutoCancelamentoReservaIntegracao other)
        {
            return (other.Codigo == this.Codigo);
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
