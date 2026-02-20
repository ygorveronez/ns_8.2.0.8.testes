using System;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PEDIDO_IMPORTACAO", EntityName = "PedidoImportacao", Name = "Dominio.Entidades.Embarcador.Pedidos.PedidoImportacao", NameType = typeof(PedidoImportacao))]
    public class PedidoImportacao : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Pedidos.PedidoImportacao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PEI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroDI", Column = "PEI_NUMERO_DI", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string NumeroDI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoImportacao", Column = "PEI_CODIGO_IMPORTACAO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string CodigoImportacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoReferencia", Column = "PEI_CODIGO_REFERENCIA", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string CodigoReferencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCarga", Column = "PEI_VALOR_CARGA", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Volume", Column = "PEI_VOLUME", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Volume { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Peso", Column = "PEI_PESO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Peso { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pedido", Column = "PED_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Pedido Pedido { get; set; }

        public virtual bool Equals(PedidoImportacao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual PedidoImportacao Clonar()
        {
            return (PedidoImportacao)this.MemberwiseClone();
        }
    }
}
