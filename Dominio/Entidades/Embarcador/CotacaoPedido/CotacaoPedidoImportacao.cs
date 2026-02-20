using System;

namespace Dominio.Entidades.Embarcador.CotacaoPedido
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_COTACAO_PEDIDO_IMPORTACAO", EntityName = "CotacaoPedidoImportacao", Name = "Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoImportacao", NameType = typeof(CotacaoPedidoImportacao))]
    public class CotacaoPedidoImportacao : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoImportacao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CPI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroDI", Column = "CPI_NUMERO_DI", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string NumeroDI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoImportacao", Column = "CPI_CODIGO_IMPORTACAO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string CodigoImportacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoReferencia", Column = "CPI_CODIGO_REFERENCIA", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string CodigoReferencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCarga", Column = "CPI_VALOR_CARGA", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Volume", Column = "CPI_VOLUME", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Volume { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Peso", Column = "CPI_PESO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Peso { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CotacaoPedido", Column = "CTP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido CotacaoPedido { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Codigo.ToString();
            }

        }
        public virtual bool Equals(CotacaoPedidoImportacao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
