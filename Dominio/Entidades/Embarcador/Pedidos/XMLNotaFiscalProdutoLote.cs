using System;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_XML_NOTA_FISCAL_PRODUTO_LOTE", EntityName = "XMLNotaFiscalProdutoLote", Name = "Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProdutoLote", NameType = typeof(XMLNotaFiscalProdutoLote))]
    public class XMLNotaFiscalProdutoLote : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProdutoLote>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "XPL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "XMLNotaFiscalProduto", Column = "XFP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto XMLNotaFiscalProduto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroLote", Column = "XPL_NUMERO_LOTE", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string NumeroLote { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Quantidade", Column = "XPL_QUANTIDADE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Quantidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFabricacao", Column = "XPL_DATA_FABRICACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataFabricacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataValidade", Column = "XPL_DATA_VALIDADE", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataValidade { get; set; }

        public virtual bool Equals(XMLNotaFiscalProdutoLote other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
