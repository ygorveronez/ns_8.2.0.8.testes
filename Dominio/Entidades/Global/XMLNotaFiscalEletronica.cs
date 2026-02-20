using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_XML_NFE", EntityName = "XMLNotaFiscalEletronica", Name = "Dominio.Entidades.XMLNotaFiscalEletronica", NameType = typeof(XMLNotaFiscalEletronica))]
    public class XMLNotaFiscalEletronica : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "XML_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "XML_TRANSPORTADOR", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "XML_EMITENTE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Emitente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Chave", Column = "XML_CHAVENFE", TypeType = typeof(string), Length = 44, NotNull = false)]
        public virtual string Chave { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEmissao", Column = "XML_DATAEMISSAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "XML_VALOR", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PIN", Column = "XML_PIN", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string PIN { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "XML_NUMERONOTA", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Numero { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "XML_DESTINATARIO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Destinatario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Peso", Column = "XML_PESO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Peso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FormaDePagamento", Column = "XML_FORMPGTO", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string FormaDePagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ProdutoPredominante", Column = "XML_PRODPREDOMINANTE", TypeType = typeof(string), Length = 60, NotNull = false)]
        public virtual string ProdutoPredominante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CaminhoXML", Column = "XML_CAMINHO_XML", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string CaminhoXML { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorDoFrete", Column = "XML_VALOR_FRETE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorDoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Placa", Column = "XML_PLACA", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Placa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataImportacao", Column = "XML_DATAIMPORTACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataImportacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GeradoDocumento", Column = "XML_GERADO_DOCUMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GeradoDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Log", Column = "XML_LOG", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Log { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Modalidade", Column = "XML_MODALIDADE", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Modalidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Volumes", Column = "XML_VOLUMES", TypeType = typeof(int), NotNull = false)]
        public virtual int Volumes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Pedido", Column = "XML_PEDIDO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Pedido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "XML_CONTRATANTE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Contratante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UtilizarContratanteComoTomador", Column = "XML_UTILIZAR_CONTRATANTE_COMO_TOMADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarContratanteComoTomador { get; set; }
    }
}
