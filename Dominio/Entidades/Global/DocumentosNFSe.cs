using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_NFSE_DOCS", EntityName = "DocumentosNFSe", Name = "Dominio.Entidades.DocumentosNFSe", NameType = typeof(DocumentosNFSe))]
    public class DocumentosNFSe : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "DNS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Chave", Column = "DNS_CHAVE", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Chave { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "DNS_NUMERO", TypeType = typeof(string), NotNull = false)]
        public virtual string Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Serie", Column = "DNS_SERIE", TypeType = typeof(string), Length = 3, NotNull = false)]
        public virtual string Serie { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "DNS_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEmissao", Column = "DNS_DATAEMISSAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Peso", Column = "DNS_PESO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Peso { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NFSe", Column = "NFSE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual NFSe NFSe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Emitente", Column = "DNS_CNPJ_EMITENTE", TypeType = typeof(string), Length = 14, NotNull = false)]
        public virtual string Emitente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Destino", Column = "DNS_CNPJ_DESTINO", TypeType = typeof(string), Length = 14, NotNull = false)]
        public virtual string Destino { get; set; }
    }
}

