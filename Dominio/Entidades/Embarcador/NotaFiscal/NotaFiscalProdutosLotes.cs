using System;

namespace Dominio.Entidades.Embarcador.NotaFiscal
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_NOTA_FISCAL_PRODUTOS_LOTES", EntityName = "NotaFiscalProdutosLotes", Name = "Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalProdutosLotes", NameType = typeof(NotaFiscalProdutosLotes))]
    public class NotaFiscalProdutosLotes : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalProdutosLotes>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "NPL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroLote", Column = "NPL_NUMERO_LOTE", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string NumeroLote { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeLote", Column = "NPL_QUANTIDADE_LOTE", TypeType = typeof(decimal), Scale = 3, Precision = 15, NotNull = false)]
        public virtual decimal QuantidadeLote { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFabricacao", Column = "NPL_DATA_FABRICACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataFabricacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataValidade", Column = "NPL_DATA_VALIDADE", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataValidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoAgregacao", Column = "NPL_CODIGO_AGREGACAO", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string CodigoAgregacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NotaFiscalProdutos", Column = "NFP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual NotaFiscalProdutos NotaFiscalProdutos { get; set; }

        public virtual bool Equals(NotaFiscalProdutosLotes other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalProdutosLotes Clonar()
        {
            return (Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalProdutosLotes)this.MemberwiseClone();
        }
    }
}
