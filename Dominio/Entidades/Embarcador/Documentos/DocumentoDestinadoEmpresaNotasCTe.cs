using System;

namespace Dominio.Entidades.Embarcador.Documentos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_DOCUMENTO_DESTINADO_EMPRESA_NOTAS_CTE", EntityName = "DocumentoDestinadoEmpresaNotasCTe", Name = "Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresaNotasCTe", NameType = typeof(DocumentoDestinadoEmpresaNotasCTe))]
    public class DocumentoDestinadoEmpresaNotasCTe : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "DDN_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "DDN_NUMERO", TypeType = typeof(string), NotNull = false)]
        public virtual string Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Serie", Column = "DDN_SERIE", TypeType = typeof(string), Length = 3, NotNull = false)]
        public virtual string Serie { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DocumentoDestinadoEmpresa", Column = "DDE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual DocumentoDestinadoEmpresa DocumentoDestinadoEmpresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "DDN_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloDocumentoFiscal", Column = "MOD_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ModeloDocumentoFiscal ModeloDocumentoFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEmissao", Column = "DDN_DATAEMISSAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Peso", Column = "DDN_PESO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Peso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCalculoICMS", Column = "DDN_BCICMS", TypeType = typeof(decimal), Scale = 2, Precision = 13, NotNull = false)]
        public virtual decimal BaseCalculoICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMS", Column = "DDN_VALORICMS", TypeType = typeof(decimal), Scale = 2, Precision = 13, NotNull = false)]
        public virtual decimal ValorICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCalculoICMSST", Column = "DDN_BCICMSST", TypeType = typeof(decimal), Scale = 2, Precision = 13, NotNull = false)]
        public virtual decimal BaseCalculoICMSST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMSST", Column = "DDN_VALORICMSST", TypeType = typeof(decimal), Scale = 2, Precision = 13, NotNull = false)]
        public virtual decimal ValorICMSST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorProdutos", Column = "DDN_VALORPRODUTOS", TypeType = typeof(decimal), Scale = 2, Precision = 13, NotNull = false)]
        public virtual decimal ValorProdutos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CFOP", Column = "DDN_CFOP", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string CFOP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ChaveNFE", Column = "DDN_CHAVENFE", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string ChaveNFE { get; set; }
      
        public virtual DocumentoDestinadoEmpresaNotasCTe Clonar()
        {
            return (DocumentoDestinadoEmpresaNotasCTe)this.MemberwiseClone();
        }
    }
}
