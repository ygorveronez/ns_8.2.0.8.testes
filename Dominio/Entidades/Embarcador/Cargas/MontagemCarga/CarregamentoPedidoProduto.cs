using System;

namespace Dominio.Entidades.Embarcador.Cargas.MontagemCarga
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARREGAMENTO_PEDIDO_PRODUTO", EntityName = "CarregamentoPedidoProduto", Name = "Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto", NameType = typeof(CarregamentoPedidoProduto))]
    public class CarregamentoPedidoProduto : EntidadeBase, IEquatable<CarregamentoPedidoProduto>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CPP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CarregamentoPedido", Column = "CRP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CarregamentoPedido CarregamentoPedido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoProduto", Column = "PRP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.PedidoProduto PedidoProduto { get; set; }

        /// <summary>
        /// Contem a quantidade do pedido produto no carregamento.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "Quantidade", Column = "CPP_QUANTIDADE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Quantidade { get; set; }

        /// <summary>
        /// Contem o peso parcial/total do pedido produto no carregamento.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "Peso", Column = "CPP_PESO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Peso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadePallet", Column = "CPP_QUANTIDADE_PALLET", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal QuantidadePallet { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MetroCubico", Column = "CPP_METRO_CUBICO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal MetroCubico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeOriginal", Column = "CPP_QUANTIDADE_ORIG", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal QuantidadeOriginal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadePalletOriginal", Column = "CPP_QUANTIDADE_PALLET_ORIG", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal QuantidadePalletOriginal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MetroCubicoOriginal", Column = "CPP_METRO_CUBICO_ORIG", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal MetroCubicoOriginal { get; set; }

        public virtual bool Equals(CarregamentoPedidoProduto other)
        {
            return (this.Codigo == other.Codigo);
        }
    }
}
