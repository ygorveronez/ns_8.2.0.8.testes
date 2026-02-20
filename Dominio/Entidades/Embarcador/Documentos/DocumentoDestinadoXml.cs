using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.Entidades.Embarcador.Documentos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_DOCUMENTO_DESTINADO_XML", EntityName = "DocumentoDestinadoXml", Name = "Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoXml", NameType = typeof(DocumentoDestinadoXml))]
    public class DocumentoDestinadoXml : EntidadeBase
    {
        public DocumentoDestinadoXml() { }
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "DDX_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual long Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroSequencialUnico", Column = "DDX_NUMERO_SEQUENCIAL_UNICO", TypeType = typeof(long), NotNull = false)]
        public virtual long NumeroSequencialUnico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Chave", Column = "DDX_CHAVE", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Chave { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DDX_DATA_INTEGRACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoEvento", Column = "DDX_TIPO_EVENTO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string TipoEvento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeSchema", Column = "DDX_NOME_SCHEMA", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string NomeSchema { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Versao", Column = "DDX_VERSAO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Versao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Mensagem", Column = "DDX_MENSAGEM", Type = "StringClob", NotNull = false)]
        public virtual string Mensagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ConteudoXml", Column = "DDX_CONTEUDO_XML", Type = "StringClob", NotNull = false)]
        public virtual string ConteudoXml { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoDocumento", Column = "DDX_TIPO_DOCUMENTO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa TipoDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoXml", Column = "DDX_SITUACAO_XML", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoXml), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoXml SituacaoXml { get; set; }

    }
}

